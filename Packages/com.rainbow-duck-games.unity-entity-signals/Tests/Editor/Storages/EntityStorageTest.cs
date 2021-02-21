using System;
using EntitySignals.Handlers;
using EntitySignals.Storages;
using EntitySignals.Utility.Tact;
using JetBrains.Annotations;
using NUnit.Framework;

namespace EntitySignals.Tests.Editor.Storages {
    public class EntityStorageTest {
        [Test]
        public void ReceiverObject() {
            var handler = new MockHandlerResolver();
            var storage = new EntityStorage(handler);
            var receiver = new TestReceiver();
            var entity = new TestEntity();

            storage.On(entity).Add(receiver);
            Assert.AreEqual(2, storage.On(entity).Count);
            handler.Verify("GetHandlers", typeof(TestReceiver));
            
            storage.On(entity).Send(1);
            storage.On(entity).Send('t');
            receiver.Verify("IntReceive",  1);
            receiver.Verify("CharReceive", entity, 't');
            
            storage.On(entity).Remove(receiver);
            Assert.AreEqual(0, storage.On(entity).Count);
        }
        
        [Test]
        public void ReceiverDelegate() {
            var storage = new EntityStorage(null);
            var recorder = new Recorder();
            var entity = new TestEntity();
            var delegateA = new ESHandler<int>(i => { recorder.Record("A", i); });
            var delegateB = new ESHandler<TestEntity, char>((a, b) => { recorder.Record("B", a, b); });

            storage.On(entity).Add(delegateA);
            storage.On(entity).Add(delegateB);
            Assert.AreEqual(2, storage.On(entity).Count);
            
            storage.On(entity).Send(1);
            storage.On(entity).Send('t');
            recorder.Verify("A", 1);
            recorder.Verify("B", entity, 't');
            
            storage.On(entity).Remove(delegateA);
            storage.On(entity).Remove(delegateB);
            Assert.AreEqual(0, storage.On(entity).Count);
        }
        
        [Test]
        public void ReceiverMethodReference() {
            var storage = new EntityStorage(null);
            var receiver = new TestReceiver();
            var entity = new TestEntity();
            var delegateA = new ESHandler<int>(receiver.IntReceive);
            var delegateB = new ESHandler<TestEntity, char>(receiver.CharReceive);

            storage.On(entity).Add(delegateA);
            storage.On(entity).Add(delegateB);
            Assert.AreEqual(2, storage.On(entity).Count);
            
            storage.On(entity).Send(1);
            storage.On(entity).Send('t');
            receiver.Verify("IntReceive", 1);
            receiver.Verify("CharReceive", entity, 't');
            
            storage.On(entity).Remove(delegateA);
            storage.On(entity).Remove(delegateB);
            Assert.AreEqual(0, storage.On(entity).Count);
        }

        [Test]
        public void DisposeDelegates() {
            var handler = new MockHandlerResolver();
            var storage = new EntityStorage(handler);
            var entity = new TestEntity();
            Assert.AreEqual(0, storage.On(entity).Count);
            
            storage.On(entity).Add(new TestReceiver());
            storage.Dispose();
            Assert.AreEqual(0, storage.On(entity).Count);
            handler.Verify("GetHandlers", typeof(TestReceiver));
        }

        [Test]
        public void CountDelegates() {
            var handler = new MockHandlerResolver();
            var storage = new EntityStorage(handler);
            var entity = new TestEntity();
            storage.On(entity).Add(new ESHandler<int>(i => {}));
            Assert.AreEqual(1, storage.On(entity).Count);
        }

        private class MockHandlerResolver : Recorder, IHandlersResolver {
            public HandlerMeta[] GetHandlers(Type type) {
                Record("GetHandlers", type);
                Assert.AreEqual(typeof(TestReceiver), type);
                return new[] {
                    new HandlerMeta(typeof(TestEntity), typeof(int), 1, EfficientInvoker.ForMethod(typeof(TestReceiver), "IntReceive")),
                    new HandlerMeta(typeof(TestEntity), typeof(char), 2, EfficientInvoker.ForMethod(typeof(TestReceiver), "CharReceive")),
                    // Invalid handler, should be ignored
                    new HandlerMeta(typeof(string), typeof(char), 2, null)
                };
            }
        }

        private class TestReceiver : Recorder {
            [UsedImplicitly]
            public void IntReceive(int i) {
                Record("IntReceive", i);
            }

            [UsedImplicitly]
            public void CharReceive(TestEntity entity, char c) {
                Record("CharReceive", entity, c);
            }
        }

        private class TestEntity {
        }
    }
}