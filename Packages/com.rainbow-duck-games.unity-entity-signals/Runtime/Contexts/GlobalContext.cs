using System;
using System.Collections.Generic;
using System.Linq;
using RainbowDuckGames.UnityEntitySignals.Handlers;
using RainbowDuckGames.UnityEntitySignals.Storages;

namespace RainbowDuckGames.UnityEntitySignals.Contexts {
    public sealed class GlobalContext : AbstractContext<object> {
        private readonly GlobalStorage _storage;

        public GlobalContext(IHandlersResolver resolver, GlobalStorage storage) : base(resolver) {
            _storage = storage;
        }

        public override void Add(object receiver) {
            var receiverType = receiver.GetType();
            var handlers = Resolver.GetHandlers(receiverType)
                .Where(meta => meta.RequiredType == null)
                .Select(meta =>
                    new HandlerDelegate(meta.SignalType, meta.MethodInvoker, receiver, meta.ParamCount))
                .ToArray();
            if (handlers.Length == 0)
                throw new Exception(
                    $"Can't bind method {receiver.GetType().Name} to global: No methods matched signature");
            
            GetContextDelegates().AddRange(handlers);
        }

        public override void Dispose() {
            GetContextDelegates().Clear();
        }

        public override void Send<TSignal>(TSignal arg) {
            // Copy current list to avoid concurrent modifications
            ExecuteSend(null, arg, GetContextDelegates().ToList());
        }

        protected override List<HandlerDelegate> GetContextDelegates() {
            return _storage.GetDelegates();
        }
    }
}