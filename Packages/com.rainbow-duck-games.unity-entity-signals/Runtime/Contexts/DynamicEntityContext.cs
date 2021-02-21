using EntitySignals.Handlers;
using EntitySignals.Storages;

namespace EntitySignals.Contexts {
    public class DynamicEntityContext<TEntity> : EntityContext<TEntity, DynamicStorage> {
        public DynamicEntityContext(IHandlersResolver resolver, DynamicStorage dynamicStorage,
            TEntity entity = default) : base(resolver, dynamicStorage, entity) {
        }

        public override void Send<TSignal>(TSignal arg) {
            ExecuteSend(Entity, arg, Storage.GetRelatedDelegates(Entity));
        }
    }
}