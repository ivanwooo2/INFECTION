using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ResultScene : MonoBehaviour
{
    [SerializeField] private TMP_Text resultTimeText;

    private void Start()
    {
        if (TimeManager.Instance != null)
        {
            float remainingTime = TimeManager.Instance.CurrentTime;
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);
            resultTimeText.text = $"{minutes:00}:{seconds:00}";

            Destroy(TimeManager.Instance.gameObject);
        }
    }

    public void RestartGame()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.ResetTimer();
        }
        SceneManager.LoadScene("MainGame");
    }

    public void Title()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
