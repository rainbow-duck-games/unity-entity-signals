using System;
using EntitySignals.Handlers;
using EntitySignals.Storages;
using EntitySignals.Utility.Tact;
using JetBrains.Annotations;
using NUnit.Framework;

namespace EntitySignals.Tests.Editor.Storages {
    public class GlobalStorageTest {
        [Test]
        public void ReceiverObject() {
            var handler = new MockHandlerResolver();
            var storage = new GlobalStorage(handler);
            var receiver = new TestReceiver();

            storage.On().Add(receiver);
            Assert.AreEqual(2, storage.Count);
            
            storage.On().Send(1);
            storage.On().Send('t');
            receiver.Verify("IntReceive", 1);
            receiver.Verify("CharReceive", null, 't');
            
            storage.On().Remove(receiver);
            Assert.AreEqual(0, storage.Count);
        }
        
        [Test]
        public void ReceiverDelegate() {
            var storage = new GlobalStorage(null);
            var recorder = new Recorder();
            var delegateA = new ESHandler<int>(i => { recorder.Record("A", i); });
            var delegateB = new ESHandler<object, char>((a, b) => { recorder.Record("B", a, b); });

            storage.On().Add(delegateA);
            storage.On().Add(delegateB);
            Assert.AreEqual(2, storage.Count);
            
            storage.On().Send(1);
            storage.On().Send('t');
            recorder.Verify("A", 1);
            recorder.Verify("B", null, 't');
            
            storage.On().Remove(delegateA);
            storage.On().Remove(delegateB);
            Assert.AreEqual(0, storage.Count);
        }
        
        [Test]
        public void ReceiverMethodReference() {
            var storage = new GlobalStorage(null);
            var receiver = new TestReceiver();
            var delegateA = new ESHandler<int>(receiver.IntReceive);
            var delegateB = new ESHandler<object, char>(receiver.CharReceive);

            storage.On().Add(delegateA);
            storage.On().Add(delegateB);
            Assert.AreEqual(2, storage.Count);
            
            storage.On().Send(1);
            storage.On().Send('t');
            receiver.Verify("IntReceive", 1);
            receiver.Verify("CharReceive", null, 't');
            
            storage.On().Remove(delegateA);
            storage.On().Remove(delegateB);
            Assert.AreEqual(0, storage.Count);
        }

        [Test]
        public void AddHandlerOnSend() {
            var storage = new GlobalStorage(null);
            var recorder = new Recorder();
            var delegateA = new ESHandler<int>(i => { recorder.Record("A", i); });
            var delegateB = new ESHandler<object, char>((a, b) => {
                recorder.Record("B", a, b);
                storage.On().Add(delegateA);
            });

            storage.On().Add(delegateB);
            storage.On().Send(1);
            storage.On().Send('t');
            recorder.Never("A");
            recorder.Verify("B", null, 't');
            Assert.AreEqual(2, storage.On().Count);
            
            storage.On().Send(42);
            recorder.Verify("A", 42);
        }

        [Test]
        public void RemoveHandlerOnSend() {
            var storage = new GlobalStorage(null);
            var recorder = new Recorder();
            var delegateA = new ESHandler<int>(i => { recorder.Record("A", i); });
            var delegateB = new ESHandler<object, char>((a, b) => {
                recorder.Record("B", a, b);
                storage.On().Remove(delegateA);
            });

            storage.On().Add(delegateA);
            storage.On().Add(delegateB);
            storage.On().Send(1);
            storage.On().Send('t');
            recorder.Verify("A", 1);
            recorder.Verify("B", null, 't');
            Assert.AreEqual(1, storage.On().Count);
        }

        [Test]
        public void DisposeDelegates() {
            var handler = new MockHandlerResolver();
            var storage = new GlobalStorage(handler);
            Assert.AreEqual(0, storage.Count);
            
            storage.On().Add(new TestReceiver());
            storage.Dispose();
            Assert.AreEqual(0, storage.Count);
        }

        [Test]
        public void CountDelegates() {
            var handler = new MockHandlerResolver();
            var storage = new GlobalStorage(handler);
            storage.On().Add(new ESHandler<int>(i => {}));
            Assert.AreEqual(1, storage.Count);
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

        private class MockHandlerResolver : IHandlersResolver {
            public HandlerMeta[] GetHandlers(Type type) {
                Assert.AreEqual(typeof(TestReceiver), type);
                return new[] {
                    new HandlerMeta(null, typeof(int), 1, EfficientInvoker.ForMethod(typeof(TestReceiver), "IntReceive")),
                    new HandlerMeta(null, typeof(char), 2, EfficientInvoker.ForMethod(typeof(TestReceiver), "CharReceive")),
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
            public void CharReceive(object entity, char c) {
                Record("CharReceive", entity, c);
            }
        }
    }
}