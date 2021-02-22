using NUnit.Framework;
using RainbowDuckGames.UnityEntitySignals.Handlers;
using UnityEngine;
using Object = UnityEngine.Object;

#pragma warning disable 618

namespace RainbowDuckGames.UnityEntitySignals.Tests.Editor {
    public class EntitySignalsTest {
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

            es.On().Add(handler);
            Assert.AreEqual(1, es.On().Count);

            es.On().Send(1);
            es.On().Dispose();

            Assert.AreEqual(0, es.On().Count);
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

            es.On(entity).Add((ESHandler<GameObject, int>) receiver.OnGameObjectSignal);
            es.On(entity).Add((ESHandler<char>) receiver.OnGameObjectSignal);
            Assert.AreEqual(2, es.On(entity).Count);

            es.On(entity).Send(1);
            es.On(entity).Send('t');
            es.On(entity).Dispose();

            Assert.AreEqual(0, es.On(entity).Count);
            receiver.Verify("First", entity, 1);
            receiver.Verify("Second", 't');
        }

        private class TestReceiver : Recorder {
            [SignalHandler]
            public void OnGameObjectSignal(GameObject obj, int x) {
                Record("First", obj, x);
            }

            [SignalHandler]
            public void OnGameObjectSignal(char c) {
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