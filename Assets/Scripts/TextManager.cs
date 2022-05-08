using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextManager : MonoBehaviour
{

    [Serializable]
    public class CustomText
    {
        public string textName;
        public string text;
    }

    public static TextManager Instance;
    public List<CustomText> textList;
    private Dictionary<string, string> textDict;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        textDict = new Dictionary<string, string>(textList.Count);
        for (int i = 0; i < textList.Count; i++)
        {
            textDict.Add(textList[i].textName, textList[i].text);
        }
    }

    public string GetTextByName(string textName)
    {
        if (textDict.ContainsKey(textName))
            return textDict[textName];
        else
        {
            Debug.LogError(textName + "- text 가 없음");
            return null;

        }
    }
}
