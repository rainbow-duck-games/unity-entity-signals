using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using EntitySignals.Context;
using EntitySignals.Handlers;
using EntitySignals.Utility.Tact;

namespace EntitySignals.Storage {
    public class EntitySignalsStorage {
        private readonly IHandlersResolver _resolver;

        private ConditionalWeakTable<object, List<ReceiverDelegate>> _delegates =
            new ConditionalWeakTable<object, List<ReceiverDelegate>>();

        public EntitySignalsStorage(IHandlersResolver resolver) {
            _resolver = resolver;
        }

        public int Count => 0; // ToDo

        public IContext<TEntity> On<TEntity>(TEntity entity) {
            if (entity == null)
                throw new Exception("Can't create a Entity Signals context for empty entity");

            return new EntityContext<TEntity>(_resolver, this, entity);
        }

        public void Dispose() {
            _delegates = new ConditionalWeakTable<object, List<ReceiverDelegate>>();
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

        private sealed class EntityContext<TEntity> : IContext<TEntity> {
            private readonly IHandlersResolver _resolver;
            private readonly EntitySignalsStorage _entitySignalsStorage;
            private readonly TEntity _entity;

            public EntityContext(IHandlersResolver resolver, EntitySignalsStorage entitySignalsStorage, TEntity entity = default) {
                _resolver = resolver;
                _entitySignalsStorage = entitySignalsStorage;
                _entity = entity;
            }

            public void Add(object receiver) {
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

                _entitySignalsStorage.GetDelegates(_entity).AddRange(handlers);
            }

            public void Remove(object receiver) {
                _entitySignalsStorage.GetDelegates(_entity)
                    .RemoveAll(rd => rd.Receiver == receiver);
            }

            public void Add<TSignal>(ESHandler<TSignal> signalHandler) {
                _entitySignalsStorage.GetDelegates(_entity)
                    .Add(new ReceiverDelegate(typeof(TSignal), signalHandler.GetInvoker(), signalHandler, 1));
            }

            public void Add<TSignal>(ESHandler<TEntity, TSignal> signalHandler) {
                _entitySignalsStorage.GetDelegates(_entity)
                    .Add(new ReceiverDelegate(typeof(TSignal), signalHandler.GetInvoker(), signalHandler));
            }

            public void Remove<TSignal>(ESHandler<TSignal> signalHandler) {
                _entitySignalsStorage.GetDelegates(_entity)
                    .RemoveAll(c => ReferenceEquals(c.Receiver, signalHandler));
            }

            public void Remove<TSignal>(ESHandler<TEntity, TSignal> signalHandler) {
                _entitySignalsStorage.GetDelegates(_entity)
                    .RemoveAll(c => ReferenceEquals(c.Receiver, signalHandler));
            }

            public void Dispose() {
                _entitySignalsStorage.Dispose(_entity);
            }

            public void Send<TSignal>(TSignal arg) {
                _entitySignalsStorage.Send(_entity, arg);
            }

            public int Count => _entitySignalsStorage.GetDelegates(_entity).Count;
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