using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    [Serializable]
    public class CustomSprite
    {
        public string spriteName;
        public Sprite sprite;
    }

    public static SpriteManager Instance;
    public List<CustomSprite> spriteList;
    private Dictionary<string, Sprite> spriteDict;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        spriteDict = new Dictionary<string, Sprite>(spriteList.Count);
        for(int i=0; i<spriteList.Count; i++)
        {
            spriteDict.Add(spriteList[i].spriteName, spriteList[i].sprite);
        }
    }

    public Sprite GetSpriteByName(string spriteName)
    {
        if (spriteDict.ContainsKey(spriteName))
            return spriteDict[spriteName];
        else
        {
            Debug.LogError(spriteName + "- sprite 가 없음");
            return null;

        }
    }
}
