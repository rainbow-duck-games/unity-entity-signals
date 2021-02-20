using System;
using EntitySignals.Context;
using EntitySignals.Handlers;
using EntitySignals.Storage;

#pragma warning disable 618

namespace EntitySignals {
    public class EntitySignals {
        private readonly IStorage _storage;

        public EntitySignals(IHandlersResolver resolver = null, IStorage storage = null) {
            _storage = storage ??
                       new EntityStorage(resolver ?? new CachedHandlersResolver(new AttributeHandlersResolver()));
        }

        public int Count => _storage.Count;

        public IContext<object> On() {
            return _storage.On();
        }

        public IContext<TEntity> On<TEntity>(TEntity entity) {
            return _storage.On(entity);
        }

        public IContext<TEntity> On<TEntity>(Predicate<TEntity> predicate) {
            return _storage.On(predicate);
        }

        public void Dispose() {
            _storage.Dispose();
        }

        internal void Dispose(object instance) {
            _storage.Dispose(instance);
        }
    }
}