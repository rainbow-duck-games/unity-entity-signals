using RainbowDuckGames.UnityEntitySignals.Handlers;
using RainbowDuckGames.UnityEntitySignals.Storages;

#pragma warning disable 618

namespace RainbowDuckGames.UnityEntitySignals {
    public class EntitySignals {
        private readonly GlobalStorage _global;
        private readonly DynamicStorage _entity;

        public EntitySignals(IHandlersResolver resolver = null) {
            var handlersResolver = resolver ?? new CachedHandlersResolver(new AttributeHandlersResolver());
            _global = new GlobalStorage(handlersResolver);
            _entity = new DynamicStorage(handlersResolver);
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