using System.Collections.Generic;
using System.Linq;
using EntitySignals.Contexts;
using EntitySignals.Handlers;

namespace EntitySignals.Storages {
    public class DynamicSignals : EntitySignals {
        public DynamicSignals(IHandlersResolver resolver) : base(resolver) {
        }

        public override IContext<TEntity> On<TEntity>(TEntity entity) {
            return new DynamicEntityContext<TEntity>(Resolver, this, entity);
        }

        public IContext<TEntity> On<TEntity>() {
            return new DynamicTypeContext<TEntity>(Resolver, this);
        }

        internal List<HandlerDelegate> GetRelatedDelegates<TEntity>(TEntity entity) {
            return GetDelegates(entity)
                .Concat(GetDelegates<TEntity>())
                .ToList();
        }

        internal List<HandlerDelegate> GetDelegates<TEntity>() {
            return Delegates.GetValue(typeof(TEntity), key => new List<HandlerDelegate>());
        }

        public void Dispose<TEntity>() {
            GetDelegates(typeof(TEntity)).Clear();
        }
    }
}