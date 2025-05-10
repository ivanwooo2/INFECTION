using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    [SerializeField] private float totalTime = 300f;
    private float currentTime;
    public bool isGameOver = false;
    [SerializeField] private TMP_Text timerText;

    public float CurrentTime => currentTime;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        currentTime = totalTime;
    }

    private void Update()
    {
        if (!isGameOver && SceneManager.GetActiveScene().name != "ResultScene")
        {
            currentTime -= Time.deltaTime;
            UpdateTimerDisplay();

            if (currentTime <= 0)
            {
                currentTime = 0;
                LoadResultScene();
            }
        }
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void LoadResultScene()
    {
        isGameOver = true;
        SceneManager.LoadScene("ResultScene");
    }

    public void ResetTimer()
    {
        currentTime = totalTime;
        isGameOver = false;
    }
}
