using System;
using System.Collections.Generic;

namespace EntitySignals.Handlers {
    public interface IHandlersResolver {
        IEnumerable<HandlerMeta> GetHandlers(Type type);
    }
}