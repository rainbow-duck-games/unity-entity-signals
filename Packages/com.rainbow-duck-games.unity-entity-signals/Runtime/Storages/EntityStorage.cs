using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RainbowDuckGames.UnityEntitySignals.Contexts;
using RainbowDuckGames.UnityEntitySignals.Handlers;

namespace RainbowDuckGames.UnityEntitySignals.Storages {
    public class EntityStorage {
        protected readonly IHandlersResolver Resolver;

        protected ConditionalWeakTable<object, List<HandlerDelegate>> Delegates =
            new ConditionalWeakTable<object, List<HandlerDelegate>>();
 
        public EntityStorage(IHandlersResolver resolver) {
            Resolver = resolver;
        }

        public int Count => throw new NotImplementedException("Rotate over delegates?");

        public virtual IContext<TEntity> On<TEntity>(TEntity entity) {
            if (entity == null)
                throw new Exception("Can't create a Entity Signals context for empty entity");

            return new EntityContext<TEntity, EntityStorage>(Resolver, this, entity);
        }

        public virtual void Dispose() {
            Delegates = new ConditionalWeakTable<object, List<HandlerDelegate>>();
        }

        public void Dispose<TEntity>(TEntity entity) {
            GetDelegates(entity).Clear();
        }

        internal List<HandlerDelegate> GetDelegates<TEntity>(TEntity entity) {
            return Delegates.GetValue(entity, k => new List<HandlerDelegate>());
        }
    }
}