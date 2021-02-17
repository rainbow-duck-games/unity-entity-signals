using EntitySignals.Handlers;
using NUnit.Framework;
using UnityEngine;

namespace EntitySignals.Tests.Editor.Handlers {
    public class InterfaceHandlersResolverTest {
        [Test]
        public void ProcessReceiverClass() {
            var es = new EntitySignals(new InterfaceHandlersResolver());
            var entity = Object.Instantiate(new GameObject());
            var receiver = new TestReceiver();

            es.On(entity).Add(receiver);
            Assert.AreEqual(2, es.On(entity).Count);

            es.On(entity).Send(1);
            es.On(entity).Send('t');
            es.On(entity).Remove(receiver);

            Assert.AreEqual(0, es.On(entity).Count);
            receiver.Verify("First", entity, 1);
            receiver.Verify("Second", entity, 't');
        }

        private class TestReceiver : Recorder, IReceive<GameObject, int>, IReceive<GameObject, char> {
            public void HandleSignal(GameObject entity, int signal) {
                Record("First", entity, signal);
            }

            public void HandleSignal(GameObject entity, char signal) {
                Record("Second", entity, signal);
            }
        }
    }
}