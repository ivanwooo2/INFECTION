using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCheckManager : MonoBehaviour
{
    public static TutorialCheckManager instance;

    public bool TutorialDone = false;
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        
    }
}
