using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIShake : MonoBehaviour
{

    public Vector2 shakeDistance;

    public bool shaking = false;
    private Vector3 ogPos;
    private RectTransform rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        ogPos = rectTransform.anchoredPosition;
    }

    public void Shake() {
        rectTransform.anchoredPosition = new Vector2(ogPos.x + Random.Range(-shakeDistance.x, shakeDistance.x), ogPos.y + Random.Range(-shakeDistance.y, shakeDistance.y));
    }

    // Update is called once per frame
    void Update()
    {
        if (shaking)
            Shake();
    }
}
