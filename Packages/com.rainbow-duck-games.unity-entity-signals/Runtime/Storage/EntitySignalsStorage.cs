using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using EntitySignals.Contexts;
using EntitySignals.Handlers;

namespace EntitySignals.Storage {
    public class EntitySignalsStorage {
        protected readonly IHandlersResolver Resolver;

        private ConditionalWeakTable<object, List<HandlerDelegate>> _delegates =
            new ConditionalWeakTable<object, List<HandlerDelegate>>();
 
        public EntitySignalsStorage(IHandlersResolver resolver) {
            Resolver = resolver;
        }

        public int Count => 0; // ToDo

        public IContext<TEntity> On<TEntity>(TEntity entity) {
            if (entity == null)
                throw new Exception("Can't create a Entity Signals context for empty entity");

            return new EntityContext<TEntity>(Resolver, this, entity);
        }

        public virtual void Dispose() {
            _delegates = new ConditionalWeakTable<object, List<HandlerDelegate>>();
        }

        public void Dispose<TEntity>(TEntity entity) {
            GetDelegates(entity).Clear();
        }

        internal virtual List<HandlerDelegate> GetDelegates<TEntity>(TEntity entity) {
            return _delegates.GetValue(entity, k => new List<HandlerDelegate>());
        }
    }
}