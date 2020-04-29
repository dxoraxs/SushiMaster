using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private RectTransform whiteLine;
    [SerializeField] private Text textProcent;
    private float maxSize;

    private void Start()
    {
        maxSize = whiteLine.sizeDelta.x;
        whiteLine.sizeDelta = new Vector2(0, whiteLine.sizeDelta.y);
        EventManager.OnUpdateProgressBar += UpdateProgressBar;
    }

    private void OnDestroy()
    {
        EventManager.OnUpdateProgressBar -= UpdateProgressBar;
    }

    private void UpdateProgressBar(float procent)
    {
        whiteLine.sizeDelta = new Vector2(procent * maxSize, whiteLine.sizeDelta.y);
        textProcent.text = (int)(procent*100) + "%";
    }
}
