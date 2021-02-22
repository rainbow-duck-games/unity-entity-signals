using System;
using System.Runtime.CompilerServices;

namespace RainbowDuckGames.UnityEntitySignals.Handlers {
    public class CachedHandlersResolver : IHandlersResolver {
        private readonly IHandlersResolver _resolver;

        private readonly ConditionalWeakTable<Type, HandlerMeta[]> _cache =
            new ConditionalWeakTable<Type, HandlerMeta[]>();

        public CachedHandlersResolver(IHandlersResolver resolver) {
            _resolver = resolver;
        }

        public HandlerMeta[] GetHandlers(Type type) {
            return _cache.GetValue(type, t => _resolver.GetHandlers(t));
        }
    }
}