using System;
using EntitySignals.Handlers;
using Homebrew;
using NUnit.Framework;
using Unity.PerformanceTesting;

#pragma warning disable 618

namespace EntitySignals.Tests.Editor.Performance {
    public abstract class AbstractPerfTest {
        private const int Warmup = 10;
        private const int Measurements = 10;
        private const int Iterations = 5000;
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

        protected class TestEntity {
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

        protected class SignalsTestReceiver : Recorder, IReceive<int> {
            public void HandleSignal(int arg) {
                Record("First", arg);
            }
        }
    }
}