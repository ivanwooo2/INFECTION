using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialCheckerTextBoxManager : MonoBehaviour
{
    private TutorialCheckManager TutorialCheckManager;
    private TMP_Text TutorialCheck;
    void Start()
    {
        TutorialCheckManager = FindObjectOfType<TutorialCheckManager>();
        TutorialCheck = GetComponent<TMP_Text>();
    }

    public void Switch()
    {
        if (TutorialCheckManager.TutorialDone == true)
        {
            TutorialCheckManager.TutorialDone = false;
        }
        else if (TutorialCheckManager.TutorialDone == false)
        {
            TutorialCheckManager.TutorialDone = true;
        }
    }

    void Update()
    {
        if (TutorialCheckManager.TutorialDone == true)
        {
            TutorialCheck.text = "Tutorial Status:Done";
        }
        else if (TutorialCheckManager.TutorialDone == false)
        {
            TutorialCheck.text = "Tutorial Status:unDone";
        }
    }
}
