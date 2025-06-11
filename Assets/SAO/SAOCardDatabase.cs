using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SAOCardDatabase", menuName = "ScriptableObjects/Card Database")]
public class SAOCardDatabase : ScriptableObject
{
    public enum Type
    {
        Knight,
        Viking,
        Western
    }

    [System.Serializable]
    public class UnitElement
    {
        public GameObject Unit;
        public string UnitName;
        public int UnitAmount;
    }
    
    
    [System.Serializable]
    public class CardData
    {
        public string ID;
        public string Name;
        public int Rank;
        public Type Type;
        public List<UnitElement> Units;
        public string Description;
        public Sprite Image;
        public Sprite TypeImage;
        public Sprite RankImage;
        public Sprite BackgroundImage;
        public bool Triple = false; // 트리플 카드 여부
    }
    
    public List<CardData> cards = new List<CardData>();
    
    public CardData GetCardById(string id)
    {
        foreach (CardData entry in cards)
        {
            if (entry.ID == id)
            {
                return entry;
            }
        }
        Debug.LogWarning($"Sprite with ID '{id}' not found in {name}.");
        return null;
    }
}
