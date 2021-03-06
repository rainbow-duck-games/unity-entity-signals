﻿using System;
using System.Collections.Generic;
using RainbowDuckGames.UnityEntitySignals.Handlers;
using RainbowDuckGames.UnityEntitySignals.Storages;

namespace RainbowDuckGames.UnityEntitySignals.Contexts {
    public class DynamicTypeContext<TEntity> : AbstractContext<TEntity> {
        private readonly DynamicStorage _storage;

        public DynamicTypeContext(IHandlersResolver resolver, DynamicStorage storage) : base(resolver) {
            _storage = storage;
        }

        public override void Send<TSignal>(TSignal arg) {
            throw new NotImplementedException("Dynamic ExecuteSend() is not implemented yet");
        }

        public override void Dispose() {
            _storage.GetDelegates<TEntity>().Clear();
        }

        protected override List<HandlerDelegate> GetContextDelegates() {
            return _storage.GetDelegates<TEntity>();
        }
    }
}