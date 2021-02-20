using System;
using System.Collections.Generic;
using System.Linq;
using EntitySignals.Context;
using EntitySignals.Handlers;
using EntitySignals.Utility.Tact;

namespace EntitySignals.Storage {
    public class GlobalSignalsStorage {
        private List<ReceiverDelegate> _delegates = new List<ReceiverDelegate>();
        private readonly GlobalContext _global;

        public GlobalSignalsStorage(IHandlersResolver resolver) {
            _global = new GlobalContext(resolver, this);
        }

        public int Count => _delegates.Count;

        public IContext<object> On() {
            return _global;
        }

        public void Dispose() {
            _delegates = new List<ReceiverDelegate>();
        }

        public void Send<TSignal>(TSignal arg) {
            // if (Delegates.Count == 0)
            //     throw new Exception(""); // ToDo Strict mode?

            _delegates
                .ForEach(rd => {
                    if (!rd.Valid(arg))
                        return;

                    switch (rd.Args) {
                        case 1:
                            rd.Invoker.Invoke(rd.Receiver, arg);
                            break;
                        case 2:
                            rd.Invoker.Invoke(rd.Receiver, null, arg);
                            break;
                    }
                });
        }

        private sealed class GlobalContext : IContext<object> {
            private readonly IHandlersResolver _resolver;
            private readonly GlobalSignalsStorage _storage;

            public GlobalContext(IHandlersResolver resolver, GlobalSignalsStorage storage) {
                _resolver = resolver;
                _storage = storage;
            }

            public void Add(object receiver) {
                var receiverType = receiver.GetType();
                var handlers = _resolver.GetHandlers(receiverType)
                    .Where(meta => meta.RequiredType == null)
                    .Select(meta =>
                        new ReceiverDelegate(meta.SignalType, meta.MethodInvoker, receiver, meta.ParamCount))
                    .ToArray();
                if (handlers.Length == 0)
                    throw new Exception(
                        $"Can't bind method {receiver.GetType().Name} to global: No methods matched signature");

                _storage._delegates.AddRange(handlers);
            }

            public void Remove(object receiver) {
                _storage._delegates
                    .RemoveAll(rd => rd.Receiver == receiver);
            }

            public void Add<TSignal>(ESHandler<TSignal> signalHandler) {
                _storage._delegates
                    .Add(new ReceiverDelegate(typeof(TSignal), signalHandler.GetInvoker(), signalHandler, 1));
            }

            public void Add<TSignal>(ESHandler<object, TSignal> signalHandler) {
                _storage._delegates
                    .Add(new ReceiverDelegate(typeof(TSignal), signalHandler.GetInvoker(), signalHandler));
            }

            public void Remove<TSignal>(ESHandler<TSignal> signalHandler) {
                _storage._delegates
                    .RemoveAll(c => ReferenceEquals(c.Receiver, signalHandler));
            }

            public void Remove<TSignal>(ESHandler<object, TSignal> signalHandler) {
                _storage._delegates
                    .RemoveAll(c => ReferenceEquals(c.Receiver, signalHandler));
            }

            public void Dispose() {
                _storage.Dispose();
            }

            public void Send<TSignal>(TSignal arg) {
                _storage.Send(arg);
            }

            public int Count => _storage.Count;
        }
    }
}