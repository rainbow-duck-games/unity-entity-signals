using System.Collections.Generic;
using EntitySignals.Handlers;

namespace EntitySignals.Contexts {
    public class EntityContext<TEntity, TSignals> : AbstractContext<TEntity> where TSignals : Storages.EntitySignals {
        protected readonly TSignals Storage;
        protected readonly TEntity Entity;

        public EntityContext(IHandlersResolver resolver, TSignals storage,
            TEntity entity = default) : base(resolver) {
            Storage = storage;
            Entity = entity;
        }

        protected override List<HandlerDelegate> GetContextDelegates() {
            return Storage.GetDelegates(Entity);
        }

        public override void Send<TSignal>(TSignal arg) {
            ExecuteSend(Entity, arg, GetContextDelegates());
        }

        public override void Dispose() {
            Storage.Dispose(Entity);
        }
    }
}