using System;
using EntitySignals.Handlers;
using EntitySignals.Storages;
using EntitySignals.Utility.Tact;
using JetBrains.Annotations;
using NUnit.Framework;

namespace EntitySignals.Tests.Editor.Storages {
    public abstract partial class AbstractEntityStorageTest<TStorage> where TStorage : EntityStorage {
        protected TStorage Storage;

        [Test]
        public void ReceiverObject() {
            var receiver = new TestReceiver();
            var entity = new TestEntity();

            Storage.On(entity).Add(receiver);
            Assert.AreEqual(2, Storage.On(entity).Count);

            Storage.On(entity).Send(1);
            Storage.On(entity).Send('t');
            receiver.Verify("IntReceive", 1);
            receiver.Verify("CharReceive", entity, 't');

            Storage.On(entity).Remove(receiver);
            Assert.AreEqual(0, Storage.On(entity).Count);
        }

        [Test]
        public void ReceiverDelegate() {
            var recorder = new Recorder();
            var entity = new TestEntity();
            var delegateA = new ESHandler<int>(i => { recorder.Record("A", i); });
            var delegateB = new ESHandler<TestEntity, char>((a, b) => { recorder.Record("B", a, b); });

            Storage.On(entity).Add(delegateA);
            Storage.On(entity).Add(delegateB);
            Assert.AreEqual(2, Storage.On(entity).Count);

            Storage.On(entity).Send(1);
            Storage.On(entity).Send('t');
            recorder.Verify("A", 1);
            recorder.Verify("B", entity, 't');

            Storage.On(entity).Remove(delegateA);
            Storage.On(entity).Remove(delegateB);
            Assert.AreEqual(0, Storage.On(entity).Count);
        }

        [Test]
        public void ReceiverMethodReference() {
            var receiver = new TestReceiver();
            var entity = new TestEntity();
            var delegateA = new ESHandler<int>(receiver.IntReceive);
            var delegateB = new ESHandler<TestEntity, char>(receiver.CharReceive);

            Storage.On(entity).Add(delegateA);
            Storage.On(entity).Add(delegateB);
            Assert.AreEqual(2, Storage.On(entity).Count);

            Storage.On(entity).Send(1);
            Storage.On(entity).Send('t');
            receiver.Verify("IntReceive", 1);
            receiver.Verify("CharReceive", entity, 't');

            Storage.On(entity).Remove(delegateA);
            Storage.On(entity).Remove(delegateB);
            Assert.AreEqual(0, Storage.On(entity).Count);
        }

        [Test]
        public void DisposeDelegates() {
            var entity = new TestEntity();
            Assert.AreEqual(0, Storage.On(entity).Count);

            Storage.On(entity).Add(new TestReceiver());
            Storage.Dispose();
            Assert.AreEqual(0, Storage.On(entity).Count);
        }

        [Test]
        public void CountDelegates() {
            var entity = new TestEntity();
            Storage.On(entity).Add(new ESHandler<int>(i => { }));
            Assert.AreEqual(1, Storage.On(entity).Count);
        }

        [Test]
        public void ThrowIfNoHandlers() {
            var handler = new NoHandlersResolver();
            var storage = new GlobalStorage(handler);
            var receiver = new TestReceiver();

            var e = Assert.Throws<Exception>(() => storage.On().Add(receiver));
            Assert.AreEqual("Can't bind method TestReceiver to global: No methods matched signature", e.Message);
        }

        private class NoHandlersResolver : IHandlersResolver {
            public HandlerMeta[] GetHandlers(Type type) {
                Assert.AreEqual(typeof(TestReceiver), type);
                return new HandlerMeta[] { };
            }
        }

        protected class MockHandlerResolver : Recorder, IHandlersResolver {
            public HandlerMeta[] GetHandlers(Type type) {
                Record("GetHandlers", type);
                Assert.AreEqual(typeof(TestReceiver), type);
                return new[] {
                    new HandlerMeta(typeof(TestEntity), typeof(int), 1,
                        EfficientInvoker.ForMethod(typeof(TestReceiver), "IntReceive")),
                    new HandlerMeta(typeof(TestEntity), typeof(char), 2,
                        EfficientInvoker.ForMethod(typeof(TestReceiver), "CharReceive")),
                    // Invalid handler, should be ignored
                    new HandlerMeta(typeof(string), typeof(char), 2, null)
                };
            }
        }

        protected class TestReceiver : Recorder {
            [UsedImplicitly]
            public void IntReceive(int i) {
                Record("IntReceive", i);
            }

            [UsedImplicitly]
            public void CharReceive(TestEntity entity, char c) {
                Record("CharReceive", entity, c);
            }
        }

        protected class TestEntity {
        }
    }
}