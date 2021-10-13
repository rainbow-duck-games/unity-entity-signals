using System;
using System.Threading;
using Homebrew;
using NUnit.Framework;
using RainbowDuckGames.UnityEntitySignals.Handlers;
using Unity.PerformanceTesting;

#pragma warning disable 618

namespace RainbowDuckGames.UnityEntitySignals.Tests.Editor.Performance {
    public abstract class AbstractPerfTest {
        private const int Warmup = 10;
        private const int Measurements = 10;
        private const int Iterations = 5000;
        protected readonly TestEntity Entity = new TestEntity();

        protected abstract Action PrepareSignals(Signals signals, Recorder receiver);
        protected abstract Action Prepare(EntitySignals es, Recorder receiver);

        protected virtual void Verify(EntitySignals entitySignals, Recorder recorder, int iterations) {
            recorder.Verify("Value", iterations);
        }
        
        protected virtual void Verify(Signals signals, Recorder recorder, int iterations) {
            recorder.Verify("Value", iterations);
        }

        private void RunMeasure(EntitySignals es, Recorder recorder) {
            Measure.Method(Prepare(es, recorder))
                .WarmupCount(Warmup)
                .MeasurementCount(Measurements)
                .IterationsPerMeasurement(Iterations)
                .GC()
                .Run();
            Verify(es, recorder, Iterations * (Measurements + Warmup));
        }

        private void RunMeasure(Signals signals, Recorder recorder) {
            Measure.Method(PrepareSignals(signals, recorder))
                .WarmupCount(Warmup)
                .MeasurementCount(Measurements)
                .IterationsPerMeasurement(Iterations)
                .GC()
                .Run();
            Verify(signals, recorder, Iterations * (Measurements + Warmup));
        }

        [Test, Performance]
        public void AttributesResolver() {
            RunMeasure(new EntitySignals(new AttributeHandlersResolver()), new AttributeTestReceiver());
        }

        [Test, Performance]
        public void AttributesCachedResolver() {
            RunMeasure(new EntitySignals(new CachedHandlersResolver(new AttributeHandlersResolver())),
                new AttributeCacheTestReceiver());
        }

        [Test, Performance]
        public void InterfaceResolver() {
            RunMeasure(new EntitySignals(new InterfaceHandlersResolver()), new InterfaceTestReceiver());
        }

        [Test, Performance]
        public void InterfaceCachedResolver() {
            RunMeasure(new EntitySignals(new CachedHandlersResolver(new InterfaceHandlersResolver())),
                new InterfaceCacheTestReceiver());
        }

        [Test, Performance]
        public void PixeyeSignals() {
            RunMeasure(new Signals(), new SignalsTestReceiver());
        }

        protected class TestEntity {
        }

        private class AttributeTestReceiver : BaseTestReceiver {
            [SignalHandler]
            public void OnGameObjectFirstSignal(TestEntity entity, int x) {
                Increment();
            }
        }

        private class AttributeCacheTestReceiver : BaseTestReceiver {
            [SignalHandler]
            public void OnGameObjectFirstSignal(TestEntity entity, int x) {
                Increment();
            }
        }

        private class InterfaceTestReceiver : BaseTestReceiver, IReceive<TestEntity, int> {
            public void HandleSignal(TestEntity entity, int signal) {
                Increment();
            }
        }

        private class InterfaceCacheTestReceiver : BaseTestReceiver, IReceive<TestEntity, int> {
            public void HandleSignal(TestEntity entity, int signal) {
                Increment();
            }
        }

        private class SignalsTestReceiver : BaseTestReceiver, IReceive<int> {
            public void HandleSignal(int arg) {
                Increment();
            }
        }

        private abstract class BaseTestReceiver : Recorder {
            private long value;

            public void Increment() {
                Record("Value", Interlocked.Increment(ref value));
            }
        }
    }
}