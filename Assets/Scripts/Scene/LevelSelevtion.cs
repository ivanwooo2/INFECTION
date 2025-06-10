using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelevtion : MonoBehaviour
{
    [SerializeField] private bool isReturn;
    [SerializeField] Animator transition;
    [SerializeField] float transitionTime = 1f;

    private int selectedIndex = -1;
    private bool isLevelSelected = false;

    [SerializeField] private AudioSource AudioSource;
    [SerializeField] private AudioClip confiem, back;

    [Header("ステージデータ")]
    public LevelData[] levelDatas;

    [Header("UI")]
    public Button[] levelButtons;
    public Button[] levelBackGround;
    public TextMeshProUGUI levelName;
    public Button nextStageButton;
    void Start()
    {
        transition = FindAnyObjectByType<Animator>();
        Cursor.lockState = CursorLockMode.None;
        InitializeButtons();
        if (nextStageButton != null)
        {
            nextStageButton.interactable = false;
        }
    }

    private void InitializeButtons()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int index = i;
            levelButtons [i].onClick.AddListener(() => ShowLevelName(index));

            UpdateButtonAppearance(index);
        }
    }

    private void UpdateButtonAppearance(int index)
    {
        if (index < levelDatas.Length)
        {
            Image buttonBackGround = levelBackGround[index].GetComponent<Image>();
            Image buttonImage = levelButtons[index].GetComponent<Image>();
            buttonImage.sprite = levelDatas[index].isUnlocked ?
                levelDatas[index].unlockedSprite :
                levelDatas[index].lockedSprite;
            buttonBackGround.sprite = levelDatas[index].BackGround;
        }
    }

    public void ShowLevelName(int levelIndex)
    {
        if (levelIndex < levelDatas.Length)
        {
            levelName.text = levelDatas[levelIndex].isUnlocked ?
                $"{levelDatas[levelIndex].StageName}" :
                "LOCKED";
        }

        if (selectedIndex >= 0 && selectedIndex < levelButtons.Length)
        {
            levelButtons[selectedIndex].GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f);
            levelBackGround[selectedIndex].GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f);
        }
        AudioSource.clip = confiem;
        AudioSource.Play();
        levelButtons[levelIndex].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        levelBackGround[levelIndex].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        selectedIndex = levelIndex;

        bool isUnlocked = levelDatas[levelIndex].isUnlocked;
        isLevelSelected = isUnlocked;

        if (nextStageButton != null)
        {
            nextStageButton.interactable = isUnlocked;
            nextStageButton.GetComponent<Image>().color = isUnlocked ?
                new Color(1f, 1f, 1f, 1f) :
                new Color(1f, 1f, 1f, 0.1f);
        }
    }

    public void NextStageScene()
    {
        if (isLevelSelected && selectedIndex >= 0)
        {
            SelectionSceneBGMManager BGMmanager = FindObjectOfType<SelectionSceneBGMManager>();
            if (BGMmanager != null)
            {
                Destroy(BGMmanager.gameObject);
            }
            AudioSource.clip = confiem;
            AudioSource.Play();
            selectedIndex = PlayerPrefs.GetInt("SelectedLevelIndex");
            if (selectedIndex == 0)
            {
                StartCoroutine(GoNormal());
            }
            else if (selectedIndex == 1)
            {
                StartCoroutine(GoHard());
            }
        }
    }

    public void CharacterScene()
    {
        AudioSource.clip = back;
        AudioSource.Play();
        isReturn = true;
        StartCoroutine(Return());
    }

    IEnumerator Return()
    {
        transition.SetBool("Start", true);
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene("CharacterSelectScene");
    }

    IEnumerator GoNormal()
    {
        transition.SetBool("Start", true);
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene("Stage1normal");
    }

    IEnumerator GoHard()
    {
        transition.SetBool("Start", true);
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene("Stage1hard");
    }
}
