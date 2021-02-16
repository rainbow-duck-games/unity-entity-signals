using System;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

#pragma warning disable 618

namespace EntitySignals.Tests.Editor {
    public class EntitySignalsTest {
        // ES.Subscribe(Any<Type>(), handler, ...);
        // ES.Register<Type>(entity);
        // Handler interface - part of the signal?
        //
        // Currents issues:
        // - Messy in calls(what context used?) & signal instances
        // - Hard to debug(no logging, no misconfiguration detection) - strict mode required?
        // - Hard to test(required to pass the whole type as receiver)
        // - Weak signatures(a lot of HandleSignal methods, hard to find out what is what)
        //
        // Features:
        // - Entity-related handlers
        // - Matching entity by predicate
        // - Strict mode to detect misconfiguration
        // - Verbose mode to detect misconfiguration(incl.on entity level)

        [Test]
        public void DefaultSignalContext() {
            var entity = Object.Instantiate(new GameObject());
            var recorder = new Recorder();
            var handler = (ESHandler<GameObject, int>) ((a1, a2) => { recorder.Record("First", a1, a2); });

            ES.On(entity).Add(handler);
            Assert.AreEqual(1, ES.On(entity).Count);

            ES.On(entity).Send(1);
            ES.Dispose();

            Assert.AreEqual(0, ES.On(entity).Count);
            recorder.Verify("First", entity, 1);
        }

        [Test]
        public void ProcessGlobalSignal() {
            var es = new EntitySignals();
            var recorder = new Recorder();
            var handler = (ESHandler<int>) ((a1) => { recorder.Record("First", a1); });

            es.Global.Add(handler);
            Assert.AreEqual(1, es.Global.Count);

            es.Global.Send(1);
            es.Global.Dispose();

            Assert.AreEqual(0, es.Global.Count);
            recorder.Verify("First", 1);
        }

        [Test]
        public void ProcessSimpleSignal() {
            var es = new EntitySignals();
            var entity = Object.Instantiate(new GameObject());
            var recorder = new Recorder();
            var handler = (ESHandler<GameObject, int>) ((a1, a2) => { recorder.Record("First", a1, a2); });

            es.On(entity).Add(handler);
            Assert.AreEqual(1, es.On(entity).Count);

            es.On(entity).Send(1);
            es.On(entity).Dispose();

            Assert.AreEqual(0, es.On(entity).Count);
            recorder.Verify("First", entity, 1);
        }

        [Test]
        public void ProcessReceiverClass() {
            var es = new EntitySignals();
            var entity = Object.Instantiate(new GameObject());
            var receiver = new TestReceiver();

            es.On(entity).Add(receiver);
            Assert.AreEqual(2, es.On(entity).Count);

            es.On(entity).Send(1);
            es.On(entity).Send('t');
            es.On(entity).Remove(receiver);

            Assert.AreEqual(0, es.On(entity).Count);
            receiver.Verify("First", entity, 1);
            receiver.Verify("Second", 't');
        }

        [Test]
        public void ProcessReceiverClassMethods() {
            var es = new EntitySignals();
            var entity = Object.Instantiate(new GameObject());
            var receiver = new TestReceiver();

            es.On(entity).Add((ESHandler<GameObject, int>) receiver.OnGameObjectFirstSignal);
            es.On(entity).Add((ESHandler<char>) receiver.OnGameObjectSecondSignal);
            Assert.AreEqual(2, es.On(entity).Count);

            es.On(entity).Send(1);
            es.On(entity).Send('t');
            es.On(entity).Dispose();

            Assert.AreEqual(0, es.On(entity).Count);
            receiver.Verify("First", entity, 1);
            receiver.Verify("Second", 't');
        }

        [Test]
        public void ThrowIfArgumentMismatch() {
            var es = new EntitySignals();
            var entity = Object.Instantiate(new GameObject());
            var receiver = new WrongTestReceiver();

            var e = Assert.Throws<Exception>(() => { es.On(entity).Add(receiver); });
            Assert.AreEqual(
                $"Can't bind method {nameof(WrongTestReceiver)}: Method WrongHandler has wrong amount of arguments",
                e.Message);
        }

        [Test]
        public void ThrowIfNothingToBind() {
            var es = new EntitySignals();
            var entity = Object.Instantiate(new GameObject());
            var receiver = new ComponentReceiver();

            var e = Assert.Throws<Exception>(() => { es.On(entity).Add(receiver); });
            Assert.AreEqual(
                $"Can't bind method {nameof(ComponentReceiver)} to entity UnityEngine.GameObject: No methods matched signature",
                e.Message);
        }

        private class TestReceiver : Recorder {
            [SignalHandler]
            public void OnGameObjectFirstSignal(GameObject obj, int x) {
                Record("First", obj, x);
            }

            [SignalHandler]
            public void OnGameObjectSecondSignal(char c) {
                Record("Second", c);
            }
        }

        private class WrongTestReceiver {
            [SignalHandler]
            public void WrongHandler(GameObject obj, int x, int y) {
                Assert.Fail("This method shouldn't be called");
            }
        }

        private class ComponentReceiver : Recorder {
            [SignalHandler]
            public void OnGameObjectSecondSignal(Component obj, char c) {
                Record("Second", obj, c);
            }
        }
    }
}