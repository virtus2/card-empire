using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Image cardImage;
    public TextMeshProUGUI text;
    public TextMeshProUGUI leftText;
    public TextMeshProUGUI rightText;
    public CardController cardController;
    public CardPicker cardPicker;

    public CardData currentCardData;
    private CardData preCardData;
    private Dictionary<string, bool> conditions;

    private void Start()
    {

        SetConditions();
        GameStart();
    }

    private void GameStart()
    {
        currentCardData = cardPicker.PickCard("gameStart");
        cardController.UpdateMovingCardData(currentCardData);
    }

    private void SetConditions()
    {
        conditions = new Dictionary<string, bool>();
        conditions.Add("gameStarted", false);
        conditions.Add("tutorial", true);

    }
    // 조건이 만족하면 true, 아니면 false를 반환
    public bool IsConditionSatisfy(string condition)
    {
        string s = condition.TrimStart('!');
        // 거짓이여야 만족하는 조건일경우
        if (condition.Contains("!"))
        {
            // 해당 조건이 거짓일때 참을 반환
            if (conditions[s] == false)
                return true;
            else
                return false;
        }
        // 참이여야 만족하는 조건일 경우
        else
        {
            // 해당 조건이 거짓일때 거짓을 반환
            if (conditions[s] == false)
                return false;
            else
                return true;
        }
    }
    public void OnLeftSwipe()
    {
        // lockturn 설정
        if(currentCardData.lockturn > 0)
        {
            cardPicker.StartLockturn(currentCardData.id, currentCardData.lockturn);
        }
        // 조건 설정
        if (currentCardData.yesCondition.Length > 1)
        {
            string s = currentCardData.yesCondition.TrimStart('!');
            if (currentCardData.yesCondition.Contains("!"))
                conditions[s] = false;
            else
                conditions[s] = true;
            Debug.Log(s + ": " + conditions[s]);
        }
        // 다음 카드 설정
        if(currentCardData.nextCardNameYes.Length > 1)
        {
            Debug.Log("nextCardNameYes: " + currentCardData.nextCardNameYes);
            currentCardData = cardPicker.PickCard(currentCardData.nextCardNameYes);
        }
        else
        {
            currentCardData = cardPicker.PickRandomCard();
        }
        cardController.StartCardUpdate(currentCardData);
        cardPicker.TickLockturn();
    }
    
    public void OnRightSwipe()
    {
        // lockturn 설정
        if (currentCardData.lockturn > 0)
        {
            cardPicker.StartLockturn(currentCardData.id, currentCardData.lockturn);
        }
        // 조건 설정
        if (currentCardData.noCondition.Length > 1)
        {
            string s = currentCardData.noCondition.TrimStart('!');
            if (currentCardData.noCondition.Contains("!"))
                conditions[s] = false;
            else
                conditions[s] = true;
            Debug.Log(s + ": " + conditions[s]);
        }
        // 다음 카드 설정
        if(currentCardData.nextCardNameNo.Length > 1)
        {
            Debug.Log("nextCardNameNo: " + currentCardData.nextCardNameNo);
            currentCardData = cardPicker.PickCard(currentCardData.nextCardNameNo);
        }
        else
        {
            currentCardData = cardPicker.PickRandomCard();
        }
        cardController.StartCardUpdate(currentCardData);
        cardPicker.TickLockturn();
    }
}
