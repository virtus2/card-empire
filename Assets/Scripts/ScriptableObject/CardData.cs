using UnityEngine;
using System.Collections;
using GoogleSheetsToUnity;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using GoogleSheetsToUnity.ThirdPary;

#if UNITY_EDITOR
using UnityEditor;
#endif
[CreateAssetMenu]
public class CardData : ScriptableObject
{
    public string id;
    public string cardName;
    public string teller;
    public int lockturn;
    public int weight;
    public bool isEvent;
    public string condition;
    public string nextCardNameYes;
    public string nextCardNameNo;
    public string yesCondition;
    public string noCondition;
    public string textKor;
    public string answerYesKor;
    public string answerNoKor;

    internal void UpdateData(GstuSpreadSheet ss, string id)
    {
        id = ss[id, "id"].value;
        cardName = ss[id, "cardName"].value;
        teller = ss[id, "teller"].value;
        lockturn = int.Parse(ss[id, "lockturn"].value);
        weight = int.Parse(ss[id, "weight"].value);
        isEvent = bool.Parse(ss[id, "isEvent"].value);
        condition = ss[id, "condition"].value;
        nextCardNameYes = ss[id, "nextCardNameYes"].value;
        nextCardNameNo = ss[id, "nextCardNameNo"].value;
        yesCondition = ss[id, "yesCondition"].value;
        noCondition = ss[id, "noCondition"].value;
        textKor = ss[id, "textKor"].value;
        answerYesKor = ss[id, "answerYesKor"].value;
        answerNoKor = ss[id, "answerNoKor"].value;
    }

}

[CustomEditor(typeof(CardData))]
public class CardEditor : Editor
{
    CardData cardData;

    void OnEnable()
    {
        cardData = (CardData)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Label("Read Data");

        if (GUILayout.Button("Pull Data Method One"))
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
        cardData.UpdateData(ss, cardData.id);
        EditorUtility.SetDirty(target);
    }

}