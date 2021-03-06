﻿using System.Linq;
using RainbowDuckGames.UnityEntitySignals.Handlers;
using RainbowDuckGames.UnityEntitySignals.Storages;

namespace RainbowDuckGames.UnityEntitySignals.Contexts {
    public class DynamicEntityContext<TEntity> : EntityContext<TEntity, DynamicStorage> {
        public DynamicEntityContext(IHandlersResolver resolver, DynamicStorage dynamicStorage,
            TEntity entity = default) : base(resolver, dynamicStorage, entity) {
        }

        public override void Send<TSignal>(TSignal arg) {
            // Copy current list to avoid concurrent modifications
            ExecuteSend(Entity, arg, GetContextDelegates().ToList());
            ExecuteSend(Entity, arg, Storage.GetDelegates<TEntity>().ToList());
        }
    }
}