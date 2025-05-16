using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultImageController : MonoBehaviour
{
    [System.Serializable]
    public class TimeBasedImage
    {
        public float minTime;
        public Sprite displaySprite;
    }

    [SerializeField] private Image targetImage;
    [SerializeField] private TimeBasedImage[] timeImages;

    [SerializeField] private Sprite defaultSprite;

    private void Start()
    {
        float remainingTime = TimeManager.LastRemainingTime;
        Sprite selectedSprite = defaultSprite;

        foreach (var timeImage in timeImages)
        {
            if (remainingTime >= timeImage.minTime)
            {
                selectedSprite = timeImage.displaySprite;
                break;
            }
        }

        targetImage.sprite = selectedSprite;
        targetImage.preserveAspect = true;

        if (TimeManager.Instance != null)
        {
            Destroy(TimeManager.Instance.gameObject);
        }
    }
}
