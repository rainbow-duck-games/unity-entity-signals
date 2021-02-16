using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace EntitySignals {
    public class HandlersCache {
        private readonly ConditionalWeakTable<Type, List<HandlerMeta>> _cache =
            new ConditionalWeakTable<Type, List<HandlerMeta>>();

        public IEnumerable<HandlerMeta> GetValue(Type type) {
            return _cache.GetValue(type, PrepareReceiverMeta);
        }

        private static List<HandlerMeta> PrepareReceiverMeta(Type receiverType) {
            var list = new List<HandlerMeta>();
            var all = receiverType.GetMethods();
            foreach (var candidate in all) {
                var attr = candidate.GetCustomAttribute<SignalHandlerAttribute>();
                if (attr == null)
                    continue;

                switch (candidate.GetParameters().Length) {
                    case 1:
                        list.Add(SingleArgDelegate(attr, candidate));
                        break;
                    case 2:
                        list.Add(TwoArgDelegate(attr, candidate));
                        break;
                    default:
                        throw new Exception(
                            $"Can't bind method {receiverType.Name}: Method {candidate.Name} has wrong amount of arguments");
                }
            }

            return list;
        }

        private static HandlerMeta SingleArgDelegate(SignalHandlerAttribute attr, MethodInfo candidate) {
            return new HandlerMeta(1, candidate) {
                RequiredType = attr.EntityType,
                SignalType = attr.SignalType ?? candidate.GetParameters()[0].ParameterType
            };
        }

        private static HandlerMeta TwoArgDelegate(SignalHandlerAttribute attr, MethodInfo candidate) {
            return new HandlerMeta(2, candidate) {
                RequiredType = attr.EntityType ?? candidate.GetParameters()[0].ParameterType,
                SignalType = attr.SignalType ?? candidate.GetParameters()[1].ParameterType
            };
        }
        
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
}