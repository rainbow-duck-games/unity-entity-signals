using System;
using System.Diagnostics;
using EntitySignals.Handlers;
using Homebrew;
using NUnit.Framework;
using Debug = UnityEngine.Debug;

#pragma warning disable 618

namespace EntitySignals.Tests.Editor {
    public class EntitySignalsPerf {
        [Test]
        public void ProcessSubscribeSendUnsubscribe() {
            var entity = new TestEntity();
            var sig = new Signals();
            var oldTime = RunNTimes(i => {
                var receiver = new SignalsTestReceiver();
                sig.Add(receiver);
                sig.Send(i);
                sig.Remove(receiver);
                receiver.Verify("First", i);
                Assert.AreEqual(0, sig.ReceiverCount());
            });

            Action<int> Prepare(EntitySignals es, Func<Recorder> receiverPrepare) {
                return i => {
                    var receiver = receiverPrepare.Invoke();
                    es.On(entity).Add(receiver);
                    es.On(entity).Send(i);
                    es.On(entity).Remove(receiver);
                    receiver.Verify("First", entity, i);
                    Assert.AreEqual(0, es.On(entity).Count);
                };
            }

            var attributesTime = RunNTimes(Prepare(
                new EntitySignals(new AttributeHandlersResolver()),
                () => new AttributeTestReceiver()));
            var attributesCachedTime = RunNTimes(Prepare(
                new EntitySignals(new CachedHandlersResolver(new AttributeHandlersResolver())),
                () => new AttributeCacheTestReceiver()));
            var interfaceTime = RunNTimes(Prepare(
                new EntitySignals(new InterfaceHandlersResolver()),
                () => new InterfaceTestReceiver()));
            var interfaceCachedTime = RunNTimes(Prepare(
                new EntitySignals(new CachedHandlersResolver(new InterfaceHandlersResolver())),
                () => new InterfaceCacheTestReceiver()));

            Debug.Log($"FULL FLOW: Results for 100k runs");
            Debug.Log($"REFERENCE: FULL FLOW - took {oldTime} ms to complete");
            Debug.Log($"ATTRIBUTE: FULL FLOW - took {attributesTime} ms to complete");
            Debug.Log($"ATT CACHE: FULL FLOW - took {attributesCachedTime} ms to complete");
            Debug.Log($"INTERFACE: FULL FLOW - took {interfaceTime} ms to complete");
            Debug.Log($"INT CACHE: FULL FLOW - took {interfaceCachedTime} ms to complete");
        }

        [Test]
        public void ProcessSendOnly() {
            var entity = new TestEntity();

            var sig = new Signals();
            var sReceiver = new SignalsTestReceiver();
            sig.Add(sReceiver);
            var oldTime = RunNTimes(i => { sig.Send(i); });

            Action<int> Prepare(EntitySignals es, Recorder receiver) {
                es.On(entity).Add(receiver);
                return i => { es.On(entity).Send(i); };
            }

            var attributesTime = RunNTimes(Prepare(
                new EntitySignals(new AttributeHandlersResolver()),
                new AttributeTestReceiver()
            ));
            var attributesCachedTime = RunNTimes(Prepare(
                new EntitySignals(new CachedHandlersResolver(new AttributeHandlersResolver())),
                new AttributeCacheTestReceiver()
            ));
            var interfaceTime = RunNTimes(Prepare(
                new EntitySignals(new InterfaceHandlersResolver()),
                new InterfaceTestReceiver()
            ));
            var interfaceCachedTime = RunNTimes(Prepare(
                new EntitySignals(new CachedHandlersResolver(new InterfaceHandlersResolver())),
                new InterfaceCacheTestReceiver()
            ));

            Debug.Log($"SEND ONLY: Results for 100k runs");
            Debug.Log($"REFERENCE: SEND ONLY - took {oldTime} ms to complete");
            Debug.Log($"ATTRIBUTE: SEND ONLY - took {attributesTime} ms to complete");
            Debug.Log($"ATT CACHE: SEND ONLY - took {attributesCachedTime} ms to complete");
            Debug.Log($"INTERFACE: SEND ONLY - took {interfaceTime} ms to complete");
            Debug.Log($"INT CACHE: SEND ONLY - took {interfaceCachedTime} ms to complete");
        }

        [Test]
        public void ProcessSubscribeOnly() {
            var entity = new TestEntity();

            var sig = new Signals();
            var oldTime = RunNTimes(i => {
                var receiver = new SignalsTestReceiver();
                sig.Add(receiver);
            });
            sig.Send(99999);

            Action<int> Prepare(EntitySignals es, Func<Recorder> receiverCreate) {
                return i => {
                    var receiver = receiverCreate.Invoke();
                    es.On(entity).Add(receiver);
                };
            }

            var attributesTime = RunNTimes(Prepare(
                new EntitySignals(new AttributeHandlersResolver()),
                () => new AttributeTestReceiver()
            ));
            var attributesCachedTime = RunNTimes(Prepare(
                new EntitySignals(new CachedHandlersResolver(new AttributeHandlersResolver())),
                () => new AttributeCacheTestReceiver()
            ));
            var interfaceTime = RunNTimes(Prepare(
                new EntitySignals(new InterfaceHandlersResolver()),
                () => new InterfaceTestReceiver()
            ));
            var interfaceCachedTime = RunNTimes(Prepare(
                new EntitySignals(new CachedHandlersResolver(new InterfaceHandlersResolver())),
                () => new InterfaceCacheTestReceiver()
            ));

            Debug.Log($"SUBSCRIBE ONLY: Results for 100k runs");
            Debug.Log($"REFERENCE: SUBSCRIBE ONLY - took {oldTime} ms to complete");
            Debug.Log($"ATTRIBUTE: SUBSCRIBE ONLY - took {attributesTime} ms to complete");
            Debug.Log($"ATT CACHE: SUBSCRIBE ONLY - took {attributesCachedTime} ms to complete");
            Debug.Log($"INTERFACE: SUBSCRIBE ONLY - took {interfaceTime} ms to complete");
            Debug.Log($"INT CACHE: SUBSCRIBE ONLY - took {interfaceCachedTime} ms to complete");
        }

        private static long RunNTimes(Action<int> action, int runs = 100000) {
            var st = new Stopwatch();
            st.Start();
            for (var i = 0; i < runs; i++) {
                action.Invoke(i);
            }

            st.Stop();
            return st.ElapsedMilliseconds;
        }

        private class TestEntity {
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