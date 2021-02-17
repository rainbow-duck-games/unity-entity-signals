using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;

namespace EntitySignals.Handlers {
    public class AttributeHandlersResolver : IHandlersResolver {
        public IEnumerable<HandlerMeta> GetHandlers(Type type) {
            var list = new List<HandlerMeta>();
            var all = type.GetMethods();
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
                            $"Can't bind method {type.Name}: Method {candidate.Name} has wrong amount of arguments");
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
    }

    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Method)]
    // ToDo Analyzer to enforce compile-time errors
    public class SignalHandlerAttribute : Attribute {
        public readonly Type EntityType;
        public readonly Type SignalType;

        public SignalHandlerAttribute(Type entityType = null, Type signalType = null) {
            EntityType = entityType;
            SignalType = signalType;
        }
    }
}