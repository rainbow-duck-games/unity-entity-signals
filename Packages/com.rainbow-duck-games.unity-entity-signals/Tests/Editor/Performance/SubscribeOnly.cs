using System;
using Homebrew;
using JetBrains.Annotations;

namespace RainbowDuckGames.UnityEntitySignals.Tests.Editor.Performance {
    [UsedImplicitly]
    public class SubscribeOnly : AbstractPerfTest {
        protected override void Verify(EntitySignals entitySignals, Recorder recorder, int iterations) {
            entitySignals.On(Entity).Send(1);
            base.Verify(entitySignals, recorder, iterations);
        }

        protected override void Verify(Signals signals, Recorder recorder, int iterations) {
            signals.Send(1);
            base.Verify(signals, recorder, iterations);
        }

        protected override Action PrepareSignals(Signals signals, Recorder receiver) {
            return () => {
                signals.Add(receiver);
            };
        }

        protected override Action Prepare(EntitySignals es, Recorder receiver) {
            return () => {
                es.On(Entity).Add(receiver);
            };
        }
    }
}