using System.Collections.Generic;
using RainbowDuckGames.UnityEntitySignals.Contexts;
using RainbowDuckGames.UnityEntitySignals.Handlers;

namespace RainbowDuckGames.UnityEntitySignals.Storages {
    public class GlobalStorage {
        private List<HandlerDelegate> _delegates = new List<HandlerDelegate>();
        private readonly GlobalContext _global;

        public GlobalStorage(IHandlersResolver resolver) {
            _global = new GlobalContext(resolver, this);
        }

        public int Count => _delegates.Count;

        public IContext<object> On() {
            return _global;
        }

        public void Dispose() {
            _delegates = new List<HandlerDelegate>();
        }

        internal List<HandlerDelegate> GetDelegates() => _delegates;
    }
}