using System;
using Homebrew;
using JetBrains.Annotations;

namespace EntitySignals.Tests.Editor.Performance {
        [UsedImplicitly]
        public class SendOnly : AbstractPerfTest {
            protected override Action PrepareSignals(Signals signals, Func<Recorder> receiverPrepare) {
                var receiver = receiverPrepare.Invoke();
                signals.Add(receiver);
                return () => { signals.Send(1); };
            }

            protected override Action Prepare(EntitySignals es, Func<Recorder> receiverPrepare) {
                var receiver = receiverPrepare.Invoke();
                es.On(Entity).Add(receiver);
                return () => { es.On(Entity).Send(1); };
            }
        }
}