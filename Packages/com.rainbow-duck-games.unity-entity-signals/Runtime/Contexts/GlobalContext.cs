using System;
using System.Collections.Generic;
using System.Linq;
using EntitySignals.Handlers;
using EntitySignals.Storage;

namespace EntitySignals.Contexts {
    public sealed class GlobalContext : AbstractContext<object> {
        private readonly GlobalSignalsStorage _storage;

        public GlobalContext(IHandlersResolver resolver, GlobalSignalsStorage storage) : base(resolver) {
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

            _storage.GetDelegates().AddRange(handlers);
        }

        public override void Dispose() {
            _storage.Dispose();
        }

        public override void Send<TSignal>(TSignal arg) {
            ExecuteSend(null, arg);
        }

        protected override List<HandlerDelegate> GetContextDelegates() {
            return _storage.GetDelegates();
        }
    }
}