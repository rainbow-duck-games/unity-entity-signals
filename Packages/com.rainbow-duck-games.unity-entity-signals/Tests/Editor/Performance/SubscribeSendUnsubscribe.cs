using System;
using Homebrew;
using JetBrains.Annotations;

namespace RainbowDuckGames.UnityEntitySignals.Tests.Editor.Performance {
    [UsedImplicitly]
    public class SubscribeSendUnsubscribe : AbstractPerfTest {
        protected override Action PrepareSignals(Signals signals, Func<Recorder> receiverPrepare) {
            return () => {
                var receiver = new SignalsTestReceiver();
                signals.Add(receiver);
                signals.Send(1);
                signals.Remove(receiver);
            };
        }

        protected override Action Prepare(EntitySignals es, Func<Recorder> receiverPrepare) {
            return () => {
                var receiver = receiverPrepare.Invoke();
                es.On(Entity).Add(receiver);
                es.On(Entity).Send(1);
                es.On(Entity).Remove(receiver);
            };
        }
    }
}