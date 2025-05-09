using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterSelectScene : MonoBehaviour
{
    [SerializeField] CharacterData[] characters;
    [SerializeField] Transform buttonContainer;
    [SerializeField] GameObject buttonPrefab;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] float buttonSpacing;
    [SerializeField] float selectScale;
    [SerializeField] float tweenDuration;

    GameObject[] buttons;
    int selectedIndex = -1;
    // Start is called before the first frame update
    void Start()
    {
        DG.Tweening.DOTween.Init();
        InitializeButtons();
        LoadPreviousSelection();
    }

    void InitializeButtons()
    {
        buttons = new GameObject[characters.Length];
        float totalWidth = (characters.Length - 1) * buttonSpacing;
        float startX = -totalWidth / 2;

        for (int i = 0; i < characters.Length; i++)
        {
            GameObject bth = Instantiate(buttonPrefab, buttonContainer);
            RectTransform rt = bth.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2 (startX + i * buttonSpacing, 0);
            bth.GetComponent<Image>().sprite = characters[i].icon;
            int index = i;
            bth.GetComponent<Button>().onClick.AddListener(() => SelectCharacter(index));
            buttons[i] = bth;
        }
    }

    void LoadPreviousSelection()
    {
        CharacterData prev = GameDataManger.LoadCharacter();
        if (prev != null)
        {
            int index = System.Array.IndexOf(characters, prev);
            if (index >= 0) SelectCharacter(index);
        }
    }

    void SelectCharacter(int index)
    {
        if (selectedIndex == index) return;

        if (selectedIndex != -1)
        {
            buttons[selectedIndex].transform.DOKill();
            buttons[selectedIndex].transform.DOScale(Vector3.one, tweenDuration);
        }

        selectedIndex = index;
        buttons[index].transform.DOScale(Vector3.one * selectScale, tweenDuration)
            .SetEase(Ease.OutBack);

        descriptionText.DOFade(0, tweenDuration / 2).OnComplete(() =>
        {
            descriptionText.text = characters[index].skillDescription;
            descriptionText.DOFade(1, tweenDuration / 2);
        });

        GameDataManger.SaveCharacter(characters[index]);
    }

    public void GoToLevelSelect()
    {
        if(selectedIndex == -1) return;
        SceneManager.LoadScene("LevelSelectScene");
    }

    public void GoBack()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
