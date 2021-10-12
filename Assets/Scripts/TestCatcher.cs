using DefaultNamespace;
using RainbowDuckGames.UnityEntitySignals;
using RainbowDuckGames.UnityEntitySignals.Handlers;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TestCatcher : MonoBehaviour {
    void OnEnable() {
        Global.Signals.On().Add((ESHandler<TestEmitter.ColorSignal>) OnSignal);
    }

    void OnDisable() {
        Global.Signals.On().Remove(this);
    }

    [SignalHandler]
    public void OnSignal(TestEmitter.ColorSignal signal) {
        GetComponent<SpriteRenderer>().color = signal.Color;
    }
}