using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatcCard : MonoBehaviour
{
    public SAOCardDatabase.CardData currentCardData;
    
    public Image BackGroundImage;
    public Image ChoiceBackGroundImage;
    public Image TitleImage;
    public TMP_Text Title;
    public Image CardImage;
    public Image AmountImage;
    public TMP_Text Amount;
    public int AllAmount;
    public Image RankImage;
    public Image TypeImage;
    
    public UnitAssignTrun unitAssignTrun;

    public float maxRayDistance = 50f;

    public float[] Buffs = new float[5];

    private void Start()
    {
    }

    public void InsertCard(SAOCardDatabase.CardData insertData)
    {
        InputCardData(insertData);
        UpdateCardInfo();
    }

    public void InputCardData(SAOCardDatabase.CardData insertData)
    {
        currentCardData.ID = insertData.ID;
        currentCardData.Name = insertData.Name;
        currentCardData.Rank = insertData.Rank;
        currentCardData.Type = insertData.Type;
        currentCardData.Units.Clear();
        for (int i = 0; i < insertData.Units.Count; i++)
        {
            currentCardData.Units.Add(new SAOCardDatabase.UnitElement(insertData.Units[i].Unit, insertData.Units[i].UnitName, insertData.Units[i].UnitAmount));
        }
        currentCardData.Description = insertData.Description;
        currentCardData.Image = insertData.Image;
        currentCardData.TypeImage = insertData.TypeImage;
        currentCardData.RankImage = insertData.RankImage;
        currentCardData.BackgroundImage = insertData.BackgroundImage;
        currentCardData.Golden = insertData.Golden;
        currentCardData.MaxNum = insertData.MaxNum;
    }
    
    public void UpdateCardInfo()
    {
        AllAmount = 0;
        for (int i = 0; i < currentCardData.Units.Count; i++)
        {
            AllAmount += currentCardData.Units[i].UnitAmount;
        }
        
        Title.text = currentCardData.Name;
        CardImage.sprite = currentCardData.Image;
        Amount.text = AllAmount.ToString();
        RankImage.sprite = currentCardData.RankImage;
        TypeImage.sprite = currentCardData.TypeImage;
        ChangeColorFromType();
    }
    
    private void ChangeColorFromType()
    {
        if (currentCardData.Golden)
        {
            BackGroundImage.color = new Color(1f, 1f, 0, 1f); // 회색
        }
        else
        {
            switch (currentCardData.Type)
            {
                case SAOCardDatabase.Type.Knight:
                    BackGroundImage.color = new Color(0.5f, 0.5f, 0.5f, 1f); // 회색
                    break;
                case SAOCardDatabase.Type.Viking:
                    BackGroundImage.color = new Color(0.6f, 0.8f, 1f, 1f); // 하늘색
                    break;
                case SAOCardDatabase.Type.Western:
                    BackGroundImage.color = new Color(1f, 0.2f, 0.2f, 1f); // 붉은색
                    break;
                default:
                    Debug.Log("알 수 없는 카드 타입입니다.");
                    break;
            }
        }
    }

    public SAOCardDatabase.UnitElement GetUnitsByUnit(GameObject unit)
    {
        for (int i = 0; i < currentCardData.Units.Count; i++)
        {
            if (currentCardData.Units[i].Unit == unit)
            {
                return currentCardData.Units[i];
            }
        }
        return null;
    }
}
