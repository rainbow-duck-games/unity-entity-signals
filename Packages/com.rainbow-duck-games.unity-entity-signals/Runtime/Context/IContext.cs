﻿namespace EntitySignals.Context {
    public interface IContext<out TEntity> {
        int Count { get; }
        
        void Add(object receiver);
        void Add<TSignal>(ESHandler<TSignal> signalHandler);
        void Add<TSignal>(ESHandler<TEntity, TSignal> signalHandler);
        
        void Remove(object receiver);
        void Remove<TSignal>(ESHandler<TEntity> signalHandler);
        void Remove<TSignal>(ESHandler<TEntity, TSignal> signalHandler);
        
        void Send<TSignal>(TSignal arg);
        
        void Dispose();
    }
}