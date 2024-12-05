using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class element : MonoBehaviour
{
    public float weight;
    public bool symbol;
    public Vector2 defaultSize;
    public Vector2 startPos;
    public bool used;

    private void Start()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        defaultSize = rectTransform.sizeDelta;
        startPos = rectTransform.anchoredPosition;
    }
}
