using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance;
    
    public Dictionary<string, Action<GameObject>> abilityDictionary = new Dictionary<string, Action<GameObject>>();
    public SAOCardDatabase cardDatabase;
    public SAOUnitPrefabDatabase unitPrefabDatabase;
    public Field field;
    public Hand hand;
    public Shop shop;
    public ReadyTurn readyTurn;
    
    

    public void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InsertAbilityDictionary("SwordMan", SwordManAbility);
        InsertAbilityDictionary("FreshMan", FreshManAbility);
        InsertAbilityDictionary("SpearMan", SpearManAbility);
        InsertAbilityDictionary("BowMan", BowManAbility);
        InsertAbilityDictionary("Bachelor", BachelorAbility);
        InsertAbilityDictionary("Banneret", BanneretAbility);
        InsertAbilityDictionary("WarLord", WarLordAbility);
        InsertAbilityDictionary("Knight", KnightAbility);
    }

    private bool ContainNameList(List<SAOCardDatabase.UnitElement> list, string name)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].UnitName == name)
            {
                return true;
            }
        }
        
        return false;
    }
    
    private SAOCardDatabase.UnitElement GetUnitElementNameList(List<SAOCardDatabase.UnitElement> list, string name)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].UnitName == name)
            {
                return list[i];
            }
        }
        
        return null;
    }

    private bool ContainNameArray(SAOCardDatabase.UnitElement[] array, string name)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].UnitName == name)
            {
                return true;
            }
        }

        return false;
    }

    public void InsertAbilityDictionary(string CardName, Action<GameObject> abilityAction)
    {
        if (!abilityDictionary.ContainsKey(CardName))
        {
            abilityDictionary.Add(CardName, abilityAction);
        }
        else
        {
            Debug.LogWarning($"Ability {CardName} already exists in the dictionary.");
        }
    }
    
    public void OldSwordManAbility(GameObject cardObj)
    {
        // 5골드를 사용할 때마다 카드에 SwordMan 2기 추가
        
        cardObj.transform.TryGetComponent(out Card card);
        
        int plusUnitNum = 2;
        int checkGold = 5;
        string TargetUnitName = "SwordMan";
        if (card.currentCardData.Golden) plusUnitNum *= 2;
        
        if (card.intAbilityValue2 < 0)
        {
            card.intAbilityValue2 = field.consumedGold;
            card.intAbilityValue1 = 0;
        }
        
        card.intAbilityValue1 = field.consumedGold - card.intAbilityValue2;
        
        if(card.intAbilityValue1 >= checkGold)
        {
            Debug.Log("늘어남!");

            for (int j = 0; j < Mathf.FloorToInt(card.intAbilityValue1 / checkGold); j++)
            {
                for (int i = 0; i < card.currentCardData.Units.Count; i++)
                {
                    if (card.currentCardData.Units[i].UnitName == "SwordMan")
                    {
                        card.currentCardData.Units[i].UnitAmount += plusUnitNum;
                        break;
                    }
                }
            }

            int tempLeast = card.intAbilityValue1 % checkGold;
            int tempInt = card.intAbilityValue1 - tempLeast;
            card.intAbilityValue2 += tempInt;
            card.intAbilityValue1 = tempLeast;
            card.UpdateCardInfo();
        }
    }
    
    public void SwordManAbility(GameObject cardObj)
    {
        //턴이 종료될 때마다 SwordMan 2기 추가
        cardObj.transform.TryGetComponent(out Card cardObjCard);
        
        string unitName = "SpearMan";
        int plusUnitNum = 2;
        if(cardObjCard.currentCardData.Golden) plusUnitNum *= 2;
        
        if(cardObjCard.turnEnd)
        {
            Debug.Log("턴 종료!");
            cardObjCard.turnEnd = false;
            
            for (int i = 0; i < cardObjCard.currentCardData.Units.Count; i++)
            {
                if (cardObjCard.currentCardData.Units[i].UnitName == unitName)
                {
                    cardObjCard.currentCardData.Units[i].UnitAmount += plusUnitNum;
                    break;
                }
            }
        }
    }

    public void FreshManAbility(GameObject cardObj)
    {
        // 필드에 진입 시, 맨 왼쪽 SwordMan 카드에 SwordMan 2기 추가
        cardObj.transform.TryGetComponent(out Card cardObjCard);
        
        int plusUnitNum = 2;
        if (cardObjCard.currentCardData.Golden) plusUnitNum *= 2;

        if (!cardObjCard.boolAbilityValue)
        {
            cardObjCard.boolAbilityValue = true;
            for (int i = 0; i < field.fieldCards.Count; i++)
            {
                field.fieldCards[i].transform.TryGetComponent(out Card card);
                if (card.currentCardData.Name == "SwordMan")
                {
                    //card.currentCardData.Units.FirstOrDefault(c => c.UnitName == "SwordMan").UnitAmount += 2;
                    //card.UpdateCardInfo();

                    for (int j = 0; j < card.currentCardData.Units.Count; j++)
                    {
                        if (card.currentCardData.Units[j].UnitName == "SwordMan")
                        {
                            card.currentCardData.Units[j].UnitAmount += plusUnitNum;
                            card.UpdateCardInfo();
                            return;
                        }
                    }
                }
            }
        }

        
        
    }
    public void SpearManAbility(GameObject cardObj)
    {
        //턴이 종료될 때마다 SwordMan 2기를 SpearMan으로 변환
        cardObj.transform.TryGetComponent(out Card cardObjCard);
        Debug.Log("씨발 왜 안돼");
        
        string unitName = "SpearMan";
        string targetUnitName = "SwordMan";
        int changeUnitNum = 2;
        if(cardObjCard.currentCardData.Golden) changeUnitNum *= 2;
        
        if(cardObjCard.turnEnd)
        {
            Debug.Log("턴 종료!");
            cardObjCard.turnEnd = false;
            for (int i = 0; i < field.fieldCards.Count; i++)//필드에 놓인 카드들 중에서
            {
                field.fieldCards[i].transform.TryGetComponent(out Card card);
                for (int j = 0; j < card.currentCardData.Units.Count; j++)//SwordMan카드가 가진 유닛 중에서
                {
                    if (card.currentCardData.Units[j].UnitName == targetUnitName)//SwordMan 유닛이 있고
                    {
                        Debug.Log("스워드 유닛 있음!");
                        int tempNum = changeUnitNum;
                        if (card.currentCardData.Units[j].UnitAmount < changeUnitNum)//유닛 수가 변환하는 숫자보다 적으면
                        {
                            tempNum = card.currentCardData.Units[j].UnitAmount;//남아있는 유닛이라도 변화시킨다.
                        }
                        
                        card.currentCardData.Units[j].UnitAmount -= tempNum;//변환하는 수만큼 유닛 수를 줄이고
                        if (ContainNameList(card.currentCardData.Units, unitName))//이미 SpearMan이 그 카드에 존재하면
                        {
                            GetUnitElementNameList(card.currentCardData.Units,unitName).UnitAmount += tempNum;//SpearMan에 변환하는 수를 추가한다.
                        }
                        else//만약 SpearMan이 그 카드에 없다면
                        {
                            card.currentCardData.Units.Add(new SAOCardDatabase.UnitElement(unitPrefabDatabase.GetUnitPrefabByName(unitName), unitName, tempNum));//SpearMan을 새로 추가한다.
                        }
                        
                        Debug.Log("스피어맨으로 변환!");
                        card.UpdateCardInfo();
                        return;
                    }
                }
            }
        }
    }
    public void BowManAbility(GameObject cardObj)
    {
        //턴이 종료될 때마다 SwordMan 2기를 SpearMan으로 변환
                cardObj.transform.TryGetComponent(out Card cardObjCard);
                
                string unitName = "BowMan";
                string targetUnitName = "SwordMan";
                int changeUnitNum = 2;
                if(cardObjCard.currentCardData.Golden) changeUnitNum *= 2;
                
                if(cardObjCard.turnEnd)
                {
                    cardObjCard.turnEnd = false;
                    for (int i = 0; i < field.fieldCards.Count; i++)//필드에 놓인 카드들 중에서
                    {
                        field.fieldCards[i].transform.TryGetComponent(out Card card);
                        for (int j = 0; j < card.currentCardData.Units.Count; j++)//SwordMan카드가 가진 유닛 중에서
                        {
                            if (card.currentCardData.Units[j].UnitName == targetUnitName)//SwordMan 유닛이 있고
                            {
                                int tempNum = changeUnitNum;
                                if (card.currentCardData.Units[j].UnitAmount < changeUnitNum)//유닛 수가 변환하는 숫자보다 적으면
                                {
                                    tempNum = card.currentCardData.Units[j].UnitAmount;//남아있는 유닛이라도 변화시킨다.
                                }
                                
                                card.currentCardData.Units[j].UnitAmount -= tempNum;//변환하는 수만큼 유닛 수를 줄이고
                                if (ContainNameList(card.currentCardData.Units, unitName))//이미 SpearMan이 그 카드에 존재하면
                                {
                                    GetUnitElementNameList(card.currentCardData.Units,unitName).UnitAmount += tempNum;//SpearMan에 변환하는 수를 추가한다.
                                }
                                else//만약 SpearMan이 그 카드에 없다면
                                {
                                    card.currentCardData.Units.Add(new SAOCardDatabase.UnitElement(unitPrefabDatabase.GetUnitPrefabByName(unitName), unitName, tempNum));//SpearMan을 새로 추가한다.
                                }
                                
                                card.UpdateCardInfo();
                                return;
                            }
                        }
                    }
                }
    }
    public void BachelorAbility(GameObject cardObj)
    {
        //턴이 종료될 때마다 SwordMan 2기를 SpearMan으로 변환
                cardObj.transform.TryGetComponent(out Card cardObjCard);
                
                string unitName = "Bachelor";
                string targetUnitName = "SpearMan";
                int changeUnitNum = 2;
                if(cardObjCard.currentCardData.Golden) changeUnitNum *= 2;
                
                if(cardObjCard.turnEnd)
                {
                    cardObjCard.turnEnd = false;
                    for (int i = 0; i < field.fieldCards.Count; i++)//필드에 놓인 카드들 중에서
                    {
                        field.fieldCards[i].transform.TryGetComponent(out Card card);
                        for (int j = 0; j < card.currentCardData.Units.Count; j++)//SwordMan카드가 가진 유닛 중에서
                        {
                            if (card.currentCardData.Units[j].UnitName == targetUnitName)//SwordMan 유닛이 있고
                            {
                                int tempNum = changeUnitNum;
                                if (card.currentCardData.Units[j].UnitAmount < changeUnitNum)//유닛 수가 변환하는 숫자보다 적으면
                                {
                                    tempNum = card.currentCardData.Units[j].UnitAmount;//남아있는 유닛이라도 변화시킨다.
                                }
                                
                                card.currentCardData.Units[j].UnitAmount -= tempNum;//변환하는 수만큼 유닛 수를 줄이고
                                if (ContainNameList(card.currentCardData.Units, unitName))//이미 SpearMan이 그 카드에 존재하면
                                {
                                    GetUnitElementNameList(card.currentCardData.Units,unitName).UnitAmount += tempNum;//SpearMan에 변환하는 수를 추가한다.
                                }
                                else//만약 SpearMan이 그 카드에 없다면
                                {
                                    card.currentCardData.Units.Add(new SAOCardDatabase.UnitElement(unitPrefabDatabase.GetUnitPrefabByName(unitName), unitName, tempNum));//SpearMan을 새로 추가한다.
                                }
                                
                                Debug.Log("Bachelor로 변환!");
                                card.UpdateCardInfo();
                                return;
                            }
                        }
                    }
                }
    }
    public void BanneretAbility(GameObject cardObj)
    {
        //턴이 종료될 때마다 SwordMan 2기를 SpearMan으로 변환
                cardObj.transform.TryGetComponent(out Card cardObjCard);
                Debug.Log("씨발 왜 안돼");
                
                string unitName = "Banneret";
                string targetUnitName = "Bachelor";
                int changeUnitNum = 2;
                if(cardObjCard.currentCardData.Golden) changeUnitNum *= 2;
                
                if(cardObjCard.turnEnd)
                {
                    cardObjCard.turnEnd = false;
                    for (int i = 0; i < field.fieldCards.Count; i++)//필드에 놓인 카드들 중에서
                    {
                        field.fieldCards[i].transform.TryGetComponent(out Card card);
                        for (int j = 0; j < card.currentCardData.Units.Count; j++)//SwordMan카드가 가진 유닛 중에서
                        {
                            if (card.currentCardData.Units[j].UnitName == targetUnitName)//SwordMan 유닛이 있고
                            {
                                int tempNum = changeUnitNum;
                                if (card.currentCardData.Units[j].UnitAmount < changeUnitNum)//유닛 수가 변환하는 숫자보다 적으면
                                {
                                    tempNum = card.currentCardData.Units[j].UnitAmount;//남아있는 유닛이라도 변화시킨다.
                                }
                                
                                card.currentCardData.Units[j].UnitAmount -= tempNum;//변환하는 수만큼 유닛 수를 줄이고
                                if (ContainNameList(card.currentCardData.Units, unitName))//이미 SpearMan이 그 카드에 존재하면
                                {
                                    GetUnitElementNameList(card.currentCardData.Units,unitName).UnitAmount += tempNum;//SpearMan에 변환하는 수를 추가한다.
                                }
                                else//만약 SpearMan이 그 카드에 없다면
                                {
                                    card.currentCardData.Units.Add(new SAOCardDatabase.UnitElement(unitPrefabDatabase.GetUnitPrefabByName(unitName), unitName, tempNum));//SpearMan을 새로 추가한다.
                                }
                                
                                Debug.Log("Banneret으로 변환!");
                                card.UpdateCardInfo();
                                return;
                            }
                        }
                    }
                }
    }
    public void WarLordAbility(GameObject cardObj)
    {
        //턴이 종료될 때마다 SwordMan 2기를 SpearMan으로 변환
                cardObj.transform.TryGetComponent(out Card cardObjCard);
                
                string unitName = "WarLord";
                string targetUnitName = "Banneret";
                int changeUnitNum = 2;
                if(cardObjCard.currentCardData.Golden) changeUnitNum *= 2;
                
                if(cardObjCard.turnEnd)
                {
                    cardObjCard.turnEnd = false;
                    for (int i = 0; i < field.fieldCards.Count; i++)//필드에 놓인 카드들 중에서
                    {
                        field.fieldCards[i].transform.TryGetComponent(out Card card);
                        for (int j = 0; j < card.currentCardData.Units.Count; j++)//SwordMan카드가 가진 유닛 중에서
                        {
                            if (card.currentCardData.Units[j].UnitName == targetUnitName)//SwordMan 유닛이 있고
                            {
                                int tempNum = changeUnitNum;
                                if (card.currentCardData.Units[j].UnitAmount < changeUnitNum)//유닛 수가 변환하는 숫자보다 적으면
                                {
                                    tempNum = card.currentCardData.Units[j].UnitAmount;//남아있는 유닛이라도 변화시킨다.
                                }
                                
                                card.currentCardData.Units[j].UnitAmount -= tempNum;//변환하는 수만큼 유닛 수를 줄이고
                                if (ContainNameList(card.currentCardData.Units, unitName))//이미 SpearMan이 그 카드에 존재하면
                                {
                                    GetUnitElementNameList(card.currentCardData.Units,unitName).UnitAmount += tempNum;//SpearMan에 변환하는 수를 추가한다.
                                }
                                else//만약 SpearMan이 그 카드에 없다면
                                {
                                    card.currentCardData.Units.Add(new SAOCardDatabase.UnitElement(unitPrefabDatabase.GetUnitPrefabByName(unitName), unitName, tempNum));//SpearMan을 새로 추가한다.
                                }
                                
                                Debug.Log("WarLord로 변환!");
                                card.UpdateCardInfo();
                                return;
                            }
                        }
                    }
                }
    }
    public void KnightAbility(GameObject cardObj)
    {
        //턴이 종료될 때마다 SwordMan 2기를 SpearMan으로 변환
                cardObj.transform.TryGetComponent(out Card cardObjCard);
                Debug.Log("씨발 왜 안돼");
                
                string unitName = "Knight";
                string targetUnitName = "WarLord";
                int changeUnitNum = 2;
                if(cardObjCard.currentCardData.Golden) changeUnitNum *= 2;
                
                if(cardObjCard.turnEnd)
                {
                    cardObjCard.turnEnd = false;
                    for (int i = 0; i < field.fieldCards.Count; i++)//필드에 놓인 카드들 중에서
                    {
                        field.fieldCards[i].transform.TryGetComponent(out Card card);
                        for (int j = 0; j < card.currentCardData.Units.Count; j++)//SwordMan카드가 가진 유닛 중에서
                        {
                            if (card.currentCardData.Units[j].UnitName == targetUnitName)//SwordMan 유닛이 있고
                            {
                                int tempNum = changeUnitNum;
                                if (card.currentCardData.Units[j].UnitAmount < changeUnitNum)//유닛 수가 변환하는 숫자보다 적으면
                                {
                                    tempNum = card.currentCardData.Units[j].UnitAmount;//남아있는 유닛이라도 변화시킨다.
                                }
                                
                                card.currentCardData.Units[j].UnitAmount -= tempNum;//변환하는 수만큼 유닛 수를 줄이고
                                if (ContainNameList(card.currentCardData.Units, unitName))//이미 SpearMan이 그 카드에 존재하면
                                {
                                    GetUnitElementNameList(card.currentCardData.Units,unitName).UnitAmount += tempNum;//SpearMan에 변환하는 수를 추가한다.
                                }
                                else//만약 SpearMan이 그 카드에 없다면
                                {
                                    card.currentCardData.Units.Add(new SAOCardDatabase.UnitElement(unitPrefabDatabase.GetUnitPrefabByName(unitName), unitName, tempNum));//SpearMan을 새로 추가한다.
                                }
                                
                                Debug.Log("Knight로 변환!");
                                card.UpdateCardInfo();
                                return;
                            }
                        }
                    }
                }
    }
}
