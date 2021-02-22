using System;
using RainbowDuckGames.UnityEntitySignals.Utility.Tact;

namespace RainbowDuckGames.UnityEntitySignals.Handlers {
    public interface IHandlersResolver {
        HandlerMeta[] GetHandlers(Type type);
    }

    public class HandlerMeta {
        public readonly Type RequiredType;
        public readonly Type SignalType;
        public readonly int ParamCount;
        public readonly EfficientInvoker MethodInvoker;

        public HandlerMeta(Type requiredType, Type signalType, int paramCount, EfficientInvoker methodInvoker) {
            RequiredType = requiredType;
            SignalType = signalType;
            ParamCount = paramCount;
            MethodInvoker = methodInvoker;
        }
    }
}