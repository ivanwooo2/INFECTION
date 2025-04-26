using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] GameObject mainmenu;
    [SerializeField] GameObject settingPanel;
    [SerializeField] Slider volumeSlider;
    [SerializeField] Toggle fullscreenToggle;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    void LoadSetting()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
    }
    public void Play()
    {
        SceneManager.LoadScene("CharacterSelect");
    }

    public void Setting()
    {
        mainmenu.SetActive(false);
        settingPanel.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Back()
    {
        mainmenu.SetActive(true);
        settingPanel.SetActive(false);
        PlayerPrefs.Save();
    }

    public void onSetVolume(float vol)
    {
        AudioListener.volume = vol;
        PlayerPrefs.SetFloat("MasterVolume", vol);
    }

    public void onSetFullScreen(bool isFull)
    {
        Screen.fullScreen = isFull;
        PlayerPrefs.SetInt("Fullscreen", isFull ? 1 : 0);
    }
}
