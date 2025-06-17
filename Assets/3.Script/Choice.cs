using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Choice : MonoBehaviour
{
    public GameObject[] choiceCards;
    private List<Shop.CardInven> allowChoiceCards = new List<Shop.CardInven>();
    public GameObject choicePanel;
    
    public Shop shop;
    public Hand hand;
    public Field field;

    private void Start()
    {
        choicePanel.SetActive(false);
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            OnChoice(shop.shopRank + 1);
        }
    }

    public void OnChoice(int rank)
    {
        choicePanel.SetActive(true);
        allowChoiceCards.Clear();
        for (int i = 0; i < shop.cardPool[rank].Count; i++)
        {
            for(int j = 0; j < shop.cardPool[rank][i].currentNum; j++)
            {
                allowChoiceCards.Add(shop.cardPool[rank][i]);
            }
        }

        for (int i = 0; i < choiceCards.Length; i++)
        {
            choiceCards[i].transform.TryGetComponent(out Card card);
            Shop.CardInven tempCardData = allowChoiceCards[Random.Range(0, allowChoiceCards.Count)];
            card.parentList = field.fieldCards;
            card.InsertCard(tempCardData.card);
            card.inventorys = tempCardData;
            card.UpdateCardInfo();
            card.choicePanel = choicePanel;
        }
    }
}
