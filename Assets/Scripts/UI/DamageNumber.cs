using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    public float fadeSpeed = 1f;
    public float moveDistance = 1f;
    public float positionRandomness = 0.2f;
    public float number;
    private float alpha = 1f;
    private TextMeshPro text;
    private Vector3 initialPos;

    private void Start()
    {
        text = GetComponent<TextMeshPro>();
        text.text = number.ToString();
        initialPos = transform.position + new Vector3(Random.Range(-positionRandomness, positionRandomness), Random.Range(-positionRandomness, positionRandomness));
    }

    void Update()
    {
        alpha -= Time.deltaTime * fadeSpeed;

        if (alpha < 0f)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = initialPos + (new Vector3(0, moveDistance) * (1f - alpha));
        var color = text.faceColor;
        text.faceColor = new Color32(color.r, color.g, color.b, (byte)(alpha * 255f));
    }
}
