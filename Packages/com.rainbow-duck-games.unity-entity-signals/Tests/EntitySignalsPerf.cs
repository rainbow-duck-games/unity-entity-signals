using System;
using System.Diagnostics;
using Homebrew;
using NUnit.Framework;
using Debug = UnityEngine.Debug;

#pragma warning disable 618

namespace EntitySignals.Tests {
    public class EntitySignalsPerf {
        [Test]
        public void ProcessSubscribeSendUnsubscribe() {
            var entity = new TestEntity();
            var newTime = Elasped(() => {
                var es = new global::EntitySignals.EntitySignals();
                var receiver = new TestReceiver();
                es.On(entity).Add(receiver);
                es.On(entity).Send(1);
                es.On(entity).Remove(receiver);
                // MonoMock.Verify(receiver, "First", entity, 1);
            });
        
            var oldTime = Elasped(() => {
                var sig = new Signals();
                var receiver = new SignalsTestReceiver();
                sig.Add(receiver);
                sig.Send(1);
                sig.Remove(receiver);
                // MonoMock.Verify(receiver, "First", 1);
            });
            
            Debug.Log($"NEW ES: FULL FLOW - took {newTime} ms to complete");
            Debug.Log($"OLD ES: FULL FLOW - took {oldTime} ms to complete");
        }
        
        [Test]
        public void ProcessSendOnly() {
            // New system
            var es = new global::EntitySignals.EntitySignals();
            var receiver = new TestReceiver();
            var entity = new TestEntity();
            es.On(entity).Add(receiver);
            var newTime = Elasped(() => es.On(entity).Send(1));
            es.On(entity).Remove(receiver);
        
            // Old system
            var sig = new Signals();
            var sReceiver = new SignalsTestReceiver();
            sig.Add(sReceiver);
            var oldTime = Elasped(() => sig.Send(1));
            sig.Remove(sReceiver);
            
            Debug.Log($"NEW ES: SEND ONLY - took {newTime} ms to complete");
            Debug.Log($"OLD ES: SEND ONLY - took {oldTime} ms to complete");
        }
        
        [Test]
        public void ProcessSubscribeOnly() {
            // New system
            var es = new global::EntitySignals.EntitySignals();
            var entity = new TestEntity();
            var newTime = Elasped(() => {
                var receiver = new TestReceiver();
                es.On(entity).Add(receiver);
            });
        
            // Old system
            var sig = new Signals();
            var oldTime = Elasped(() => {
                var receiver = new SignalsTestReceiver();
                sig.Add(receiver);
            });
            
            Debug.Log($"NEW ES: SUBSCRIBE ONLY - took {newTime} ms to complete");
            Debug.Log($"OLD ES: SUBSCRIBE ONLY - took {oldTime} ms to complete");
        }
        
        private long Elasped(Action action, long runs = 100000) {
            var st = new Stopwatch();
            st.Start();
            for (var i = 0; i < runs; i++) {
                action.Invoke();
            }
        
            st.Stop();
            return st.ElapsedMilliseconds;
        }
        
        private class TestReceiver {
            [SignalHandler]
            public void OnGameObjectFirstSignal(TestEntity obj, int x) {
                // MonoMock.Record(this, "First", obj, x);
            }
        }
        
        private class TestEntity { }
        
        private class SignalsTestReceiver : IReceive<int> {
            public void HandleSignal(int arg) {
                // MonoMock.Record(this, "First", arg);
            }
        }
    }
}