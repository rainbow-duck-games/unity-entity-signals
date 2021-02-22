using System;
using System.Linq;
using NUnit.Framework;
using RainbowDuckGames.UnityEntitySignals.Handlers;

namespace RainbowDuckGames.UnityEntitySignals.Tests.Editor.Handlers {
    public class AttributesHandlersResolverTest {
        [Test]
        public void ProcessReceiverClass() {
            var receiver = new TestReceiver();
            var resolver = new AttributeHandlersResolver();

            var results = resolver.GetHandlers(receiver.GetType());
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
        public void ProcessSingleReceiverClass() {
            var receiver = new TestSingleReceiver();
            var resolver = new AttributeHandlersResolver();

            var results = resolver.GetHandlers(receiver.GetType());
            Assert.AreEqual(1, results.Length);

            var intHandler = results.First(m => m.SignalType == typeof(int));
            Assert.AreEqual(1, intHandler.ParamCount);
            Assert.AreEqual(typeof(string), intHandler.RequiredType);
            intHandler.MethodInvoker.Invoke(receiver, 42);
            receiver.Verify("Int", 42);
        }

        [Test]
        public void ExceptionIfArgumentMismatch() {
            var resolver = new AttributeHandlersResolver();
            var ex = Assert.Throws<Exception>(() => resolver.GetHandlers(typeof(WrongReceiver)));
            Assert.AreEqual("Can't bind method WrongReceiver: Method HandleSignal has wrong amount of arguments", ex.Message);
        }

        [Test]
        public void ProcessEmptyClass() {
            var resolver = new AttributeHandlersResolver();
            var results = resolver.GetHandlers(typeof(EmptyReceiver));
            Assert.AreEqual(0, results.Length);
        }

        private class TestReceiver : Recorder {
            [SignalHandler]
            public void HandleSignal(string entity, int signal) {
                Record("Int", entity, signal);
            }

            [SignalHandler]
            public void HandleSignal(string entity, char signal) {
                Record("Char", entity, signal);
            }
        }

        private class TestSingleReceiver : Recorder {
            [SignalHandler(typeof(string))]
            public void HandleSignal(int signal) {
                Record("Int", signal);
            }
        }

        private class WrongReceiver : Recorder {
            [SignalHandler]
            public void HandleSignal(string entity, int signal, char third) {
                throw new Exception("Shouldn't be called");
            }
        }

        private class EmptyReceiver : Recorder {
        }
    }
}