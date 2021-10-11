using System;
using Homebrew;
using JetBrains.Annotations;
using NUnit.Framework;

namespace RainbowDuckGames.UnityEntitySignals.Tests.Editor.Performance {
    [UsedImplicitly]
    public class SubscribeSendUnsubscribe : AbstractPerfTest {
        protected override Action PrepareSignals(Signals signals, Recorder receiver) {
            return () => {
                signals.Add(receiver);
                signals.Send(1);
                signals.Remove(receiver);
            };
        }

        protected override Action Prepare(EntitySignals es, Recorder receiver) {
            return () => {
                es.On(Entity).Add(receiver);
                es.On(Entity).Send(1);
                es.On(Entity).Remove(receiver);
            };
        }
    }
}