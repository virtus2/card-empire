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
public class CardManager : MonoBehaviour
{
    public static string associatedSheet = "1qkHTR2x5pBfWMTmqkyjBzBA_HeA8AhOhnqN9lvqjlYk";
    public static string associatedWorksheet = "Sheet1";

    public CardDataContainer container;


    public bool updateOnPlay;

    void Awake()
    {
        if (updateOnPlay)
        {
            UpdateStats();
        }
    }

    void UpdateStats()
    {
        SpreadsheetManager.Read(new GSTU_Search(associatedSheet, associatedWorksheet), UpdateAllCards);
    }

    void UpdateAllCards(GstuSpreadSheet ss)
    {
        foreach(CardData data in container.allCardData)
        {
            data.UpdateData(ss, data.id);
        }
    }
}
