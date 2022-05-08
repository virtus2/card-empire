using UnityEngine;
using System.Collections;
using GoogleSheetsToUnity;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using GoogleSheetsToUnity.ThirdPary;
using System.Runtime.ExceptionServices;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class CardDataContainer : ScriptableObject
{
    public List<CardData> allCardData;

}

[CustomEditor(typeof(CardDataContainer))]
public class CardDataContainerEditor: Editor
{
    CardDataContainer container;
    private void OnEnable()
    {
        container = (CardDataContainer)target;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Label("Read All Data");

        if (GUILayout.Button("Pull Data"))
        {
            UpdateData(UpdateMethodOne);
        }
    }

    void UpdateData(UnityAction<GstuSpreadSheet> callback, bool mergedCells = false)
    {
        SpreadsheetManager.Read(new GSTU_Search(CardManager.associatedSheet, CardManager.associatedWorksheet), callback, mergedCells);
    }

    void UpdateMethodOne(GstuSpreadSheet ss)
    {
        container.allCardData.Clear();
        for (int i = 0; i < ss.columns["A"].Count - 1; i++)
        {
            string id = i.ToString();
            CardData newData = CreateInstance<CardData>();
            newData.id = ss[id, "id"].value;
            newData.cardName = ss[id, "cardName"].value;
            newData.teller = ss[id, "teller"].value;
            newData.lockturn = int.Parse(ss[id, "lockturn"].value);
            newData.weight = int.Parse(ss[id, "weight"].value);
            newData.isEvent = bool.Parse(ss[id, "isEvent"].value);
            newData.condition = ss[id, "condition"].value;
            newData.nextCardNameYes = ss[id, "nextCardNameYes"].value;
            newData.nextCardNameNo = ss[id, "nextCardNameNo"].value;
            newData.yesCondition = ss[id, "yesCondition"].value;
            newData.noCondition = ss[id, "noCondition"].value;
            newData.textKor = ss[id, "textKor"].value;
            newData.answerYesKor = ss[id, "answerYesKor"].value;
            newData.answerNoKor = ss[id, "answerNoKor"].value;



            AssetDatabase.CreateAsset(newData, "Assets/Editor/" + newData.id + "-" + newData.cardName + ".asset");
            
            container.allCardData.Add(newData);
            AssetDatabase.Refresh();
        }
        Debug.Log("All Card Data Pulled");
        EditorUtility.SetDirty(target);
    }

}