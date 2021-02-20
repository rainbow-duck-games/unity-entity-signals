using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using EntitySignals.Context;
using EntitySignals.Handlers;
using EntitySignals.Utility.Tact;

namespace EntitySignals.Storage {
    public class EntityStorage : IStorage {
        private readonly IHandlersResolver _resolver;

        private readonly ConditionalWeakTable<object, List<ReceiverDelegate>> _delegates =
            new ConditionalWeakTable<object, List<ReceiverDelegate>>();

        public EntityStorage(IHandlersResolver resolver) {
            _resolver = resolver;
        }

        public int Count => 0;

        public IContext<object> On() {
            throw new NotImplementedException();
        }

        public IContext<TEntity> On<TEntity>(TEntity entity) {
            if (entity == null)
                throw new Exception("Can't create a Entity Signals context for empty entity");

            return new EntityContext<TEntity>(_resolver, this, entity);
        }

        public IContext<TEntity> On<TEntity>(Predicate<TEntity> entity) {
            throw new NotImplementedException();
        }

        public void Dispose() {
            throw new NotImplementedException();
        }

        public void Dispose<TEntity>(TEntity entity) {
            GetDelegates(entity).Clear();
        }

        public void Send<TEntity, TSignal>(TEntity entity, TSignal arg) {
            // if (Delegates.Count == 0)
            //     throw new Exception(""); // ToDo Strict mode?

            GetDelegates(entity)
                .ForEach(rd => {
                    if (!rd.Valid(arg))
                        return;

                    switch (rd.Args) {
                        case 1:
                            rd.Invoker.Invoke(rd.Receiver, arg);
                            break;
                        case 2:
                            rd.Invoker.Invoke(rd.Receiver, entity, arg);
                            break;
                    }
                });
        }

        private List<ReceiverDelegate> GetDelegates<TEntity>(TEntity entity) {
            return _delegates.GetValue(entity, k => new List<ReceiverDelegate>());
        }

        private class EntityContext<TEntity> : IContext<TEntity> {
            private readonly IHandlersResolver _resolver;
            private readonly EntityStorage _entityStorage;
            private readonly TEntity _entity;

            public EntityContext(IHandlersResolver resolver, EntityStorage entityStorage, TEntity entity = default) {
                _resolver = resolver;
                _entityStorage = entityStorage;
                _entity = entity;
            }

            public virtual void Add(object receiver) {
                var receiverType = receiver.GetType();
                var entityType = typeof(TEntity);
                var handlers = _resolver.GetHandlers(receiverType)
                    .Where(meta => meta.RequiredType == null || meta.RequiredType.IsAssignableFrom(entityType))
                    .Select(meta =>
                        new ReceiverDelegate(meta.SignalType, meta.MethodInvoker, receiver, meta.ParamCount))
                    .ToArray();
                if (handlers.Length == 0)
                    throw new Exception(
                        $"Can't bind method {receiver.GetType().Name} to entity {typeof(TEntity)}: No methods matched signature");

                _entityStorage.GetDelegates(_entity).AddRange(handlers);
            }

            public virtual void Remove(object receiver) {
                _entityStorage.GetDelegates(_entity)
                    .RemoveAll(rd => rd.Receiver == receiver);
            }

            public virtual void Add<TSignal>(ESHandler<TSignal> signalHandler) {
                _entityStorage.GetDelegates(_entity)
                    .Add(new ReceiverDelegate(typeof(TSignal), signalHandler.GetInvoker(), signalHandler, 1));
            }

            public virtual void Add<TSignal>(ESHandler<TEntity, TSignal> signalHandler) {
                _entityStorage.GetDelegates(_entity)
                    .Add(new ReceiverDelegate(typeof(TSignal), signalHandler.GetInvoker(), signalHandler));
            }

            public virtual void Remove<TSignal>(ESHandler<TEntity> signalHandler) {
                _entityStorage.GetDelegates(_entity)
                    .RemoveAll(c => ReferenceEquals(c.Receiver, signalHandler));
            }

            public virtual void Remove<TSignal>(ESHandler<TEntity, TSignal> signalHandler) {
                _entityStorage.GetDelegates(_entity)
                    .RemoveAll(c => ReferenceEquals(c.Receiver, signalHandler));
            }

            public virtual void Dispose() {
                _entityStorage.Dispose(_entity);
            }

            public virtual void Send<TSignal>(TSignal arg) {
                _entityStorage.Send(_entity, arg);
            }

            public int Count => _entityStorage.GetDelegates(_entity).Count;
        }
    }

    public class ReceiverDelegate {
        public readonly Type SignalType;
        public readonly EfficientInvoker Invoker;
        public readonly object Receiver;
        public readonly int Args;

        public ReceiverDelegate(Type signalType, EfficientInvoker invoker, object receiver, int args = 2) {
            SignalType = signalType;
            Invoker = invoker;
            Receiver = receiver;
            Args = args;
        }

        public bool Valid(object signal) {
            return SignalType == signal.GetType();
        }
    }
}