using System;
using System.Linq;
using EntitySignals.Handlers;
using EntitySignals.Utility.Tact;

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
            var receiverType = receiver.GetType();
            var entityType = typeof(TEntity);
            var handlers = _resolver.GetHandlers(receiverType)
                .Where(meta => meta.RequiredType == null || meta.RequiredType.IsAssignableFrom(entityType))
                .Select(meta => new ReceiverDelegate(meta.SignalType, meta.MethodInvoker, receiver, meta.ParamCount))
                .ToArray();
            if (handlers.Length == 0)
                throw new Exception(
                    $"Can't bind method {receiver.GetType().Name} to entity {typeof(TEntity)}: No methods matched signature");
            
            _entitySignals.GetDelegates(_entity).AddRange(handlers);
        }

        public virtual void Remove(object receiver) {
            _entitySignals.GetDelegates(_entity)
                .RemoveAll(rd => rd.Receiver == receiver);
        }

        public virtual void Add<TSignal>(ESHandler<TSignal> signalHandler) {
            _entitySignals.GetDelegates(_entity)
                .Add(new ReceiverDelegate(typeof(TSignal), signalHandler.GetInvoker(), signalHandler, 1));
        }

        public virtual void Add<TSignal>(ESHandler<TEntity, TSignal> signalHandler) {
            _entitySignals.GetDelegates(_entity)
                .Add(new ReceiverDelegate(typeof(TSignal), signalHandler.GetInvoker(), signalHandler));
        }

        public virtual void Remove<TSignal>(ESHandler<TEntity> signalHandler) {
            _entitySignals.GetDelegates(_entity)
                .RemoveAll(c => ReferenceEquals(c.Receiver, signalHandler));
        }

        public virtual void Remove<TSignal>(ESHandler<TEntity, TSignal> signalHandler) {
            _entitySignals.GetDelegates(_entity)
                .RemoveAll(c => ReferenceEquals(c.Receiver, signalHandler));
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