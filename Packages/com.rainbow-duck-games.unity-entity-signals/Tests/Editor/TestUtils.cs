using System.Collections.Generic;
using NUnit.Framework;

namespace RainbowDuckGames.UnityEntitySignals.Tests.Editor {
    public class Recorder {
        private readonly Dictionary<string, object[]> _records = new Dictionary<string, object[]>();

        public void Record(string key, params object[] args) {
            _records[key] = args;
        }

        public void Verify(string key, params object[] args) {
            Assert.IsTrue(_records.ContainsKey(key), $"No record with key '{key}' found for instance {GetType()}");
            Assert.AreEqual(args, _records[key], $"Arguments of the call are not matching for instance {GetType()}");
        }

        public void Never(string key) {
            Assert.IsFalse(_records.ContainsKey(key),
                $"Record with key '{key}' shouldn't be called for instance {GetType()}");
        }
    }
}