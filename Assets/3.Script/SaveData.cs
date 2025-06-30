using System.Collections.Generic;
using Firebase.Firestore; // 꼭 필요!
using UnityEngine;

[FirestoreData]
public class SaveData
{
    [FirestoreProperty]
    public BattleSave battleSave { get; set; } = new BattleSave();
    [FirestoreProperty] public List<Log> logSave { get; set; }
}

[FirestoreData]
public class Log
{
    [FirestoreProperty] public bool isWin { get; set; }
    [FirestoreProperty] public int score { get; set; }
    [FirestoreProperty] public List<SaveCard> finalField { get; set; } = new List<SaveCard>();
}

[FirestoreData]
public class BattleSave
{
    [FirestoreProperty] public int saveRoundIndex { get; set; }
    [FirestoreProperty] public int savePersonalIdNum { get; set; }
    [FirestoreProperty] public int saveMaxGold { get; set; }
    [FirestoreProperty] public int saveCurrentGold { get; set; }
    [FirestoreProperty] public int saveConsumeGold { get; set; }
    [FirestoreProperty] public int saveTakeGold { get; set; }
    [FirestoreProperty] public int saveSellCardCount { get; set; }
    [FirestoreProperty] public int saveBuyCardCount { get; set; }
    [FirestoreProperty] public int savePreGold { get; set; }
    [FirestoreProperty] public int saveShopRank { get; set; }

    [FirestoreProperty] public List<SaveCard> saveFields { get; set; } = new List<SaveCard>();
    [FirestoreProperty] public List<SaveCard> saveHands { get; set; } = new List<SaveCard>();
    [FirestoreProperty] public List<SaveCard> saveShop { get; set; } = new List<SaveCard>();
    [FirestoreProperty] public List<SaveCardInven> saveCardInvens { get; set; } = new List<SaveCardInven>();
}

[FirestoreData]
public class SaveCardInven
{
    [FirestoreProperty] public int rank { get; set; }
    [FirestoreProperty] public SaveCardData card { get; set; } = new SaveCardData();
    [FirestoreProperty] public int SaveMaxNum { get; set; }
    [FirestoreProperty] public int SaveCurrentNum { get; set; }
}

[FirestoreData]
public class SaveCard
{
    [FirestoreProperty] public int SavePersonalId { get; set; }
    [FirestoreProperty] public SaveCardData SaveCardData { get; set; } = new SaveCardData();
    
    [FirestoreProperty] public int SaveintAbilityValue1 { get; set; }
    [FirestoreProperty] public int SaveintAbilityValue2 { get; set; }
    [FirestoreProperty] public float SavefloatAbilityValue { get; set; }
    [FirestoreProperty] public bool SaveboolAbilityValue { get; set; }
    [FirestoreProperty] public List<SaveBuffData> SaveBuffs { get; set; } = new List<SaveBuffData>();
}

[FirestoreData]
public class SaveCardData
{
    [FirestoreProperty] public string SaveId { get; set; }
    [FirestoreProperty] public string SaveName { get; set; }
    [FirestoreProperty] public string SaveType { get; set; }

    [FirestoreProperty] public List<SaveUnitElemt> SaveUnitElemts { get; set; } = new List<SaveUnitElemt>();

    [FirestoreProperty] public string SaveDescription { get; set; }
    [FirestoreProperty] public string SaveGoldenDescription { get; set; }

    [FirestoreProperty] public int SaveImageId { get; set; }
    [FirestoreProperty] public int SaveTypeImageId { get; set; }
    [FirestoreProperty] public int SaveRankImageId { get; set; }
    [FirestoreProperty] public int SaveBackgroundImageId { get; set; }

    [FirestoreProperty] public bool Golden { get; set; }
    [FirestoreProperty] public int MaxNum { get; set; }
}

[FirestoreData]
public class SaveUnitElemt
{
    [FirestoreProperty] public int SaveUnitId { get; set; }
    [FirestoreProperty] public string SaveUnitName { get; set; }
    [FirestoreProperty] public int SaveUnitAmount { get; set; }
}

[FirestoreData]
public class SaveBuffData
{
    [FirestoreProperty] public int SaveBuffIndex { get; set; }
    [FirestoreProperty] public int SaveBuffPersonalId { get; set; }
    [FirestoreProperty] public float BuffValue { get; set; }
}
