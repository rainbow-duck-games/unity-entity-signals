using EntitySignals.Handlers;
using EntitySignals.Storages;

#pragma warning disable 618

namespace EntitySignals {
    public class EntitySignals {
        private readonly GlobalSignals _global;
        private readonly DynamicSignals _entity;

        public EntitySignals(IHandlersResolver resolver = null) {
            var handlersResolver = resolver ?? new CachedHandlersResolver(new AttributeHandlersResolver());
            _global = new GlobalSignals(handlersResolver);
            _entity = new DynamicSignals(handlersResolver);
        }

        public int Count => _global.Count + _entity.Count;

        public IContext<object> On() {
            return _global.On();
        }

        public IContext<TEntity> On<TEntity>(TEntity entity) {
            return _entity.On(entity);
        }

        public IContext<TEntity> On<TEntity>() {
            return _entity.On<TEntity>();
        }

        public void Dispose() {
            _global.Dispose();
            _entity.Dispose();
        }

        public void Dispose<TEntity>(TEntity instance) {
            _entity.Dispose(instance);
        }
        
        public void Dispose<TEntity>() {
            _entity.Dispose<TEntity>();
        }
    }
}