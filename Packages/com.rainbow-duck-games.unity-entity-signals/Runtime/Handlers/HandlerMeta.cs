using System;
using System.Reflection;

namespace EntitySignals.Handlers {
    public class HandlerMeta {
        public readonly int ParamCount;
        public readonly MethodInfo MethodInfo;
        public Type RequiredType;
        public Type SignalType;

        public HandlerMeta(int paramCount, MethodInfo methodInfo) {
            ParamCount = paramCount;
            MethodInfo = methodInfo;
        }
    }
}