using System.Linq;
using NUnit.Framework;
using RainbowDuckGames.UnityEntitySignals.Handlers;

namespace RainbowDuckGames.UnityEntitySignals.Tests.Editor.Handlers {
    public class InterfaceHandlersResolverTest {
        [Test]
        public void ProcessReceiverClass() {
            var receiver = new TestReceiver();
            var resolver = new InterfaceHandlersResolver();

            var results = resolver.GetHandlers(typeof(TestReceiver));
            Assert.AreEqual(2, results.Length);

            var intHandler = results.First(m => m.SignalType == typeof(int));
            Assert.AreEqual(2, intHandler.ParamCount);
            Assert.AreEqual(typeof(string), intHandler.RequiredType);
            intHandler.MethodInvoker.Invoke(receiver, "entity", 1);
            receiver.Verify("Int", "entity", 1);

            var charHandler = results.First(m => m.SignalType == typeof(char));
            Assert.AreEqual(2, charHandler.ParamCount);
            Assert.AreEqual(typeof(string), charHandler.RequiredType);
            charHandler.MethodInvoker.Invoke(receiver, "entity", 't');
            receiver.Verify("Char", "entity", 't');
        }

        [Test]
        public void ProcessEmptyClass() {
            var resolver = new InterfaceHandlersResolver();
            var results = resolver.GetHandlers(typeof(EmptyReceiver));
            Assert.AreEqual(0, results.Length);
        }

        private class TestReceiver : Recorder, IReceive<string, int>, IReceive<string, char> {
            public void HandleSignal(string entity, int signal) {
                Record("Int", entity, signal);
            }

            public void HandleSignal(string entity, char signal) {
                Record("Char", entity, signal);
            }
        }

        private class EmptyReceiver : Recorder {
        }
    }
}