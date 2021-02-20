using System;
using EntitySignals.Handlers;
using EntitySignals.Storage;

#pragma warning disable 618

namespace EntitySignals {
    public class EntitySignals {
        private readonly GlobalSignalsStorage _global;
        private readonly EntitySignalsStorage _entity;
        // ToDo Dynamic?

        public EntitySignals(IHandlersResolver resolver = null) {
            var handlersResolver = resolver ?? new CachedHandlersResolver(new AttributeHandlersResolver());
            _global = new GlobalSignalsStorage(handlersResolver);
            _entity = new EntitySignalsStorage(handlersResolver);
        }

        public int Count => _global.Count + _entity.Count;

        public IContext<object> On() {
            return _global.On();
        }

        public IContext<TEntity> On<TEntity>(TEntity entity) {
            return _entity.On(entity);
        }

        public IContext<TEntity> On<TEntity>(Predicate<TEntity> predicate) {
            throw new NotImplementedException();
        }

        public void Dispose() {
            _global.Dispose();
            _entity.Dispose();
        }

        public void Dispose<TEntity>(TEntity instance) {
            _entity.Dispose(instance);
        }
        
        public void Dispose<TEntity>(Predicate<TEntity> predicate) {
            throw new NotImplementedException();
        }
    }
}