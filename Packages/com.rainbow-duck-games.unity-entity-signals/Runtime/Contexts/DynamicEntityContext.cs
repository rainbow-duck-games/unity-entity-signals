using EntitySignals.Handlers;
using EntitySignals.Storages;

namespace EntitySignals.Contexts {
    public class DynamicEntityContext<TEntity> : EntityContext<TEntity, DynamicSignals> {
        public DynamicEntityContext(IHandlersResolver resolver, DynamicSignals dynamicSignals,
            TEntity entity = default) : base(resolver, dynamicSignals, entity) {
        }

        public override void Send<TSignal>(TSignal arg) {
            ExecuteSend(Entity, arg, Storage.GetRelatedDelegates(Entity));
        }
    }
}