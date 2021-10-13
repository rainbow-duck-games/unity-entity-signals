using System.Collections;
using RainbowDuckGames.UnityEntitySignals;
using UnityEngine;

public class TestEmitter : MonoBehaviour {
    public float TimeFrom = 1;
    public float TimeTo = 5;

    void Start() {
        StartCoroutine(RandomColor());
    }

    public IEnumerator RandomColor() {
        while (true) {
            var color = new Color(Random.value, Random.value, Random.value);
            ES.Global.Send(new ColorSignal(color));
            yield return new WaitForSeconds(Random.Range(TimeFrom, TimeTo));
        }
    }

    public class ColorSignal {
        public readonly Color Color;

        public ColorSignal(Color color) {
            Color = color;
        }
    }
}