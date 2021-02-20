using System.Collections.Generic;
using EntitySignals.Handlers;
using EntitySignals.Storage;

namespace EntitySignals.Contexts {
    public class EntityContext<TEntity> : AbstractContext<TEntity> {
        private readonly EntitySignalsStorage _entitySignalsStorage;
        protected readonly TEntity Entity;

        public EntityContext(IHandlersResolver resolver, EntitySignalsStorage entitySignalsStorage,
            TEntity entity = default) : base(resolver) {
            _entitySignalsStorage = entitySignalsStorage;
            Entity = entity;
        }

        protected override List<HandlerDelegate> GetContextDelegates() {
            return _entitySignalsStorage.GetDelegates(Entity);
        }

        public override void Send<TSignal>(TSignal arg) {
            ExecuteSend(Entity, arg);
        }

        public override void Dispose() {
            _entitySignalsStorage.Dispose(Entity);
        }
    }
}