using RainbowDuckGames.UnityEntitySignals;
using RainbowDuckGames.UnityEntitySignals.Handlers;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TestCatcher : MonoBehaviour {
    void OnEnable() {
        ES.Global.Add(this);
    }

    void OnDisable() {
        ES.Global.Remove(this);
    }

    [SignalHandler]
    public void OnSignal(TestEmitter.ColorSignal signal) {
        GetComponent<SpriteRenderer>().color = signal.Color;
    }
}