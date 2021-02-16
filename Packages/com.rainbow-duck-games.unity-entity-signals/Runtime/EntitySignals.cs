using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using EntitySignals.Context;
using JetBrains.Annotations;

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
        private static HandlersCache _cache;
        private readonly List<Delegate> _globalDelegates = new List<Delegate>();

        private ConditionalWeakTable<object, List<Delegate>> _delegates =
            new ConditionalWeakTable<object, List<Delegate>>();

        public readonly IContext<object> Global;

        public EntitySignals(HandlersCache cache = null) {
            _cache = cache ?? new HandlersCache();
            Global = new NullContext(_cache, this);
        }

        // ToDo Refactor to count all delegates over global & local instances
        public int Count => 0;

        public IContext<TEntity> On<TEntity>(TEntity entity) {
            if (entity == null)
                throw new Exception("Can't create a Entity Signals context for empty entity");

            return new EntityContext<TEntity>(_cache, this, entity);
        }

        public void Dispose() {
            _globalDelegates.Clear();
            _delegates = new ConditionalWeakTable<object, List<Delegate>>();
        }

        internal List<Delegate> GetDelegates(object instance) {
            return instance == null
                ? _globalDelegates
                : _delegates.GetValue(instance, k => new List<Delegate>());
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
}