using System;
using EntitySignals.Handlers;
using Homebrew;
using JetBrains.Annotations;
using NUnit.Framework;
using Unity.PerformanceTesting;

#pragma warning disable 618

namespace EntitySignals.Tests.Editor {
    [UsedImplicitly]
    public class EntitySignalsPerf {
        private const int Warmup = 10;
        private const int Measurements = 100;
        private const int Iterations = 5;

        [UsedImplicitly]
        public class SendOnly : AbstractPerfTest {
            protected override Action PrepareSignals(Signals signals, Func<Recorder> receiverPrepare) {
                var receiver = receiverPrepare.Invoke();
                signals.Add(receiver);
                return () => { signals.Send(1); };
            }

            protected override Action Prepare(EntitySignals es, Func<Recorder> receiverPrepare) {
                var receiver = receiverPrepare.Invoke();
                es.On(Entity).Add(receiver);
                return () => { es.On(Entity).Send(1); };
            }
        }

        [UsedImplicitly]
        public class SubscribeOnly : AbstractPerfTest {
            protected override Action PrepareSignals(Signals signals, Func<Recorder> receiverPrepare) {
                return () => {
                    var receiver = receiverPrepare.Invoke();
                    signals.Add(receiver);
                };
            }

            protected override Action Prepare(EntitySignals es, Func<Recorder> receiverCreate) {
                return () => {
                    var receiver = receiverCreate.Invoke();
                    es.On(Entity).Add(receiver);
                };
            }
        }

        [UsedImplicitly]
        public class SubscribeSendUnsubscribe : AbstractPerfTest {
            protected override Action PrepareSignals(Signals signals, Func<Recorder> receiverPrepare) {
                return () => {
                    var receiver = new SignalsTestReceiver();
                    signals.Add(receiver);
                    signals.Send(1);
                    signals.Remove(receiver);
                };
            }

            protected override Action Prepare(EntitySignals es, Func<Recorder> receiverPrepare) {
                return () => {
                    var receiver = receiverPrepare.Invoke();
                    es.On(Entity).Add(receiver);
                    es.On(Entity).Send(1);
                    es.On(Entity).Remove(receiver);
                };
            }
        }

        public abstract class AbstractPerfTest {
            protected readonly TestEntity Entity = new TestEntity();

            protected abstract Action PrepareSignals(Signals signals, Func<Recorder> receiverPrepare);
            protected abstract Action Prepare(EntitySignals es, Func<Recorder> receiverPrepare);

            private static void RunMeasure(Action action) {
                Measure.Method(action)
                    .WarmupCount(Warmup)
                    .MeasurementCount(Measurements)
                    .IterationsPerMeasurement(Iterations)
                    .GC()
                    .Run();
            }

            [Test, Performance]
            public void AttributesResolver() {
                RunMeasure(Prepare(
                    new EntitySignals(new AttributeHandlersResolver()),
                    () => new AttributeTestReceiver()
                ));
            }

            [Test, Performance]
            public void AttributesCachedResolver() {
                RunMeasure(Prepare(
                    new EntitySignals(new CachedHandlersResolver(new AttributeHandlersResolver())),
                    () => new AttributeCacheTestReceiver()
                ));
            }

            [Test, Performance]
            public void InterfaceResolver() {
                RunMeasure(Prepare(
                    new EntitySignals(new InterfaceHandlersResolver()),
                    () => new InterfaceTestReceiver()
                ));
            }

            [Test, Performance]
            public void InterfaceCachedResolver() {
                RunMeasure(Prepare(
                    new EntitySignals(new CachedHandlersResolver(new InterfaceHandlersResolver())),
                    () => new InterfaceCacheTestReceiver()
                ));
            }

            [Test, Performance]
            public void PixeyeSignals() {
                RunMeasure(PrepareSignals(
                    new Signals(),
                    () => new SignalsTestReceiver()
                ));
            }
        }

        public class TestEntity {
        }

        private class AttributeTestReceiver : Recorder {
            [SignalHandler]
            public void OnGameObjectFirstSignal(TestEntity obj, int x) {
                Record("First", obj, x);
            }
        }

        private class AttributeCacheTestReceiver : Recorder {
            [SignalHandler]
            public void OnGameObjectFirstSignal(TestEntity obj, int x) {
                Record("First", obj, x);
            }
        }

        private class InterfaceTestReceiver : Recorder, IReceive<TestEntity, int> {
            public void HandleSignal(TestEntity entity, int signal) {
                Record("First", entity, signal);
            }
        }

        private class InterfaceCacheTestReceiver : Recorder, IReceive<TestEntity, int> {
            public void HandleSignal(TestEntity entity, int signal) {
                Record("First", entity, signal);
            }
        }

        private class SignalsTestReceiver : Recorder, IReceive<int> {
            public void HandleSignal(int arg) {
                Record("First", arg);
            }
        }
    }
}