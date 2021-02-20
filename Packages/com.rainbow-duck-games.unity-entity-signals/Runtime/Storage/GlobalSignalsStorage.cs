using System.Collections.Generic;
using EntitySignals.Contexts;
using EntitySignals.Handlers;

namespace EntitySignals.Storage {
    public class GlobalSignalsStorage {
        private List<HandlerDelegate> _delegates = new List<HandlerDelegate>();
        private readonly GlobalContext _global;

        public GlobalSignalsStorage(IHandlersResolver resolver) {
            _global = new GlobalContext(resolver, this);
        }

        public int Count => _delegates.Count;

        public IContext<object> On() {
            return _global;
        }

        public void Dispose() {
            _delegates = new List<HandlerDelegate>();
        }

        public List<HandlerDelegate> GetDelegates() => _delegates;
    }
}