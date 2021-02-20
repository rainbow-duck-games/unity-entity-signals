using System;
using System.Reflection;
using EntitySignals.Utility.Tact;

namespace EntitySignals.Handlers {
    public class HandlerMeta {
        public readonly int ParamCount;
        public readonly EfficientInvoker MethodInvoker;
        public Type RequiredType;
        public Type SignalType;

        public HandlerMeta(int paramCount, EfficientInvoker invoker) {
            ParamCount = paramCount;
            MethodInvoker = invoker;
        }
    }
}