using System.Collections.Generic;
using EntitySignals.Contexts;
using EntitySignals.Handlers;

namespace EntitySignals.Storage {
    public class DynamicTypeContext<TEntity> : AbstractContext<TEntity> {
        private readonly DynamicSignalsStorage _storage;

        public DynamicTypeContext(IHandlersResolver resolver, DynamicSignalsStorage storage) : base(resolver) {
            _storage = storage;
        }

        public override void Send<TSignal>(TSignal arg) {
            // ToDo dynamic ExecuteSend();
        }

        public override void Dispose() {
            _storage.GetDelegates<TEntity>().Clear();
        }

        protected override List<HandlerDelegate> GetContextDelegates() {
            return _storage.GetDelegates<TEntity>();
        }
    }
}