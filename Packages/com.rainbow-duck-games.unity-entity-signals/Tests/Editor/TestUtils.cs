﻿using System.Collections.Generic;
using NUnit.Framework;

namespace EntitySignals.Tests.Editor {
    public class Recorder {
        private readonly Dictionary<string, object[]> _records = new Dictionary<string, object[]>();

        public void Record(string key, params object[] args) {
            _records[key] = args;
        }

        public void Verify(string key, params object[] args) {
            Assert.IsTrue(_records.ContainsKey(key), $"No record with key '{key}' found for instance {GetType()}");
            Assert.AreEqual(_records[key], args, $"Arguments of the call are not matching for instance {GetType()}");
        }
    }
}