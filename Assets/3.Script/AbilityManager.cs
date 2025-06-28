using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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


    private void DebugLogBuff()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            for (int i = 0; i < field.fieldCards.Count; i++)
            {
                field.fieldCards[i].transform.TryGetComponent(out Card card);
                
                float[] rate = new  float[5];

                for (int j = 0; j < card.BuffRateDicArray.Length; j++)
                {
                    foreach (var buff in card.BuffRateDicArray[j])
                    {
                        rate[j] += buff.Value;
                        if (GetCardByPersonalId(buff.Key) != null)
                        {
                            switch (j)
                            {
                                case 0:
                                    Debug.Log($"<color=green>{i+1}번 카드: {GetCardByPersonalId(buff.Key).currentCardData.Name}, 체력 {buff.Value}배</color>");
                                    break;
                                case 1:
                                    Debug.Log($"<color=green>{i+1}번 카드: {GetCardByPersonalId(buff.Key).currentCardData.Name}, 데미지 {buff.Value}배</color>");
                                    break;
                                case 2:
                                    Debug.Log($"<color=green>{i+1}번 카드: {GetCardByPersonalId(buff.Key).currentCardData.Name}, 이속 {buff.Value}배</color>");
                                    break;
                                case 3:
                                    Debug.Log($"<color=green>{i+1}번 카드: {GetCardByPersonalId(buff.Key).currentCardData.Name}, 공속 {buff.Value}배</color>");
                                    break;
                                case 4:
                                    Debug.Log($"<color=green>{i+1}번 카드: {GetCardByPersonalId(buff.Key).currentCardData.Name}, 사정거리 {buff.Value}배</color>");
                                    break;
                            }
                        }
                    }
                }
                
                Debug.Log($"<color=red>{i+1}번 카드 총 버프량: 체력 {rate[0]}배, 데미지 {rate[1]}배, 이속 {rate[2]}배, 공속 {rate[3]}배, 사정거리 {rate[4]}배</color>");
            }
        }
    }

    public Card GetCardByPersonalId(int id)
    {
        List<Card> cardList = new List<Card>();
        for (int i = 0; i < field.fieldCards.Count; i++)
        {
            field.fieldCards[i].transform.TryGetComponent(out Card card);
            cardList.Add(card);
        }
        
        for (int i = 0; i < shop.currentSellCards.Count; i++)
        {
            shop.currentSellCards[i].transform.TryGetComponent(out Card card);
            cardList.Add(card);
        }

        for (int i = 0; i < hand.handCards.Count; i++)
        {
            hand.handCards[i].transform.TryGetComponent(out Card card);
            cardList.Add(card);
        }

        for (int i = 0; i < cardList.Count; i++)
        {
            if (cardList[i].personalID == id)
            {
                return cardList[i];
            }
        }

        return null;
    }

    private void Update()
    {
        DebugLogBuff();
    }

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
        InsertAbilityDictionary("Bandit", BanditAbility);
        InsertAbilityDictionary("OutLaw", OutLawAbility);
        InsertAbilityDictionary("Deputy", DeputyAbility);
        InsertAbilityDictionary("Hunter", HunterAbility);
        InsertAbilityDictionary("ChainGun", ChainGunAbility);
        InsertAbilityDictionary("Bomber", BomberAbility);
        InsertAbilityDictionary("Thralls", ThrallsAbility);
        InsertAbilityDictionary("Vedir", VedirAbility);
        InsertAbilityDictionary("Flakkari", FlakkariAbility);
        InsertAbilityDictionary("Berserker", BerserkerAbility);
        InsertAbilityDictionary("Volva", VolvaAbility);
        InsertAbilityDictionary("Jarl", JarlAbility);
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
    
    //--------------------Knight---------------------------------------
    
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
        //턴이 종료될 때마다 SwordMan 1기 추가
        cardObj.transform.TryGetComponent(out Card cardObjCard);
        
        string unitName = "병사";
        int plusUnitNum = 1;
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
        string TargetUnitName = "병사";

        if (!cardObjCard.boolAbilityValue)
        {
            cardObjCard.boolAbilityValue = true;
            for (int i = 0; i < field.fieldCards.Count; i++)
            {
                field.fieldCards[i].transform.TryGetComponent(out Card card);
                if (card.currentCardData.Name == TargetUnitName)
                {
                    //card.currentCardData.Units.FirstOrDefault(c => c.UnitName == "SwordMan").UnitAmount += 2;
                    //card.UpdateCardInfo();

                    for (int j = 0; j < card.currentCardData.Units.Count; j++)
                    {
                        if (card.currentCardData.Units[j].UnitName == TargetUnitName)
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
        
        string unitName = "창병";
        string targetUnitName = "병사";
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
                
                string unitName = "궁병";
                string targetUnitName = "병사";
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
                
                string unitName = "부관";
                string targetUnitName = "창병";
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
                
                string unitName = "부관";
                string targetUnitName = "배너렛";
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
                
                string unitName = "워로드";
                string targetUnitName = "배너렛";
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
                
                string unitName = "기사";
                string targetUnitName = "워로드";
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
    
    //--------------------Western---------------------------------------
    public void BanditAbility(GameObject cardObj)
    {
        cardObj.transform.TryGetComponent(out Card cardObjCard);

        int plusGold = 1;
        if(cardObjCard.currentCardData.Golden)  plusGold *= 2;
        
        if (!cardObjCard.boolAbilityValue)
        {
            cardObjCard.SellPrice += plusGold;
            cardObjCard.boolAbilityValue = true;
        }
    }
    public void OutLawAbility(GameObject cardObj)
    {
        cardObj.transform.TryGetComponent(out Card cardObjCard);
        
        int plusCardNum = 1;
        if(cardObjCard.currentCardData.Golden)  plusCardNum *= 2;
        
        if (!cardObjCard.boolAbilityValue)
        {
            for (int n = 0; n < plusCardNum; n++)
            {
                if (hand.handCards.Count < 6)
                {
                    GameObject tempCard = Instantiate(cardObj, hand.transform);
                    tempCard.transform.localScale = new Vector3(0.7f, 0.7f, 1);
                    tempCard.transform.TryGetComponent(out Card tempCardComponent);
                
                    List<Shop.CardInven> allowChoiceCards = new List<Shop.CardInven>();
                
                    int rank = shop.shopRank - 1;
                    if (rank == 0) rank = 1;
                
                    for (int i = 0; i < shop.cardPool[rank].Count; i++)
                    {
                        for(int j = 0; j < shop.cardPool[rank][i].currentNum; j++)
                        {
                            allowChoiceCards.Add(shop.cardPool[rank][i]);
                        }
                    }

                    Shop.CardInven tempCardData = allowChoiceCards[Random.Range(0, allowChoiceCards.Count)];
                    Debug.Log($"아웃로 효과로 얻은 카드: {tempCardData.card.Name}");
                    tempCardComponent.InsertCard(tempCardData.card);
                    tempCardComponent.isChoiceCard = false;
                    tempCardComponent.parentList = hand.handCards;
                    tempCardComponent.inventorys = tempCardData;
                    tempCardComponent.inventorys.currentNum--;
                    tempCardComponent.SetStarting();
                    tempCardComponent.parentList.Add(tempCard);
                }
            }
            cardObjCard.boolAbilityValue = true;
        }
    }
    public void DeputyAbility(GameObject cardObj)
    {
        cardObj.transform.TryGetComponent(out Card cardObjCard);

        int plusMaxGold = 1;
        int checkSellCard = 10;
        if(cardObjCard.currentCardData.Golden) plusMaxGold *= 2;
        
        if (cardObjCard.currentCardData.Golden) plusMaxGold *= 2;
        
        if (cardObjCard.intAbilityValue2 < 0)
        {
            cardObjCard.intAbilityValue2 = field.SellCardCount;
            cardObjCard.intAbilityValue1 = 0;
        }
        
        cardObjCard.intAbilityValue1 = field.SellCardCount - cardObjCard.intAbilityValue2;

        if (cardObjCard.intAbilityValue1 >= checkSellCard)
        {
            field.maxGold += plusMaxGold;
            
            int tempLeast = cardObjCard.intAbilityValue1 % checkSellCard;
            int tempInt = cardObjCard.intAbilityValue1 - tempLeast;
            cardObjCard.intAbilityValue2 += tempInt;
            cardObjCard.intAbilityValue1 = tempLeast;
            cardObjCard.UpdateCardInfo();
        }
        
        if (cardObjCard.turnStart)
        {
            cardObjCard.turnStart = false;
            cardObjCard.intAbilityValue1 = 0;
            cardObjCard.intAbilityValue2 = 0;
        }
        
        
    }
    public void HunterAbility(GameObject cardObj)
    {
        
        //5골드 팔 때마다 다음 전투에서 이 카드의 유닛들의 공격력이 20% 상승합니다.
        cardObj.transform.TryGetComponent(out Card cardObjCard);
        
        float DamageRate = 0.2f;
        int checkGold = 5;
        if (cardObjCard.currentCardData.Golden) DamageRate *= 2;
        
        if (cardObjCard.intAbilityValue2 < 0)
        {
            cardObjCard.intAbilityValue2 = field.consumedGold;
            cardObjCard.intAbilityValue1 = 0;
        }
        
        cardObjCard.intAbilityValue1 = field.consumedGold - cardObjCard.intAbilityValue2;

        if (cardObjCard.intAbilityValue1 >= checkGold)
        {

            if (cardObjCard.DamageRateDic.ContainsKey(cardObjCard.personalID))
            {
                cardObjCard.DamageRateDic[cardObjCard.personalID] += DamageRate;
            }
            else
            {
                cardObjCard.DamageRateDic.Add(cardObjCard.personalID, DamageRate);
            }
            
            
            int tempLeast = cardObjCard.intAbilityValue1 % checkGold;
            int tempInt = cardObjCard.intAbilityValue1 - tempLeast;
            cardObjCard.intAbilityValue2 += tempInt;
            cardObjCard.intAbilityValue1 = tempLeast;
            cardObjCard.UpdateCardInfo();
        }

        if (cardObjCard.turnStart)
        {
            cardObjCard.turnStart = false;
            cardObjCard.intAbilityValue1 = 0;
            cardObjCard.intAbilityValue2 = 0;
            cardObjCard.BuffRateDicArray[1].Remove(cardObjCard.personalID);
        }

        


    }
    public void ChainGunAbility(GameObject cardObj)
    {
        //이 카드가 필드에 진입할 때, 랜덤한 카드의 랜덤한 유닛이 4 증가합니다.
        cardObj.transform.TryGetComponent(out Card cardObjCard);

        int plusUnitNum = 4;
        if(cardObjCard.currentCardData.Golden)  plusUnitNum *= 2;
        
        if (!cardObjCard.boolAbilityValue)
        {
            int randomCardIndex = Random.Range(0, field.fieldCards.Count);
            field.fieldCards[randomCardIndex].transform.TryGetComponent(out Card plusCard);
            int randomUnitIndex = Random.Range(0, plusCard.currentCardData.Units.Count);
            plusCard.currentCardData.Units[randomUnitIndex].UnitAmount +=  plusUnitNum;
            plusCard.UpdateCardInfo();
            cardObjCard.boolAbilityValue = true;
        }
    }
    public void BomberAbility(GameObject cardObj)
    {
        cardObj.transform.TryGetComponent(out Card cardObjCard);
        
        if (!cardObjCard.isWaitDestroy)
        {
            cardObjCard.isWaitDestroy = true;
        }

        int ChangeToBuyCost = 2;
        if(cardObjCard.currentCardData.Golden) ChangeToBuyCost /= 2;

        for (int i = 0; i < shop.currentSellCards.Count; i++)
        {
            shop.currentSellCards[i].transform.TryGetComponent(out Card card);
            card.BuyPrice = ChangeToBuyCost;
        }
        
        
        if (cardObjCard.isSellNow)//팔릴 때 원상복귀
        {
            for (int i = 0; i < shop.currentSellCards.Count; i++)
            {
                shop.currentSellCards[i].transform.TryGetComponent(out Card card);
                card.BuyPrice = 3;
            }
            Destroy(cardObj);
        }
    }
    //--------------------Viking---------------------------------------
    public void ThrallsAbility(GameObject cardObj)
    {
        cardObj.transform.TryGetComponent(out Card cardObjCard);

        int freeReRollNum = 1;
        if(cardObjCard.currentCardData.Golden)  freeReRollNum *= 2;

        if (!cardObjCard.boolAbilityValue)
        {
            cardObjCard.boolAbilityValue = true;
            cardObjCard.intAbilityValue1 = freeReRollNum;
        }
        
        if (shop.ReRollPrice > 0 && cardObjCard.intAbilityValue1 > 0)
        {
            shop.ReRollPrice = 0;
            cardObjCard.intAbilityValue1--;
        }
    }
    public void VedirAbility(GameObject cardObj)
    {
        //사정거리 50% 증가
        
        cardObj.transform.TryGetComponent(out Card cardObjCard);
        
        if (!cardObjCard.isWaitDestroy)
        {
            cardObjCard.isWaitDestroy = true;
        }

        float buffRange = 0.5f;
        if(cardObjCard.currentCardData.Golden) buffRange *= 2;

        for (int i = 0; i < field.fieldCards.Count; i++)
        {
            field.fieldCards[i].transform.TryGetComponent(out Card card);
            card.AttackRangeRateDic.TryAdd(cardObjCard.personalID, buffRange);
        }
        
        
        if (cardObjCard.isSellNow)//팔릴 때 원상복귀
        {
            for (int i = 0; i < field.fieldCards.Count; i++)
            {
                field.fieldCards[i].transform.TryGetComponent(out Card card);
                card.AttackRangeRateDic.Remove(cardObjCard.personalID);
            }
            Destroy(cardObj);
        }
        
    }

    public void FlakkariAbility(GameObject cardObj)
    {
        //공격력 40% 증가
        
        cardObj.transform.TryGetComponent(out Card cardObjCard);
        
        if (!cardObjCard.isWaitDestroy)
        {
            cardObjCard.isWaitDestroy = true;
        }

        float buffRange = 0.4f;
        if(cardObjCard.currentCardData.Golden) buffRange *= 2;

        for (int i = 0; i < field.fieldCards.Count; i++)
        {
            field.fieldCards[i].transform.TryGetComponent(out Card card);
            card.DamageRateDic.TryAdd(cardObjCard.personalID, buffRange);
        }
        
        
        if (cardObjCard.isSellNow)//팔릴 때 원상복귀
        {
            for (int i = 0; i < field.fieldCards.Count; i++)
            {
                field.fieldCards[i].transform.TryGetComponent(out Card card);
                card.DamageRateDic.Remove(cardObjCard.personalID);
            }
            Destroy(cardObj);
        }
    }

    public void BerserkerAbility(GameObject cardObj)
    {
        //체력 50% 증가
        cardObj.transform.TryGetComponent(out Card cardObjCard);
        
        if (!cardObjCard.isWaitDestroy)
        {
            cardObjCard.isWaitDestroy = true;
        }

        float buffRange = 0.5f;
        if(cardObjCard.currentCardData.Golden) buffRange *= 2;

        for (int i = 0; i < field.fieldCards.Count; i++)
        {
            field.fieldCards[i].transform.TryGetComponent(out Card card);
            card.HealthRateDic.TryAdd(cardObjCard.personalID, buffRange);
        }
        
        
        if (cardObjCard.isSellNow)//팔릴 때 원상복귀
        {
            for (int i = 0; i < field.fieldCards.Count; i++)
            {
                field.fieldCards[i].transform.TryGetComponent(out Card card);
                card.HealthRateDic.Remove(cardObjCard.personalID);
            }
            Destroy(cardObj);
        }
    }

    public void VolvaAbility(GameObject cardObj)
    {
        //공속 100%증가
        cardObj.transform.TryGetComponent(out Card cardObjCard);
        
        if (!cardObjCard.isWaitDestroy)
        {
            cardObjCard.isWaitDestroy = true;
        }

        int VolvaKey = -2;
        float buffRange = 1f;
        if(cardObjCard.currentCardData.Golden) buffRange *= 2;

        for (int i = 0; i < field.fieldCards.Count; i++)
        {
            field.fieldCards[i].transform.TryGetComponent(out Card card);
            card.AttackSpeedRateDic.TryAdd(VolvaKey, buffRange);
        }
        
        if (cardObjCard.isSellNow)//팔릴 때 원상복귀
        {
            for (int i = 0; i < field.fieldCards.Count; i++)
            {
                field.fieldCards[i].transform.TryGetComponent(out Card card);
                card.AttackSpeedRateDic.Remove(VolvaKey);
            }
            Destroy(cardObj);
        }
    }

    public void JarlAbility(GameObject cardObj)
    {
        //모든 버프량 100% 증가
        cardObj.transform.TryGetComponent(out Card cardObjCard);
        
        if (!cardObjCard.isWaitDestroy)//나중에 팔거나 합쳐질 때 원상복귀 하라는 표시
        {
            cardObjCard.isWaitDestroy = true;
        }

        int JarlKey = -3;
        float buffBuffRate = 1f;
        if (cardObjCard.currentCardData.Golden) buffBuffRate *= 2;

        for (int i = 0; i < field.fieldCards.Count; i++)
        {
            field.fieldCards[i].transform.TryGetComponent(out Card card);

            for (int j = 0; j < card.BuffRateDicArray.Length; j++)
            {
                float tempBuff = 0f;
                foreach (var buff in card.BuffRateDicArray[j])
                {
                    if (buff.Key != JarlKey)
                    {
                        tempBuff += buff.Value;
                    }
                }

                float inputBuff = (tempBuff - 1) * buffBuffRate;
                card.BuffRateDicArray[j].TryAdd(JarlKey, inputBuff);
                if (card.BuffRateDicArray[j].ContainsKey(JarlKey))
                {
                    if (!Mathf.Approximately(card.BuffRateDicArray[j][JarlKey], inputBuff))
                    {
                        card.BuffRateDicArray[j][JarlKey] = inputBuff;
                    }
                }
            }
        }
        
        
        
        if (cardObjCard.isSellNow)//팔릴 때 원상복귀
        {
            for (int i = 0; i < field.fieldCards.Count; i++)
            {
                field.fieldCards[i].transform.TryGetComponent(out Card card);

                for (int j = 0; j < card.BuffRateDicArray.Length; j++)
                {
                    card.BuffRateDicArray[j].Remove(cardObjCard.personalID);
                }
            }
            
            Destroy(cardObj);
        }
    }
}
