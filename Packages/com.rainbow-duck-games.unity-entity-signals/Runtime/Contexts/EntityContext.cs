using System.Collections.Generic;
using System.Linq;
using EntitySignals.Handlers;
using EntitySignals.Storages;

namespace EntitySignals.Contexts {
    public class EntityContext<TEntity, TSignals> : AbstractContext<TEntity> where TSignals : EntityStorage {
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
            // Copy current list to avoid concurrent modifications
            ExecuteSend(Entity, arg, GetContextDelegates().ToList());
        }

        public override void Dispose() {
            Storage.Dispose(Entity);
        }
    }
}