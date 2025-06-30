using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Firestore;
using UnityEngine;

public class LoadManager : MonoBehaviour
{
    
    private FirebaseFirestore db;
    
    public static LoadManager Instance;
    
    public SAOCardDatabase cardDatabase;
    public SAOUnitPrefabDatabase unitDatabase;
    public SAOSpriteDatabase spriteDatabase;
    
    public SaveManager saveManager;
    
    public SaveData loadData;
    
    public async void Load()
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        DocumentReference docRef = db.Collection("playerSaves").Document(userId);
        DocumentSnapshot docSnapshot = await docRef.GetSnapshotAsync();
        if (docSnapshot.Exists)
        {
            loadData = docSnapshot.ConvertTo<SaveData>();
            //saveManager.saveData = loadData;
        }
    }

    private void Awake()
    {
        Instance = this;
        db = FirebaseFirestore.DefaultInstance;
        Load();
    }

    void Start()
    {
        
        
    }
    
    void Update()
    {
        
    }
    
    public void LoadCard(Card card, SaveCard saveCard)
    {
        card.personalID = saveCard.SavePersonalId;
        
        LoadCardData(card.currentCardData, saveCard.SaveCardData);

        card.intAbilityValue1 = saveCard.SaveintAbilityValue1;
        card.intAbilityValue2 = saveCard.SaveintAbilityValue2;
        card.floatAbilityValue =  saveCard.SavefloatAbilityValue;
        card.boolAbilityValue =  saveCard.SaveboolAbilityValue;
        
        for (int i = 0; i < saveCard.SaveBuffs.Count; i++)
        {
            if (card.BuffRateDicArray[saveCard.SaveBuffs[i].SaveBuffIndex] == null)
            {
                Dictionary<int, float> tempDict = new Dictionary<int, float>();
                card.BuffRateDicArray[saveCard.SaveBuffs[i].SaveBuffIndex] = tempDict;
            }
            card.BuffRateDicArray[saveCard.SaveBuffs[i].SaveBuffIndex].TryAdd(saveCard.SaveBuffs[i].SaveBuffPersonalId, saveCard.SaveBuffs[i].BuffValue);
        }
    }

    public void LoadCardData(SAOCardDatabase.CardData cardData, SaveCardData saveCardData)
    {
        cardData.ID = saveCardData.SaveId;
        cardData.Name = saveCardData.SaveName;

        switch (saveCardData.SaveType)
        {
            case "Viking":
                cardData.Type = SAOCardDatabase.Type.Viking;
                break;
            case "Knight":
                cardData.Type = SAOCardDatabase.Type.Knight;
                break;
            case "Western":
                cardData.Type = SAOCardDatabase.Type.Western;
                break;
        }

        for (int i = 0; i < saveCardData.SaveUnitElemts.Count; i++)
        {
            SAOCardDatabase.UnitElement tempUnitElemt = 
                new SAOCardDatabase.UnitElement(
                    unitDatabase.GetUnitPrefabById(saveCardData.SaveUnitElemts[i].SaveUnitId), 
                    saveCardData.SaveUnitElemts[i].SaveUnitName,
                    saveCardData.SaveUnitElemts[i].SaveUnitAmount);
            cardData.Units.Add(tempUnitElemt);
        }
        
        cardData.Description = saveCardData.SaveDescription;
        cardData.GoldenDescription =  saveCardData.SaveGoldenDescription;
        cardData.Image = spriteDatabase.GetSpriteById(saveCardData.SaveImageId);
        cardData.TypeImage = spriteDatabase.GetSpriteById(saveCardData.SaveTypeImageId);
        cardData.RankImage = spriteDatabase.GetSpriteById(saveCardData.SaveRankImageId);
        cardData.Golden = saveCardData.Golden;
        cardData.MaxNum = saveCardData.MaxNum;
    }
}
