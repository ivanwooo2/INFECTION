using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneLoader : MonoBehaviour
{
    [SerializeField] SpriteRenderer characterSprite;
    // Start is called before the first frame update
    void Start()
    {
        CharacterData charData = GameDataManger.LoadCharacter();

        characterSprite.sprite = charData.icon;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
