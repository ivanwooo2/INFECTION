using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (TimeManager.Instance != null)
        {
            Destroy(TimeManager.Instance.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Restart()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.ResetTimer();
        }
        int lastLevel = PlayerPrefs.GetInt("LastLevel");
        SceneManager.LoadScene(lastLevel);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
