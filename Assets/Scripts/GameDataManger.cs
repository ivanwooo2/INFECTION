using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManger : MonoBehaviour
{
    private const string CHAR_KEY = "SelectedCharacter";
    private const string LEVEL_KEY = "SelectedLevel";

    public static void SaveCharacter(CharacterData characterdata)
    {
        PlayerPrefs.SetString(CHAR_KEY, characterdata.name);
    }

    public static CharacterData LoadCharacter()
    {
        string path = "Character/" + PlayerPrefs.GetString(CHAR_KEY);
        return Resources.Load<CharacterData>(path);
    }

    public static void SaveLevel(LevelData leveldata)
    {
        PlayerPrefs.SetString(LEVEL_KEY, leveldata.name);
    }

    public static LevelData LoadLevel()
    {
        string path = "Level/" + PlayerPrefs.GetString(LEVEL_KEY);
        return Resources.Load<LevelData>(path);
    }
}
