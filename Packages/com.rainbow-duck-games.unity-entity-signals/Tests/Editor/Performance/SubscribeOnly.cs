using System;
using Homebrew;
using JetBrains.Annotations;

namespace EntitySignals.Tests.Editor.Performance {
    [UsedImplicitly]
    public class SubscribeOnly : AbstractPerfTest {
        protected override Action PrepareSignals(Signals signals, Func<Recorder> receiverPrepare) {
            return () => {
                var receiver = receiverPrepare.Invoke();
                signals.Add(receiver);
            };
        }

        protected override Action Prepare(EntitySignals es, Func<Recorder> receiverCreate) {
            return () => {
                var receiver = receiverCreate.Invoke();
                es.On(Entity).Add(receiver);
            };
        }
    }
}