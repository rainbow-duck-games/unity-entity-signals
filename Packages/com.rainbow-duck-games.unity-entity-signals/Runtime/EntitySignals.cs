using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

#pragma warning disable 618

namespace EntitySignals {
    public static class ES {
        private static readonly EntitySignals Default = new EntitySignals();

        public static EntitySignals.Context<object> Global => Default.Global;

        public static EntitySignals.Context<TEntity> On<TEntity>(TEntity entity) {
            return Default.On(entity);
        }

        public static void Dispose() {
            Default.Dispose();
        }
    }

    public class EntitySignalsCache {
        private readonly ConditionalWeakTable<Type, List<HandlerMeta>> _cache =
            new ConditionalWeakTable<Type, List<HandlerMeta>>();

        public List<HandlerMeta> GetValue(Type type) {
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
            var meta = new HandlerMeta(1, candidate);
            meta.RequiredType = attr.EntityType;
            meta.SignalType = attr.SignalType ?? candidate.GetParameters()[0].ParameterType;
            return meta;
        }

        private static HandlerMeta TwoArgDelegate(SignalHandlerAttribute attr, MethodInfo candidate) {
            var meta = new HandlerMeta(2, candidate);
            meta.RequiredType = attr.EntityType ?? candidate.GetParameters()[0].ParameterType;
            meta.SignalType = attr.SignalType ?? candidate.GetParameters()[1].ParameterType;
            return meta;
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

    public class EntitySignals {
        // TODo Matcher
        private static readonly EntitySignalsCache Cache = new EntitySignalsCache();
        private readonly List<Delegate> _globalDelegates = new List<Delegate>();

        private ConditionalWeakTable<object, List<Delegate>> _delegates =
            new ConditionalWeakTable<object, List<Delegate>>();

        public readonly Context<object> Global;

        public EntitySignals() {
            Global = new NoEntityContext(this);
        }

        // ToDo Refactor to count all delegates over global & local instances
        public int Count => 0;

        public Context<TEntity> On<TEntity>(TEntity entity) {
            if (entity == null)
                throw new Exception("Can't create a Entity Signals context for empty entity");

            return new Context<TEntity>(this, entity);
        }

        public void Dispose() {
            _globalDelegates.Clear();
            _delegates = new ConditionalWeakTable<object, List<Delegate>>();
        }

        private List<Delegate> GetDelegates(object instance) {
            return instance == null
                ? _globalDelegates
                : _delegates.GetValue(instance, k => new List<Delegate>());
        }

        private void Send<TEntity, TSignal>(TEntity entity, TSignal arg) {
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

        public class Context<TEntity> {
            private readonly EntitySignals _entitySignals;
            private readonly TEntity _entity;

            public Context(EntitySignals entitySignals, TEntity entity) {
                _entitySignals = entitySignals;
                _entity = entity;
            }

            public void Add(object receiver) {
                EachReceiveInternal(receiver, @delegate => { _entitySignals.GetDelegates(_entity).Add(@delegate); });
            }

            public void Remove(object receiver) {
                EachReceiveInternal(receiver, @delegate => { _entitySignals.GetDelegates(_entity).Remove(@delegate); });
            }

            private void EachReceiveInternal(object receiver, Action<Delegate> action) {
                var receiverType = receiver.GetType();
                var entityType = typeof(TEntity);
                var meta = Cache.GetValue(receiverType);

                // Process meta
                var count = 0;
                foreach (var candidate in meta) {
                    if (candidate.ParamCount == 1) {
                        if (candidate.RequiredType != null && !candidate.RequiredType.IsAssignableFrom(entityType))
                            continue;

                        var delegateType = typeof(ESHandler<>).MakeGenericType(candidate.SignalType);
                        var @delegate = Delegate.CreateDelegate(delegateType, receiver, candidate.MethodInfo);
                        action.Invoke(@delegate);
                        count++;
                    } else if (candidate.ParamCount == 2) {
                        if (!candidate.RequiredType.IsAssignableFrom(entityType))
                            continue;

                        var delegateType = typeof(ESHandler<,>).MakeGenericType(typeof(TEntity), candidate.SignalType);
                        var @delegate = Delegate.CreateDelegate(delegateType, receiver, candidate.MethodInfo);
                        action.Invoke(@delegate);
                        count++;
                    } else {
                        throw new Exception("ToDo"); // TODo
                    }
                }

                if (count == 0)
                    throw new Exception(
                        $"Can't bind method {receiver.GetType().Name} to entity {typeof(TEntity)}: No methods matched signature");
            }

            public void Add<TSignal>(ESHandler<TSignal> signalHandler) {
                _entitySignals.GetDelegates(_entity).Add(signalHandler);
            }

            public virtual void Add<TSignal>(ESHandler<TEntity, TSignal> signalHandler) {
                _entitySignals.GetDelegates(_entity).Add(signalHandler);
            }

            public void Remove<TSignal>(ESHandler<TEntity> signalHandler) {
                _entitySignals.GetDelegates(_entity).Remove(signalHandler);
            }

            public virtual void Remove<TSignal>(ESHandler<TEntity, TSignal> signalHandler) {
                _entitySignals.GetDelegates(_entity).Remove(signalHandler);
            }

            public void Dispose() {
                _entitySignals.GetDelegates(_entity).Clear();
            }

            public void Send<TSignal>(TSignal arg) {
                _entitySignals.Send(_entity, arg);
            }

            public int Count => _entitySignals.GetDelegates(_entity).Count;
        }

        class NoEntityContext : Context<object> {
            public NoEntityContext(EntitySignals entitySignals) : base(entitySignals, null) { }

            public override void Add<TSignal>(ESHandler<object, TSignal> signalHandler) {
                throw new Exception("Entity specific handler is not allowed for NoEntityRecords");
            }

            public override void Remove<TSignal>(ESHandler<object, TSignal> signalHandler) {
                throw new Exception("Entity specific handler is not allowed for NoEntityRecords");
            }
        }
    }

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