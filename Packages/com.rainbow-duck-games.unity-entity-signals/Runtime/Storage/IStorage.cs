using System;
using EntitySignals.Context;

namespace EntitySignals.Storage {
    public interface IStorage {
        int Count { get; }

        IContext<object> On();
        IContext<TEntity> On<TEntity>(TEntity entity);
        IContext<TEntity> On<TEntity>(Predicate<TEntity> entity);
        
        void Dispose();
        void Dispose<TEntity>(TEntity entity);

        void Send<TEntity, TSignal>(TEntity entity, TSignal arg);
    }
}