using EntitySignals.Storages;
using NUnit.Framework;

namespace EntitySignals.Tests.Editor.Storages {
    public class EntityStorageTest : AbstractEntityStorageTest<EntityStorage> {
        [SetUp]
        public void SetUp() {
            var handler = new MockHandlerResolver();
            Storage = new EntityStorage(handler);
        }
    }
}