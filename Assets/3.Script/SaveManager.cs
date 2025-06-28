using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Firestore;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private SaveData saveData = new SaveData();
    
    public SAOUnitPrefabDatabase unitDatabase;
    public SAOSpriteDatabase spriteDatabase;

    public Field field;
    public Shop shop;
    public Hand hand;
    public RoundManager roundManager;
    
    private FirebaseFirestore db; 

    private void Start()
    {
        // Firebase Firestore 초기화
        // FirebaseApp.CheckAndFixDependenciesAsync() 및 FirebaseApp.DefaultInstance 초기화는
        // 보통 MainSystem 이나 FirebaseAccountManager 같은 전역 초기화 스크립트에서 한번만 수행됩니다.
        // 여기서는 이미 초기화되었다고 가정하고 DefaultInstance를 가져옵니다.
        db = FirebaseFirestore.DefaultInstance; 

        if (db == null)
        {
            Debug.LogError("Firebase Firestore가 초기화되지 않았습니다. Firebase 설정 및 초기화 스크립트를 확인하세요.");
        }
    }

    public void SaveInsertCard(Card card, SaveCard saveCard)
    {
        saveCard.SavePersonalId = card.personalID;
        
        SaveInsertCardData(card.currentCardData, saveCard.SaveCardData);
        
        string cardname = card.gameObject.name;
        string cardDataname = card.currentCardData.Name;
        
        saveCard.SaveintAbilityValue1 = card.intAbilityValue1;
        saveCard.SavefloatAbilityValue = card.floatAbilityValue;
        saveCard.SaveboolAbilityValue = card.boolAbilityValue;
        saveCard.SaveintAbilityValue2 = card.intAbilityValue2;
        
        for (int i = 0; i < card.BuffRateDicArray.Length; i++)
        {
            foreach (var buff in card.BuffRateDicArray[i])
            {
                SaveBuffData tempBuffData = new SaveBuffData();
                tempBuffData.SaveBuffIndex = i;
                tempBuffData.SaveBuffPersonalId = buff.Key;
                tempBuffData.BuffValue = buff.Value;
                saveCard.SaveBuffs.Add(tempBuffData);
            }
        }
    }

    public void SaveInsertCardData(SAOCardDatabase.CardData cardData, SaveCardData saveData)
    {
        saveData.SaveId = cardData.ID;
        
        saveData.SaveName = cardData.Name;
        saveData.SaveType = cardData.Type.ToString();
        for (int i = 0; i < cardData.Units.Count; i++)
        {
            SaveUnitElemt tempUnitElemet = new SaveUnitElemt();
            tempUnitElemet.SaveUnitId = unitDatabase.GetIdByUnitPrefab(cardData.Units[i].Unit);
            tempUnitElemet.SaveUnitName = cardData.Units[i].UnitName;
            tempUnitElemet.SaveUnitAmount = cardData.Units[i].UnitAmount;
            saveData.SaveUnitElemts.Add(tempUnitElemet);
        }
        saveData.SaveDescription = cardData.Description;
        saveData.SaveGoldenDescription = cardData.GoldenDescription;
        saveData.SaveImageId = spriteDatabase.GetIdBySprite(cardData.Image);
        saveData.SaveTypeImageId = spriteDatabase.GetIdBySprite(cardData.TypeImage);
        saveData.SaveRankImageId = spriteDatabase.GetIdBySprite(cardData.RankImage);
        if(cardData.BackgroundImage != null)saveData.SaveBackgroundImageId = spriteDatabase.GetIdBySprite(cardData.BackgroundImage);
        saveData.Golden = cardData.Golden;
        saveData.MaxNum = cardData.MaxNum;
    }

    public void SaveBattelData()
    {
        ClearBattleSave();
        //현재 라운드 저장
        saveData.battleSave.saveRoundIndex = roundManager.currentRoundIndex;
        
        //카드 개인번호 발급 현황 저장
        saveData.battleSave.savePersonalIdNum = GameManager.Instance.pesonalIDNum;
        
        //현재 골드 정보 저장
        saveData.battleSave.saveMaxGold = field.maxGold;
        saveData.battleSave.saveCurrentGold = field.Gold;
        saveData.battleSave.saveConsumeGold = field.consumedGold;
        saveData.battleSave.saveTakeGold = field.takeGold;
        saveData.battleSave.saveSellCardCount = field.SellCardCount;
        saveData.battleSave.saveBuyCardCount = field.BuyCardCount;
        saveData.battleSave.savePreGold = field.preGold;
        
        //필드 카드 정보 저장
        for (int i = 0; i < field.fieldCards.Count; i++)
        {
            field.fieldCards[i].transform.TryGetComponent(out Card card);
            SaveCard tempSaveCard = new SaveCard();
            SaveInsertCard(card, tempSaveCard);
            saveData.battleSave.saveFields.Add(tempSaveCard);
        }
        
        //손패 카드 정보 저장
        for (int i = 0; i < hand.handCards.Count; i++)
        {
            hand.handCards[i].transform.TryGetComponent(out Card card);
            SaveCard tempSaveCard = new SaveCard();
            SaveInsertCard(card, tempSaveCard);
            saveData.battleSave.saveHands.Add(tempSaveCard);
        }
        
        //상점 랭크 저장
        saveData.battleSave.saveShopRank = shop.shopRank;
        
        //상점 매물 정보 저장
        for (int i = 0; i < shop.currentSellCards.Count; i++)
        {
            shop.currentSellCards[i].transform.TryGetComponent(out Card card);
            SaveCard tempSaveCard = new SaveCard();
            SaveInsertCard(card, tempSaveCard);
            saveData.battleSave.saveShop.Add(tempSaveCard);
        }
        
        //상점 재고 저장
        /*foreach (var rank in shop.cardPool)
        {
            for (int i = 0; i < rank.Value.Count; i++)
            {
                SaveCardInven tempSaveCardInven = new SaveCardInven();
                tempSaveCardInven.rank = rank.Key;
                SaveInsertCardData(rank.Value[i].card, tempSaveCardInven.card);
                tempSaveCardInven.SaveMaxNum =  rank.Value[i].maxNum;
                tempSaveCardInven.SaveCurrentNum = rank.Value[i].currentNum;
            }
        }*/
    }

    public void SaveLog(bool win)
    {
        Log tempLog = new Log();

        tempLog.isWin = win;
        tempLog.score = roundManager.currentRoundIndex - 1;

        for (int i = 0; i < field.fieldCards.Count; i++)
        {
            field.fieldCards[i].transform.TryGetComponent(out Card card);
            SaveCard tempSaveCard = new SaveCard();
            SaveInsertCard(card, tempSaveCard);
            tempLog.finalField.Add(tempSaveCard);
        }
        
        saveData.logSave.Add(tempLog);
    }

    public async void PushSaveData()
    {
        if (db == null)
        {
            Debug.LogError("Firestore가 초기화되지 않아 데이터를 저장할 수 없습니다.");
            return;
        }

        // 1. 저장할 데이터의 경로 정의
        // 보통 사용자별로 저장하므로, 인증된 유저의 UID를 사용합니다.
        // 여기에선 예시로 "test_user_id"를 사용하지만, 실제로는 FirebaseAuth.DefaultInstance.CurrentUser.UserId를 사용해야 합니다.
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId; 
        string documentId = "player_save_data"; // 특정 세이브 슬롯이나 고정된 문서 ID

        DocumentReference docRef = db.Collection("playerSaves").Document(userId);

        try
        {
            // 2. C# 객체를 Firestore에 저장 (자동 직렬화)
            // JsonUtility.ToJson(saveData)로 문자열을 만들 필요 없이 saveData 객체 자체를 넘겨줍니다.
            await docRef.SetAsync(saveData);

            Debug.Log($"Firebase Firestore에 데이터가 성공적으로 저장되었습니다: playerSaves/{userId}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Firebase Firestore 저장 실패: {e.Message}");
        }
    }

    public void ClearBattleSave()
    {
        saveData.battleSave =  new BattleSave();
    }

    public void ClearLog()
    {
        saveData.logSave = new List<Log>();
    }

    public void ClearSave()
    {
        saveData = new SaveData();
    }
    
}
