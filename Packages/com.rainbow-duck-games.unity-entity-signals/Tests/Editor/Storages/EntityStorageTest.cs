using NUnit.Framework;
using RainbowDuckGames.UnityEntitySignals.Storages;

namespace RainbowDuckGames.UnityEntitySignals.Tests.Editor.Storages {
    public class EntityStorageTest : AbstractEntityStorageTest<EntityStorage> {
        [SetUp]
        public void SetUp() {
            var handler = new MockHandlerResolver();
            Storage = new EntityStorage(handler);
        }
    }
}