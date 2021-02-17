using System;
using EntitySignals.Handlers;

namespace EntitySignals.Context {
    internal class NullContext : EntityContext<object> {
        public NullContext(IHandlersResolver resolver, EntitySignals entitySignals) : base(resolver, entitySignals) {
        }

        public override void Add<TSignal>(ESHandler<object, TSignal> signalHandler) {
            throw new Exception("Entity specific handler is not allowed for NoEntityRecords");
        }

        public override void Remove<TSignal>(ESHandler<object, TSignal> signalHandler) {
            throw new Exception("Entity specific handler is not allowed for NoEntityRecords");
        }
    }
}