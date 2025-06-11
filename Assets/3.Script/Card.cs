using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public SAOCardDatabase cardDatabase;
    
    public SAOCardDatabase.CardData currentCardData;
    
    public Image TitleImage;
    public TMP_Text Title;
    public Image CardImage;
    public Image AmountImage;
    public TMP_Text Amount;
    public Image RankImage;
    public Image TypeImage;
    
    void Start()
    {
        InsertCard(cardDatabase.cards[0]);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            InsertCard(cardDatabase.cards[0]);
        }
    }

    public void InsertCard(SAOCardDatabase.CardData insertData)
    {
        
        currentCardData = insertData;
        
        int allAmount = 0;
        for (int i = 0; i < currentCardData.Units.Count; i++)
        {
            allAmount += currentCardData.Units[i].UnitAmount;
        }
        
        Title.text = currentCardData.Name;
        CardImage.sprite = currentCardData.Image;
        Amount.text = allAmount.ToString();
        RankImage.sprite = currentCardData.RankImage;
        TypeImage.sprite = currentCardData.TypeImage;
    }
    
}
