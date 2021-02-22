using System;
using System.Linq;
using JetBrains.Annotations;
using RainbowDuckGames.UnityEntitySignals.Utility.Tact;

namespace RainbowDuckGames.UnityEntitySignals.Handlers {
    public class InterfaceHandlersResolver : IHandlersResolver {
        public HandlerMeta[] GetHandlers(Type type) {
            return type.GetInterfaces()
                .Where(i => i.IsGenericType)
                .Where(i => i.GetGenericTypeDefinition() == typeof(IReceive<,>))
                .Select(i => {
                    var entityType = i.GetGenericArguments()[0];
                    var signalType = i.GetGenericArguments()[1];
                    var methodInfo = type.GetMethod("HandleSignal", new[] {entityType, signalType});
                    return new HandlerMeta(
                        entityType, signalType,
                        2, type.GetMethodInvoker(methodInfo));
                })
                .ToArray();
        }
    }

    public interface IReceive<in TEntity, in TSignal> {
        [UsedImplicitly]
        void HandleSignal(TEntity entity, TSignal signal);
    }
}