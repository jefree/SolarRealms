using UnityEngine;
using System.Collections.Generic;

public enum EffectColor
{
    COMBAT,
    AUTHORITY,
    TRADE
}

public class EffectUp : MonoBehaviour
{
    TMPro.TextMeshPro textMeshPro;
    Queue<(EffectColor, float)> queue = new();
    float time = 0;

    void Start()
    {
        textMeshPro = GetComponent<TMPro.TextMeshPro>();
    }

    void Update()
    {
        if (time > 0)
        {
            time -= Time.deltaTime;
            transform.Translate(0, 0.01f, 0);
        }
        else
        {
            StartNext();
        }
    }

    public void Enqueue(EffectColor color, float value)
    {
        queue.Enqueue((color, value));
        gameObject.SetActive(true);
    }

    void StartNext()
    {
        if (queue.Count == 0)
        {
            time = 0;
            gameObject.SetActive(false);
            return;
        }

        var (color, value) = queue.Dequeue();
        time = 0.4f;

        transform.localPosition = new(0, 2.14f, 0);
        textMeshPro.text = $"{value}";
        textMeshPro.color = color switch
        {
            EffectColor.COMBAT => Color.red,
            EffectColor.AUTHORITY => Color.green,
            EffectColor.TRADE => Color.yellow
        };
    }
}