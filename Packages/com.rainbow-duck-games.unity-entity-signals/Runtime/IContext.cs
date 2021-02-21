namespace EntitySignals {
    public interface IContext<out TEntity> {
        int Count { get; }

        void Add(object receiver);
        void Add<TSignal>(ESHandler<TSignal> signalHandler);
        void Add<TSignal>(ESHandler<TEntity, TSignal> signalHandler);

        void Remove(object receiver);
        void Remove<TSignal>(ESHandler<TSignal> signalHandler);
        void Remove<TSignal>(ESHandler<TEntity, TSignal> signalHandler);

        void Send<TSignal>(TSignal arg);

        void Dispose();
    }

    public delegate void ESHandler<in TSignal>(TSignal signal);

    public delegate void ESHandler<in TEntity, in TSignal>(TEntity entity, TSignal signal);
}