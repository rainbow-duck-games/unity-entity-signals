using System;
using EntitySignals.Handlers;

namespace EntitySignals.Context {
    public class DynamicContext<TEntity> : EntityContext<TEntity> {
        private readonly Predicate<TEntity> _predicate;

        public DynamicContext(IHandlersResolver resolver, EntitySignals entitySignals, Predicate<TEntity> predicate)
            : base(resolver, entitySignals ) {
            _predicate = predicate;
        }
        
        // ToDo Dynamic handling?!
    }
}