using System.Collections.Generic;
using NUnit.Framework;

namespace EntitySignals.Tests {
    public class Recorder {
        private Dictionary<string, object[]> records = new Dictionary<string, object[]>();

        public void Record(string key, params object[] args) {
            records[key] = args;
        }

        public void Verify(string key, params object[] args) {
            Assert.IsTrue(records.ContainsKey(key));
            Assert.AreEqual(records[key], args);
        }
    }
}