using System;
using Homebrew;
using JetBrains.Annotations;

namespace RainbowDuckGames.UnityEntitySignals.Tests.Editor.Performance {
        [UsedImplicitly]
        public class SendOnly : AbstractPerfTest {
            protected override Action PrepareSignals(Signals signals, Recorder receiver) {
                signals.Add(receiver);
                return () => { signals.Send(1); };
            }

            protected override Action Prepare(EntitySignals es, Recorder receiver) {
                es.On(Entity).Add(receiver);
                return () => { es.On(Entity).Send(1); };
            }
        }
}