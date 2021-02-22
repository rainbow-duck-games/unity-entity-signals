using System;
using RainbowDuckGames.UnityEntitySignals.Utility.Tact;

namespace RainbowDuckGames.UnityEntitySignals {
    public class HandlerDelegate {
        public readonly Type SignalType;
        public readonly EfficientInvoker Invoker;
        public readonly object Receiver;
        public readonly int Args;
        public Predicate<object> EntityPredicate = obj => true; // ToDo Support it?

        public HandlerDelegate(Type signalType, EfficientInvoker invoker, object receiver, int args = 2) {
            SignalType = signalType;
            Invoker = invoker;
            Receiver = receiver;
            Args = args;
        }

        public bool Valid(object entity, object signal) {
            return SignalType == signal.GetType() && EntityPredicate.Invoke(entity);
        }
    }
}