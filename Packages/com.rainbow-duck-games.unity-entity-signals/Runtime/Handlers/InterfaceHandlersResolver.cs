using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace EntitySignals.Handlers {
    public class InterfaceHandlersResolver : IHandlersResolver {
        public IEnumerable<HandlerMeta> GetHandlers(Type type) {
            return type.GetInterfaces()
                .Where(i => i.IsGenericType)
                .Where(i => i.GetGenericTypeDefinition() == typeof(IReceive<,>))
                .Select(i => {
                    var entityType = i.GetGenericArguments()[0];
                    var signalType = i.GetGenericArguments()[1];
                    return new HandlerMeta(2, type.GetMethod("HandleSignal", new[] {entityType, signalType})) {
                        RequiredType = entityType,
                        SignalType = signalType
                    };
                });
        }
    }

    public interface IReceive<in TEntity, in TSignal> : IReceiveInternal {
        [UsedImplicitly]
        void HandleSignal(TEntity entity, TSignal signal);
    }

    public interface IReceiveInternal {
    }
}