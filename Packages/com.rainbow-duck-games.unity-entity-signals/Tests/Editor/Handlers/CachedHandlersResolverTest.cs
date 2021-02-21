using System;
using System.Linq;
using EntitySignals.Handlers;
using NUnit.Framework;

namespace EntitySignals.Tests.Editor.Handlers {
    public class CachedHandlersResolverTest {
        [Test]
        public void ProcessCachedResolver() {
            var testResolver = new TestResolver();
            var resolver = new CachedHandlersResolver(testResolver);
            var results = resolver.GetHandlers(typeof(int));
            Assert.AreEqual(1, results.Length);

            var charHandler = results.First(m => m.SignalType == typeof(char));
            Assert.AreEqual(3, charHandler.ParamCount);
            Assert.AreEqual(typeof(string), charHandler.RequiredType);
            Assert.Null(charHandler.MethodInvoker);

            var second = resolver.GetHandlers(typeof(int));
            Assert.AreEqual(results, second);
        }

        private class TestResolver : IHandlersResolver {
            private int _count;

            public HandlerMeta[] GetHandlers(Type type) {
                Assert.AreEqual(typeof(int), type);
                Assert.AreEqual(1, ++_count);
                return new[] {
                    new HandlerMeta(typeof(string), typeof(char), 3, null)
                };
            }
        }
    }
}