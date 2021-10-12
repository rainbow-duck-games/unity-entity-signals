using System;
using RainbowDuckGames.UnityEntitySignals.Utility.Tact;

namespace RainbowDuckGames.UnityEntitySignals {
    public sealed class HandlerDelegate {
        private readonly Type _signalType;
        private readonly EfficientInvoker _invoker;
        private readonly WeakReference<object> _weakReference;
        private readonly int _args;

        public HandlerDelegate(Type signalType, EfficientInvoker invoker, object receiver, int args = 2) {
            _signalType = signalType;
            _invoker = invoker;
            _weakReference = new WeakReference<object>(receiver);
            _args = args;
        }

        public bool Valid<TEntity, TSignal>(TEntity entity, TSignal signal) {
            return _signalType == signal.GetType();
        }

        public void Invoke<TEntity, TSignal>(TEntity entity, TSignal signal) {
            if (!IsAlive(out var receiver)) {
                return;
            }
            
            switch (_args) {
                case 1:
                    _invoker.Invoke(receiver, signal);
                    break;
                case 2:
                    _invoker.Invoke(receiver, entity, signal);
                    break;
            }
        }

        public bool IsAlive(out object receiver) {
            return _weakReference.TryGetTarget(out receiver);
        }

        public bool ReceiverEquals(object candidate) {
            return IsAlive(out var receiver) && ReferenceEquals(receiver, candidate);
        }
    }
}