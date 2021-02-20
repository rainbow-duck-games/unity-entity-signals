using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using EntitySignals.Handlers;

namespace EntitySignals.Storage {
    public class DynamicSignalsStorage : EntitySignalsStorage {
        private ConditionalWeakTable<Type, List<HandlerDelegate>> _delegates =
            new ConditionalWeakTable<Type, List<HandlerDelegate>>();

        public DynamicSignalsStorage(IHandlersResolver resolver) : base(resolver) {
        }

        public IContext<TEntity> On<TEntity>() {
            return new DynamicTypeContext<TEntity>(Resolver, this);
        }

        public override void Dispose() {
            base.Dispose();
            _delegates = new ConditionalWeakTable<Type, List<HandlerDelegate>>();
        }

        internal List<HandlerDelegate> GetDelegates<TEntity>(TEntity entity) {
            // ToDO Add dynamics
            return GetDelegates(entity);
        }

        internal List<HandlerDelegate> GetDelegates<TEntity>() {
            return _delegates.GetValue(typeof(TEntity), key => new List<HandlerDelegate>());
        }
    }
}