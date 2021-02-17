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
            var oldTime = RunNTimes(() => {
                var sig = new Signals();
                var receiver = new SignalsTestReceiver();
                sig.Add(receiver);
                sig.Send(1);
                sig.Remove(receiver);
                receiver.Verify("First", 1);
            }, 10000);
            
            var attributesTime = RunNTimes(() => {
                var es = new EntitySignals(new AttributeHandlersResolver());
                var receiver = new AttributeTestReceiver();
                es.On(entity).Add(receiver);
                es.On(entity).Send(1);
                es.On(entity).Remove(receiver);
                receiver.Verify("First", entity, 1);
            }, 10000);
            
            var attributesCachedTime = RunNTimes(() => {
                var es = new EntitySignals(new CachedHandlersResolver(new AttributeHandlersResolver()));
                var receiver = new AttributeTestReceiver();
                es.On(entity).Add(receiver);
                es.On(entity).Send(1);
                es.On(entity).Remove(receiver);
                receiver.Verify("First", entity, 1);
            }, 10000);
            
            var interfaceTime = RunNTimes(() => {
                var es = new EntitySignals(new InterfaceHandlersResolver());
                var receiver = new InterfaceTestReceiver();
                es.On(entity).Add(receiver);
                es.On(entity).Send(1);
                es.On(entity).Remove(receiver);
                receiver.Verify("First", entity, 1);
            }, 10000);
            
            var interfaceCachedTime = RunNTimes(() => {
                var es = new EntitySignals(new CachedHandlersResolver(new InterfaceHandlersResolver()));
                var receiver = new InterfaceTestReceiver();
                es.On(entity).Add(receiver);
                es.On(entity).Send(1);
                es.On(entity).Remove(receiver);
                receiver.Verify("First", entity, 1);
            }, 10000);

            Debug.Log($"FULL FLOW: Results for 10k runs");
            Debug.Log($"REFERENCE: FULL FLOW - took {oldTime} ms to complete");
            Debug.Log($"ATTRIBUTE: FULL FLOW - took {attributesTime} ms to complete");
            Debug.Log($"ATT CACHE: FULL FLOW - took {attributesCachedTime} ms to complete");
            Debug.Log($"INTERFACE: FULL FLOW - took {interfaceTime} ms to complete");
            Debug.Log($"INT CACHE: FULL FLOW - took {interfaceCachedTime} ms to complete");
        }

        [Test]
        public void ProcessSendOnly() {
            // New system
            var es = new EntitySignals();
            var receiver = new AttributeTestReceiver();
            var entity = new TestEntity();
            es.On(entity).Add(receiver);
            var newTime = RunNTimes(() => es.On(entity).Send(1));
            es.On(entity).Remove(receiver);

            // Old system
            var sig = new Signals();
            var sReceiver = new SignalsTestReceiver();
            sig.Add(sReceiver);
            var oldTime = RunNTimes(() => sig.Send(1));
            sig.Remove(sReceiver);

            Debug.Log($"NEW ES: SEND ONLY - took {newTime} ms to complete");
            Debug.Log($"OLD ES: SEND ONLY - took {oldTime} ms to complete");
        }

        [Test]
        public void ProcessSubscribeOnly() {
            // New system
            var es = new EntitySignals();
            var entity = new TestEntity();
            var newTime = RunNTimes(() => {
                var receiver = new AttributeTestReceiver();
                es.On(entity).Add(receiver);
            });

            // Old system
            var sig = new Signals();
            var oldTime = RunNTimes(() => {
                var receiver = new SignalsTestReceiver();
                sig.Add(receiver);
            });

            Debug.Log($"NEW ES: SUBSCRIBE ONLY - took {newTime} ms to complete");
            Debug.Log($"OLD ES: SUBSCRIBE ONLY - took {oldTime} ms to complete");
        }

        private static long RunNTimes(Action action, long runs = 100000) {
            var st = new Stopwatch();
            st.Start();
            for (var i = 0; i < runs; i++) {
                action.Invoke();
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

        private class SignalsTestReceiver : Recorder, IReceive<int> {
            public void HandleSignal(int arg) {
                Record("First", arg);
            }
        }

        private class InterfaceTestReceiver : Recorder, IReceive<TestEntity, int> {
            public void HandleSignal(TestEntity entity, int signal) {
                Record("First", entity, signal);
            }
        }
    }
}