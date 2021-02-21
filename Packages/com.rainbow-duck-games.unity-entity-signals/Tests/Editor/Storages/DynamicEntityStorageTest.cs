using EntitySignals.Storages;
using NUnit.Framework;

namespace EntitySignals.Tests.Editor.Storages {
    public class DynamicEntityStorageTest : AbstractEntityStorageTest<DynamicStorage> {
        [SetUp]
        public void SetUp() {
            var handler = new MockHandlerResolver();
            Storage = new DynamicStorage(handler);
        }

        [Test]
        public void DynamicReceiverObject() {
            var receiver = new TestReceiver();
            var entity = new TestEntity();

            Storage.On<TestEntity>().Add(receiver);
            Assert.AreEqual(2, Storage.On<TestEntity>().Count);

            Storage.On(entity).Send(1);
            Storage.On(entity).Send('t');
            receiver.Verify("IntReceive", 1);
            receiver.Verify("CharReceive", entity, 't');

            Storage.On<TestEntity>().Remove(receiver);
            Assert.AreEqual(0, Storage.On<TestEntity>().Count);
        }

        [Test]
        public void DynamicReceiverDelegate() {
            var recorder = new Recorder();
            var entity = new TestEntity();
            var delegateA = new ESHandler<int>(i => { recorder.Record("A", i); });
            var delegateB = new ESHandler<TestEntity, char>((a, b) => { recorder.Record("B", a, b); });

            Storage.On<TestEntity>().Add(delegateA);
            Storage.On<TestEntity>().Add(delegateB);
            Assert.AreEqual(2, Storage.On<TestEntity>().Count);

            Storage.On(entity).Send(1);
            Storage.On(entity).Send('t');
            recorder.Verify("A", 1);
            recorder.Verify("B", entity, 't');

            Storage.On<TestEntity>().Remove(delegateA);
            Storage.On<TestEntity>().Remove(delegateB);
            Assert.AreEqual(0, Storage.On<TestEntity>().Count);
        }

        [Test]
        public void DynamicReceiverMethodReference() {
            var receiver = new TestReceiver();
            var entity = new TestEntity();
            var delegateA = new ESHandler<int>(receiver.IntReceive);
            var delegateB = new ESHandler<TestEntity, char>(receiver.CharReceive);

            Storage.On<TestEntity>().Add(delegateA);
            Storage.On<TestEntity>().Add(delegateB);
            Assert.AreEqual(2, Storage.On<TestEntity>().Count);

            Storage.On(entity).Send(1);
            Storage.On(entity).Send('t');
            receiver.Verify("IntReceive", 1);
            receiver.Verify("CharReceive", entity, 't');

            Storage.On<TestEntity>().Remove(delegateA);
            Storage.On<TestEntity>().Remove(delegateB);
            Assert.AreEqual(0, Storage.On<TestEntity>().Count);
        }

        [Test]
        public void DynamicAddHandlerOnSend() {
            var recorder = new Recorder();
            var entity = new TestEntity();
            var delegateA = new ESHandler<int>(i => { recorder.Record("A", i); });
            var delegateB = new ESHandler<TestEntity, char>((a, b) => {
                recorder.Record("B", a, b);
                Storage.On<TestEntity>().Add(delegateA);
            });

            Storage.On<TestEntity>().Add(delegateB);
            Storage.On(entity).Send(1);
            Storage.On(entity).Send('t');
            recorder.Never("A");
            recorder.Verify("B", entity, 't');
            Assert.AreEqual(2, Storage.On<TestEntity>().Count);
            
            Storage.On(entity).Send(42);
            recorder.Verify("A", 42);
        }

        [Test]
        public void DynamicRemoveHandlerOnSend() {
            var recorder = new Recorder();
            var entity = new TestEntity();
            var delegateA = new ESHandler<int>(i => { recorder.Record("A", i); });
            var delegateB = new ESHandler<TestEntity, char>((a, b) => {
                recorder.Record("B", a, b);
                Storage.On<TestEntity>().Remove(delegateA);
            });

            Storage.On<TestEntity>().Add(delegateA);
            Storage.On<TestEntity>().Add(delegateB);
            Storage.On(entity).Send(1);
            Storage.On(entity).Send('t');
            recorder.Verify("A", 1);
            recorder.Verify("B", entity, 't');
            Assert.AreEqual(1, Storage.On<TestEntity>().Count);
        }
    }
}