using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class CardPicker : MonoBehaviour
{
    private class Lockturn
    {
        public string id;
        public int lockturn;
        public Lockturn(string id, int lockturn)
        {
            this.id = id;
            this.lockturn = lockturn;
        }
        public bool IsLocked()
        {
            if (lockturn <= 0) return false;
            else return true;
        }
    }
    public GameManager gameManager;
    public CardDataContainer container;
    private int weightSum = 0;
    private Dictionary<string, List<CardData>> dict; // <cardName, CardData>
    private List<Lockturn> lockturnList;

    private void Awake()
    {
        dict = new Dictionary<string, List<CardData>>();
        Debug.Log(dict);
        lockturnList = new List<Lockturn>(container.allCardData.Count);
        // 딕셔너리에 <카드이름, 카드 데이터 리스트>를 추가함
        // 카드이름으로 카드들을 뽑을 수 있도록 하기 위해
        for(int i=0; i<container.allCardData.Count; i++)
        {
            CardData data = container.allCardData[i];
            if (dict.ContainsKey(data.cardName))
            {
                dict[data.cardName].Add(data);
                weightSum += data.weight;
            }
            else
            {
                List<CardData> list = new List<CardData>();
                list.Add(data);
                dict.Add(data.cardName, list);
                weightSum += data.weight;
            }

        }

    }
    public CardData PickRandomCard()
    {
        int rnd = UnityEngine.Random.Range(1, weightSum);
        CardData data = null;
        for(int i=0; i<container.allCardData.Count; i++)
        {
            data = container.allCardData[i];
            bool pickable = IsCardPickable(data);
            rnd -= data.weight;
            if (pickable && !data.isEvent)
            {
                if(rnd <= 0)
                {
                    Debug.Log("Picked Random Card is " + data.id + data.cardName);
                    return data;
                }
            }
            else
            {
            }
        }
        Debug.Log("Picked Random Card is " + data.id + data.cardName);
        return data;
    }
    // 같은 이름을 가진 카드 중 하나를 뽑음
    public CardData PickCard(string cardName)
    {
        CardData ret = null;
        List<CardData> cards = dict[cardName];
        // 이름을 가진 카드가 하나일때
        if(cards.Count == 1)
        {
            bool pickable = IsCardPickable(cards[0]);
            if (pickable)
                return cards[0];
            else
            {
                Debug.LogWarning(cardName + "을 가진 카드를 뽑을 수 없음. 랜덤으로 카드를 뽑음");
                return PickRandomCard();
            }
        }

        int rnd= UnityEngine.Random.Range(0, weightSum);
        for(int i=0; i<cards.Count; i++)
        {
            bool pickable = IsCardPickable(cards[i]);
            if (pickable && cards[i].isEvent)
            {
                rnd -= cards[i].weight;
                if(rnd <= 0)
                {
                    ret = cards[i];
                    return cards[i];
                }
            }
        }
        Debug.Log("Picked Card is " + ret.id + ret.cardName);
        return ret;
    }
    public bool IsCardPickable(CardData data)
    {
        bool condition = IsConditionSatisfy(data);
        bool lockturn = IsCardLocked(data);

        if (condition && !lockturn)
            return true;
        else
            return false;
    }
    public bool IsConditionSatisfy(CardData data)
    {
        // 조건이 있을경우
        if(data.condition.Length > 1)
        {
            bool condition = false;
            if (gameManager.IsConditionSatisfy(data.condition))
                condition = true;
            else
                condition = false;
            return condition;
        }
        // 없을경우 상관없으므로 참 반환
        else
        {
            return true;
        }
    }
    private bool IsCardLocked(CardData data)
    {
        for (int i = 0; i < lockturnList.Count; i++)
        {
            if (lockturnList[i].id.Equals(data.id))
            {
                if (lockturnList[i].lockturn <= 0)
                    return false;
                else
                    return true;
            }
        }
        return false;
    }

    public void TickLockturn()
    {
        for(int i=0; i<lockturnList.Count; i++)
        {
            lockturnList[i].lockturn--;
            if(lockturnList[i].lockturn <= 0)
            {
                lockturnList.RemoveAt(i);
            }
        }
    }

    public void StartLockturn(string id, int lockturn)
    {
        lockturnList.Add(new Lockturn(id, lockturn));
    }

}
