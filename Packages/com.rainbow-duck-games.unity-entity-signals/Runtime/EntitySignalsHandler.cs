using System;
using JetBrains.Annotations;

namespace EntitySignals {
    public delegate void ESHandler<in TSignal>(TSignal signal);

    public delegate void ESHandler<in TEntity, in TSignal>(TEntity entity, TSignal signal);

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