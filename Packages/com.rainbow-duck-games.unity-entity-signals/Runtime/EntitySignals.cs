using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using EntitySignals.Context;
using EntitySignals.Handlers;

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
        // TODo Matcher
        private static IHandlersResolver _resolver;
        private readonly List<Delegate> _globalDelegates = new List<Delegate>();

        private ConditionalWeakTable<object, List<Delegate>> _delegates =
            new ConditionalWeakTable<object, List<Delegate>>();

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
            _delegates = new ConditionalWeakTable<object, List<Delegate>>();
        }

        internal List<Delegate> GetDelegates(object instance) {
            // ToDo Dynamics
            return instance == null
                ? _globalDelegates
                : _delegates.GetValue(instance, k => new List<Delegate>());
        }

        internal void Dispose(object instance) {
            // ToDo Handle dynamic 
            GetDelegates(instance).Clear();
        }

        internal void Send<TEntity, TSignal>(TEntity entity, TSignal arg) {
            // if (Delegates.Count == 0)
            //     throw new Exception(""); // ToDo Strict mode?

            GetDelegates(entity).ForEach(receiver => {
                (receiver as ESHandler<TSignal>)?
                    .Invoke(arg);
                if (entity != null)
                    (receiver as ESHandler<TEntity, TSignal>)?
                        .Invoke(entity, arg);
            });
        }
    }
    
    public delegate void ESHandler<in TSignal>(TSignal signal);

    public delegate void ESHandler<in TEntity, in TSignal>(TEntity entity, TSignal signal);

    public class DynamicDelegate {
        public readonly Delegate Delegate;
        public readonly Predicate<object> Predicate;

        public DynamicDelegate(Delegate @delegate, Predicate<object> predicate) {
            Delegate = @delegate;
            Predicate = predicate;
        }
    }
}