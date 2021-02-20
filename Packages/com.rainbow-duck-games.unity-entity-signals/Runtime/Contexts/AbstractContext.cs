using System;
using System.Collections.Generic;
using System.Linq;
using EntitySignals.Handlers;
using EntitySignals.Utility.Tact;

namespace EntitySignals.Contexts {
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

            GetContextDelegates().AddRange(handlers);
        }

        public void Remove(object receiver) {
            GetContextDelegates()
                .RemoveAll(rd => rd.Receiver == receiver);
        }

        public void Add<TSignal>(ESHandler<TSignal> signalHandler) {
            GetContextDelegates()
                .Add(new HandlerDelegate(typeof(TSignal), signalHandler.GetInvoker(), signalHandler, 1));
        }

        public void Add<TSignal>(ESHandler<TEntity, TSignal> signalHandler) {
            GetContextDelegates()
                .Add(new HandlerDelegate(typeof(TSignal), signalHandler.GetInvoker(), signalHandler));
        }

        public void Remove<TSignal>(ESHandler<TSignal> signalHandler) {
            GetContextDelegates()
                .RemoveAll(c => ReferenceEquals(c.Receiver, signalHandler));
        }

        public void Remove<TSignal>(ESHandler<TEntity, TSignal> signalHandler) {
            GetContextDelegates()
                .RemoveAll(c => ReferenceEquals(c.Receiver, signalHandler));
        }

        public abstract void Send<TSignal>(TSignal arg);

        protected void ExecuteSend(object entity, object signal) {
            // TODO Strict mode
            
            GetContextDelegates()
                .ForEach(rd => {
                    if (!rd.Valid(entity, signal))
                        return;

                    switch (rd.Args) {
                        case 1:
                            rd.Invoker.Invoke(rd.Receiver, signal);
                            break;
                        case 2:
                            rd.Invoker.Invoke(rd.Receiver, entity, signal);
                            break;
                    }
                });
        }
        
        public abstract void Dispose();

        protected abstract List<HandlerDelegate> GetContextDelegates();
    }
}