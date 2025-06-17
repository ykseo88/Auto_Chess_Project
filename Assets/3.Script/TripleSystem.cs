using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleSystem : MonoBehaviour
{
    public Hand hand;
    public Field field;
    public Shop shop;
    public Choice choice;
    
    public int tripleCount = 3;
    
    public SAOCardDatabase cardDatabase;
    
    private Dictionary<string, int> fieldCardCounts = new Dictionary<string, int>();
    
    public void UpdateFieldCardCounts()
    {
        fieldCardCounts.Clear();
        for (int i = 0; i < field.fieldCards.Count; i++)
        {
            field.fieldCards[i].transform.TryGetComponent(out Card card);
            if(card != null && !card.currentCardData.Golden)
            {
                if (fieldCardCounts.ContainsKey(card.currentCardData.Name))
                {
                    fieldCardCounts[card.currentCardData.Name]++;
                }
                else
                {
                    fieldCardCounts.Add(card.currentCardData.Name, 1);
                }
            }
        }
    }

    public void CheckTriple()
    {
        UpdateFieldCardCounts();
        
        foreach (var entry in fieldCardCounts)
        {
            if (entry.Value >= tripleCount)
            {
                Debug.Log($"트리플 발생! 카드 이름: {entry.Key}, 수량: {entry.Value}");
                List<Card> tripleCards = new List<Card>();
                List<Card> removeCards = new List<Card>();
                foreach (GameObject cardObj in field.fieldCards)
                {
                    cardObj.transform.TryGetComponent(out Card card);
                    if (!card.currentCardData.Golden && card.currentCardData.Name == entry.Key)
                    {
                        tripleCards.Add(card);
                    }
                }
                
                Card smallestCard = tripleCards[0];

                for (int i = 0; i < tripleCards.Count; i++)
                {
                    if (tripleCards[i].AllAmount <= smallestCard.AllAmount)
                    {
                        smallestCard = tripleCards[i];
                    }
                }
                
                tripleCards.Remove(smallestCard);
                field.fieldCards.Remove(smallestCard.gameObject);
                smallestCard.inventorys.currentNum++;
                Destroy(smallestCard.gameObject);
                
                
                Dictionary<string, SAOCardDatabase.UnitElement> goldenCardUnitsDic = new Dictionary<string, SAOCardDatabase.UnitElement>();
                List<SAOCardDatabase.UnitElement> goldenCardUnits = new List<SAOCardDatabase.UnitElement>();

                for (int i = 0; i < tripleCards.Count; i++)
                {
                    for (int j = 0; j < tripleCards[i].currentCardData.Units.Count; j++)
                    {
                        if (goldenCardUnitsDic.ContainsKey(tripleCards[i].currentCardData.Units[j].UnitName))
                        {
                            goldenCardUnitsDic[tripleCards[i].currentCardData.Units[j].UnitName].UnitAmount +=
                                tripleCards[i].currentCardData.Units[j].UnitAmount;
                        }
                        else
                        {
                            goldenCardUnitsDic.Add(tripleCards[i].currentCardData.Units[j].UnitName, new SAOCardDatabase.UnitElement());
                            goldenCardUnitsDic[tripleCards[i].currentCardData.Units[j].UnitName].UnitName = tripleCards[i].currentCardData.Units[j].UnitName;
                            goldenCardUnitsDic[tripleCards[i].currentCardData.Units[j].UnitName].UnitAmount = tripleCards[i].currentCardData.Units[j].UnitAmount;
                            goldenCardUnitsDic[tripleCards[i].currentCardData.Units[j].UnitName].Unit = tripleCards[i].currentCardData.Units[j].Unit;
                        }
                    }
                }

                foreach (var dics in goldenCardUnitsDic)
                {
                    SAOCardDatabase.UnitElement tempUnitElement = new SAOCardDatabase.UnitElement();
                    tempUnitElement.UnitName = dics.Key;
                    tempUnitElement.UnitAmount = dics.Value.UnitAmount;
                    tempUnitElement.Unit = dics.Value.Unit;
                    goldenCardUnits.Add(tempUnitElement);
                }
                
                GameObject goldenCardObj = Instantiate(shop.cardPrefab, field.transform);
                goldenCardObj.transform.TryGetComponent(out Card goldenCard);
                goldenCard.InsertCard(tripleCards[0].currentCardData);
                goldenCard.parentList = field.fieldCards;
                goldenCard.currentCardData.Golden = true;
                goldenCard.currentCardData.Units.Clear();
                goldenCard.currentCardData.Units.AddRange(goldenCardUnits);
                goldenCard.UpdateCardInfo();

                for (int i = 0; i < tripleCards.Count; i++)
                {
                    field.fieldCards.Remove(tripleCards[i].gameObject);
                    tripleCards[i].inventorys.currentNum++;
                    Destroy(tripleCards[i].gameObject);
                }
                
                choice.OnChoice(shop.shopRank + 1);
                
            }
        }
    }

    public void Update()
    {
        DebugLogFieldCardCounts();
    }
    
    private void DebugLogFieldCardCounts()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            Debug.Log("현재 필드의 카드 수");
            foreach (string Keys in fieldCardCounts.Keys)
            {
                Debug.Log($"{Keys} : {fieldCardCounts[Keys]}");
            }
        }
    }
}
