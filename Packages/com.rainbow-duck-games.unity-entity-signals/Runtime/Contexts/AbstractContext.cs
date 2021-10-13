using System;
using System.Collections.Generic;
using System.Linq;
using RainbowDuckGames.UnityEntitySignals.Handlers;
using RainbowDuckGames.UnityEntitySignals.Utility.Tact;
using UnityEngine;

namespace RainbowDuckGames.UnityEntitySignals.Contexts {
    public abstract class AbstractContext<TEntity> : IContext<TEntity> {
        protected readonly IHandlersResolver Resolver;

        protected AbstractContext(IHandlersResolver resolver) {
            Resolver = resolver;
        }

        public virtual int Count => GetContextDelegates().Count;

        public virtual void Add(object receiver) {
            var receiverType = receiver.GetType();
            var entityType = typeof(TEntity);
            var handlers = Resolver.GetHandlers(receiverType)
                .Where(meta => meta.RequiredType == null || meta.RequiredType.IsAssignableFrom(entityType))
                .Select(meta =>
                    new HandlerDelegate(meta.SignalType, meta.MethodInvoker, receiver, meta.ParamCount))
                .ToArray();
            if (handlers.Length == 0)
                throw new Exception(
                    $"Can't bind method {receiver.GetType().Name} to entity {typeof(TEntity)}: No methods matched signature");

            AddHandlers(handlers);
        }

        public void Add<TSignal>(ESHandler<TSignal> signalHandler) {
            AddHandlers(new HandlerDelegate(typeof(TSignal), EfficientInvoker.ForDelegate(signalHandler), signalHandler,
                1));
        }

        public void Add<TSignal>(ESHandler<TEntity, TSignal> signalHandler) {
            AddHandlers(
                new HandlerDelegate(typeof(TSignal), EfficientInvoker.ForDelegate(signalHandler), signalHandler));
        }

        public void Remove(object receiver) {
            RemoveHandlers(receiver);
        }

        public void Remove<TSignal>(ESHandler<TSignal> signalHandler) {
            RemoveHandlers(signalHandler);
        }

        public void Remove<TSignal>(ESHandler<TEntity, TSignal> signalHandler) {
            RemoveHandlers(signalHandler);
        }

        public abstract void Send<TSignal>(TSignal arg);

        protected void ExecuteSend<TSignal>(TEntity entity, TSignal signal, List<HandlerDelegate> delegates) {
            delegates
                .FindAll(handler => handler.Valid(entity, signal))
                .ForEach(handler => handler.Invoke(entity, signal));
        }

        public abstract void Dispose();

        protected abstract List<HandlerDelegate> GetContextDelegates();

        private void AddHandlers(params HandlerDelegate[] handlers) {
            GetContextDelegates().AddRange(handlers);
        }

        private void RemoveHandlers(object receiver) {
            GetContextDelegates().RemoveAll(handler => !handler.IsAlive(out _));
            GetContextDelegates().RemoveAll(handler => handler.ReceiverEquals(receiver));
        }
    }
}