using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeBarController : MonoBehaviour
{
    [SerializeField] private float totalTime;
    private RectTransform rectTransform;
    private float startY;
    private float endY;
    private float Timer;
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        InitializePosition();
    }

    // Update is called once per frame
    void Update()
    {
        if (Timer < totalTime)
        {
            Timer += Time.deltaTime;
            UpdateBarPosition();
        }
    }

    private void InitializePosition()
    {
        startY = 0;
        endY = -Screen.height + rectTransform.rect.height;
    }

    private void UpdateBarPosition()
    {
        float progress = Mathf.Clamp01(Timer / totalTime);
        float newY = Mathf.Lerp(startY, endY, progress);
        rectTransform.anchoredPosition = new Vector2(0, newY);
    }
}
