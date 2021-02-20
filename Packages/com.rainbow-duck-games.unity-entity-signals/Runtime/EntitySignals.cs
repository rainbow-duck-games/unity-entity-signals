using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using EntitySignals.Context;
using EntitySignals.Handlers;
using EntitySignals.Utility.Tact;

#pragma warning disable 618

namespace EntitySignals {
    public static class ES {
        private static readonly EntitySignals Default = new EntitySignals();

        public static IContext<object> Global => Default.Global;

        public static IContext<TEntity> On<TEntity>(TEntity entity) {
            return Default.On(entity);
        }

        public static void Dispose() {
            Default.Dispose();
        }
    }

    public class EntitySignals {
        private readonly IHandlersResolver _resolver;
        private readonly List<ReceiverDelegate> _globalDelegates = new List<ReceiverDelegate>();

        private ConditionalWeakTable<object, List<ReceiverDelegate>> _delegates =
            new ConditionalWeakTable<object, List<ReceiverDelegate>>();

        // TODo Matcher
        private ConditionalWeakTable<Type, DynamicDelegate> _dynamics =
            new ConditionalWeakTable<Type, DynamicDelegate>();

        public readonly IContext<object> Global;

        public EntitySignals(IHandlersResolver resolver = null) {
            _resolver = resolver ?? new CachedHandlersResolver(new AttributeHandlersResolver());
            Global = new NullContext(_resolver, this);
        }

        // ToDo Refactor to count all delegates over global & local instances
        public int Count => 0;

        public IContext<TEntity> On<TEntity>(TEntity entity) {
            if (entity == null)
                throw new Exception("Can't create a Entity Signals context for empty entity");

            return new EntityContext<TEntity>(_resolver, this, entity);
        }

        public DynamicContext<TEntity> On<TEntity>(Predicate<TEntity> predicate) {
            return new DynamicContext<TEntity>(_resolver, this, predicate);
        }

        public void Dispose() {
            _globalDelegates.Clear();
            _delegates = new ConditionalWeakTable<object, List<ReceiverDelegate>>();
        }

        internal List<ReceiverDelegate> GetDelegates(object instance) {
            // ToDo Dynamics
            return instance == null
                ? _globalDelegates
                : _delegates.GetValue(instance, k => new List<ReceiverDelegate>());
        }

        internal void Dispose(object instance) {
            // ToDo Handle dynamic 
            GetDelegates(instance).Clear();
        }

        internal void Send<TEntity, TSignal>(TEntity entity, TSignal arg) {
            // if (Delegates.Count == 0)
            //     throw new Exception(""); // ToDo Strict mode?

            GetDelegates(entity).ForEach(rd => {
                if (!rd.Valid(arg))
                    return;
                
                switch (rd.Args) {
                    case 1:
                        rd.Invoker.Invoke(rd.Receiver, arg);
                        break;
                    case 2:
                        rd.Invoker.Invoke(rd.Receiver, entity, arg);
                        break;
                }
            });
        }
    }

    public delegate void ESHandler<in TSignal>(TSignal signal);

    public delegate void ESHandler<in TEntity, in TSignal>(TEntity entity, TSignal signal);

    public class ReceiverDelegate {
        public readonly Type SignalType;
        public readonly EfficientInvoker Invoker;
        public readonly object Receiver;
        public readonly int Args;

        public ReceiverDelegate(Type signalType, EfficientInvoker invoker, object receiver, int args = 2) {
            SignalType = signalType;
            Invoker = invoker;
            Receiver = receiver;
            Args = args;
        }

        public bool Valid(object signal) {
            return SignalType == signal.GetType();
        }
    }

    public class DynamicDelegate {
        public readonly ReceiverDelegate ReceiverDelegate;
        public readonly Predicate<object> Predicate;

        public DynamicDelegate(ReceiverDelegate receiverDelegate, Predicate<object> predicate) {
            ReceiverDelegate = receiverDelegate;
            Predicate = predicate;
        }
    }
}