using System;
using EntitySignals.Handlers;
using EntitySignals.Storages;
using EntitySignals.Utility.Tact;
using Homebrew;
using JetBrains.Annotations;
using NUnit.Framework;
using Unity.PerformanceTesting;

namespace EntitySignals.Tests.Editor.Performance {
    public class StoragesPerformance {
        private const int Warmup = 10;
        private const int Measurements = 10;
        private const int Iterations = 5000;
        protected readonly TestEntity Entity = new TestEntity();

        private static void RunMeasure(Action action) {
            Measure.Method(action)
                .WarmupCount(Warmup)
                .MeasurementCount(Measurements)
                .IterationsPerMeasurement(Iterations)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void PixeyeSignals() {
            var signals = new Signals();
            RunMeasure(() => {
                var receiver = new SignalsTestReceiver();
                signals.Add(receiver);
                signals.Send(1);
                signals.Remove(receiver);
            });
        }

        [Test, Performance]
        public void GlobalStorage() {
            var handler = new MockHandlerResolver(new[] {
                new HandlerMeta(null, typeof(int), 2,
                    EfficientInvoker.ForMethod(typeof(TestReceiver), "HandleSignal"))
            });
            var es = new GlobalStorage(handler);
            RunMeasure(() => {
                var receiver = new TestReceiver();
                es.On().Add(receiver);
                es.On().Send(1);
                es.On().Remove(receiver);
            });
        }

        [Test, Performance]
        public void EntityStorage() {
            var handler = new MockHandlerResolver(new[] {
                new HandlerMeta(typeof(TestEntity), typeof(int), 2,
                    EfficientInvoker.ForMethod(typeof(TestReceiver), "HandleSignal"))
            });
            var es = new EntityStorage(handler);
            var entity = new TestEntity();
            RunMeasure(() => {
                var receiver = new TestReceiver();
                es.On(entity).Add(receiver);
                es.On(entity).Send(1);
                es.On(entity).Remove(receiver);
            });
        }

        [Test, Performance]
        public void DynamicEntityStorage() {
            var handler = new MockHandlerResolver(new[] {
                new HandlerMeta(typeof(TestEntity), typeof(int), 2,
                    EfficientInvoker.ForMethod(typeof(TestReceiver), "HandleSignal"))
            });
            var es = new DynamicStorage(handler);
            var entity = new TestEntity();
            RunMeasure(() => {
                var receiver = new TestReceiver();
                es.On<TestEntity>().Add(receiver);
                es.On(entity).Send(1);
                es.On<TestEntity>().Remove(receiver);
            });
        }

        protected class TestEntity {
        }

        private class SignalsTestReceiver : IReceive<int> {
            public void HandleSignal(int arg) {
            }
        }

        private class MockHandlerResolver : IHandlersResolver {
            private readonly HandlerMeta[] _mockMeta;

            public MockHandlerResolver(HandlerMeta[] mockMeta) {
                _mockMeta = mockMeta;
            }

            public HandlerMeta[] GetHandlers(Type type) {
                return _mockMeta;
            }
        }

        protected class TestReceiver {
            [UsedImplicitly]
            public void HandleSignal(TestEntity entity, int i) {
            }
        }
    }
}