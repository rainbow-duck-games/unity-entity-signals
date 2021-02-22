using System.Collections.Generic;
using RainbowDuckGames.UnityEntitySignals.Contexts;
using RainbowDuckGames.UnityEntitySignals.Handlers;

namespace RainbowDuckGames.UnityEntitySignals.Storages {
    public class DynamicStorage : EntityStorage {
        public DynamicStorage(IHandlersResolver resolver) : base(resolver) {
        }

        public override IContext<TEntity> On<TEntity>(TEntity entity) {
            return new DynamicEntityContext<TEntity>(Resolver, this, entity);
        }

        public IContext<TEntity> On<TEntity>() {
            return new DynamicTypeContext<TEntity>(Resolver, this);
        }

        internal List<HandlerDelegate> GetDelegates<TEntity>() {
            return Delegates.GetValue(typeof(TEntity), key => new List<HandlerDelegate>());
        }

        public void Dispose<TEntity>() {
            GetDelegates(typeof(TEntity)).Clear();
        }
    }
}