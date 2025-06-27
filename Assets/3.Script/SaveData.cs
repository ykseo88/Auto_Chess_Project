using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData
{
    public BattleSave battleSave = new BattleSave();
}

public class BattleSave
{
    public int saveRoundIndex;//라운드 진행
    
    public int saveMaxGold;//최대 골드
    public int saveCurrentGold;//현재 골드
    public int saveConsumeGold;//이번 판 소모 골드
    public int saveTakeGold;//이번 판 획득 골드
    
    public List<SaveCard> saveFields = new List<SaveCard>();//필드 목록
    public List<SaveCard> saveHands = new List<SaveCard>();//손패 목록
    public List<SaveCard> saveShop = new List<SaveCard>();//매물 목록
    
    public List<SaveCardInven> saveCardInvens = new List<SaveCardInven>();//상점 재고
}

public class SaveCardInven
{
    public int rank;
    public SaveCardData card = new SaveCardData();
    public int SaveMaxNum;
    public int SaveCurrentNum;
}

public class SaveCard
{
    public int SavePersonalId;
    public SaveCardData SaveCardData = new SaveCardData();
    public List<SaveBuffData> SaveBuffs = new List<SaveBuffData>();
}

public class SaveCardData
{
    public string SaveId;
    public string SaveName;
    public string SaveType;
    List<SaveUnitElemt> SaveUnitElemts = new List<SaveUnitElemt>();
    public string SaveDescription;
    public string SaveGoldenDescription;
    public int SaveImageId;
    public int SaveTypeImageId;
    public int SaveRankImageId;
    public int SaveBackgroundImageId;
    public bool Golden; // 트리플 카드 여부
    public int MaxNum; // 카드 최대 개수
    
}

public class SaveUnitElemt
{
    public int SaveUnitId;
    public string SaveUnitName;
    public int SaveUnitAmount;
}

public class SaveBuffData
{
    public int SaveBuffIndex;
    public int SaveBuffPersonalId;
    public float BuffValue;
}
