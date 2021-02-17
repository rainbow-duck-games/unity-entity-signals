using System;
using EntitySignals.Handlers;

namespace EntitySignals.Context {
    public class EntityContext<TEntity> : IContext<TEntity> {
        private readonly IHandlersResolver _resolver;
        private readonly EntitySignals _entitySignals;
        private readonly TEntity _entity;

        public EntityContext(IHandlersResolver resolver, EntitySignals entitySignals, TEntity entity = default) {
            _resolver = resolver;
            _entitySignals = entitySignals;
            _entity = entity;
        }

        public virtual void Add(object receiver) {
            EachReceiveInternal(receiver, @delegate => { _entitySignals.GetDelegates(_entity).Add(@delegate); });
        }

        public virtual void Remove(object receiver) {
            // ToDo Optimize with receiver-based dictionary
            EachReceiveInternal(receiver, @delegate => { _entitySignals.GetDelegates(_entity).Remove(@delegate); });
        }

        private void EachReceiveInternal(object receiver, Action<Delegate> action) {
            var receiverType = receiver.GetType();
            var entityType = typeof(TEntity);
            var meta = _resolver.GetHandlers(receiverType);

            // Process meta
            var count = 0;
            foreach (var candidate in meta) {
                if (candidate.ParamCount == 1) {
                    if (candidate.RequiredType != null && !candidate.RequiredType.IsAssignableFrom(entityType))
                        continue;

                    var delegateType = typeof(ESHandler<>).MakeGenericType(candidate.SignalType);
                    var @delegate = Delegate.CreateDelegate(delegateType, receiver, candidate.MethodInfo);
                    action.Invoke(@delegate);
                    count++;
                } else if (candidate.ParamCount == 2) {
                    if (!candidate.RequiredType.IsAssignableFrom(entityType))
                        continue;

                    var delegateType = typeof(ESHandler<,>).MakeGenericType(typeof(TEntity), candidate.SignalType);
                    var @delegate = Delegate.CreateDelegate(delegateType, receiver, candidate.MethodInfo);
                    action.Invoke(@delegate);
                    count++;
                } else {
                    throw new Exception("ToDo"); // TODo
                }
            }

            if (count == 0)
                throw new Exception(
                    $"Can't bind method {receiver.GetType().Name} to entity {typeof(TEntity)}: No methods matched signature");
        }

        public virtual void Add<TSignal>(ESHandler<TSignal> signalHandler) {
            _entitySignals.GetDelegates(_entity).Add(signalHandler);
        }

        public virtual void Add<TSignal>(ESHandler<TEntity, TSignal> signalHandler) {
            _entitySignals.GetDelegates(_entity).Add(signalHandler);
        }

        public virtual void Remove<TSignal>(ESHandler<TEntity> signalHandler) {
            _entitySignals.GetDelegates(_entity).Remove(signalHandler);
        }

        public virtual void Remove<TSignal>(ESHandler<TEntity, TSignal> signalHandler) {
            _entitySignals.GetDelegates(_entity).Remove(signalHandler);
        }

        public virtual void Dispose() {
            _entitySignals.Dispose(_entity);
        }

        public virtual void Send<TSignal>(TSignal arg) {
            _entitySignals.Send(_entity, arg);
        }

        public int Count => _entitySignals.GetDelegates(_entity).Count;
    }
}