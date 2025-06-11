using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public enum Type
{
    Knight,
    Viking,
    Western
}

public class CardData
{
    public string ID;
    public string Name;
    public int Rank;
    public GameObject Unit;
    public Type Type;
    public int UnitAmount;
    public string Description;
    public Sprite Image;
    //Assets/Resources/Data/CardImage/LV1-SwordSoldier.png
}
