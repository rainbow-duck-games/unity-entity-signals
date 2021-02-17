using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace EntitySignals.Handlers {
    public class CachedHandlersResolver : IHandlersResolver {
        private readonly IHandlersResolver _resolver;

        private readonly ConditionalWeakTable<Type, IEnumerable<HandlerMeta>> _cache =
            new ConditionalWeakTable<Type, IEnumerable<HandlerMeta>>();

        public CachedHandlersResolver(IHandlersResolver resolver) {
            _resolver = resolver;
        }

        public IEnumerable<HandlerMeta> GetHandlers(Type type) {
            return _cache.GetValue(type, t => _resolver.GetHandlers(t));
        }
    }
}