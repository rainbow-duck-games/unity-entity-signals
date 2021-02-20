﻿using EntitySignals.Context;

namespace EntitySignals {
    public static class ES {
        private static readonly EntitySignals Default = new EntitySignals();

        public static int Count => Default.Count;
        
        public static IContext<object> Global => Default.On();

        public static IContext<TEntity> On<TEntity>(TEntity entity) {
            return Default.On(entity);
        }

        public static void Dispose() {
            Default.Dispose();
        }
    }
}