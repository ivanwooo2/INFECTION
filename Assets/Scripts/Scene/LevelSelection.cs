using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class LevelSelection : MonoBehaviour
{
    [SerializeField] LevelData[] level;
    [SerializeField] Transform buttonContainer;
    [SerializeField] GameObject buttonPrefab;
    [SerializeField] float buttonSpacing;
    [SerializeField] float selectScale;
    [SerializeField] float tweenDuration;

    GameObject[] buttons;
    int selectedIndex = -1;
    // Start is called before the first frame update
    void Start()
    {

    }

    void InitializeButtons()
    {
        buttons = new GameObject[level.Length];
        float totalWidth = (level.Length - 1) * buttonSpacing;
        float startX = -totalWidth / 2;

        for (int i = 0; i < level.Length; i++)
        {
            GameObject btn = Instantiate(buttonPrefab, buttonContainer);
            RectTransform rt = btn.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(startX + i * buttonSpacing, 0);

            btn.GetComponent<Image>().sprite = level[i].thumbnail;
            int index = i;
            btn.GetComponent<Button>().onClick.AddListener(() => SelectLevel(index));

            buttons[i] = btn;
        }
    }

    void LoadPreviousSelection()
    {
        LevelData prev = GameDataManger.LoadLevel();
        if (prev != null)
        {
            int index = System.Array.IndexOf(level, prev);
            if (index >= 0) SelectLevel(index);
        }
    }

    void SelectLevel(int index)
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

        GameDataManger.SaveLevel(level[index]);
    }

    public void StartGame()
    {
        if (selectedIndex == -1) return;
        SceneManager.LoadScene("MainGame");
        //StartCoroutine(LoadGameAsync());
    }

    public void BacktoCharacterSelect()
    {
        SceneManager.LoadScene("CharacterSelectScene");
    }

    //IEnumerator LoadGameAsync()
    //{
    //AsyncOperation op = SceneManager.LoadSceneAsync("LoadingScene");
    //op.allowSceneActivation = false;

    //while(op.progress< 0.9f){
    //yield return null;
    //}
    //op.allowSceneActivation = true;
    //}


}
