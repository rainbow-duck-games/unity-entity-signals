using System;

namespace EntitySignals.Context {
    internal class NullContext : EntityContext<object> {
        public NullContext(HandlersCache cache, EntitySignals entitySignals) : base(cache, entitySignals, null) {
        }

        public override void Add<TSignal>(ESHandler<object, TSignal> signalHandler) {
            throw new Exception("Entity specific handler is not allowed for NoEntityRecords");
        }

        public override void Remove<TSignal>(ESHandler<object, TSignal> signalHandler) {
            throw new Exception("Entity specific handler is not allowed for NoEntityRecords");
        }
    }
}