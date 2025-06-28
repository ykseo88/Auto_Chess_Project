using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public List<GameObject> handCards;
    
    public GameObject cardPrefab;

    private void Start()
    {
        SetStart();
    }

    public void SetStart()
    {
        if (GameManager.Instance.isContinue)
        {
            SaveData loadData = LoadManager.Instance.loadData;
            
            for (int i = 0; i < loadData.battleSave.saveHands.Count; i++)
            {
                GameObject tempCardObj = Instantiate(cardPrefab);
                tempCardObj.transform.SetParent(transform);
                tempCardObj.transform.TryGetComponent(out Card tempCard);
                
                LoadManager.Instance.LoadCard(tempCard, loadData.battleSave.saveHands[i]);
                
                tempCard.inventorys = GameManager.Instance.shop.GetInvenByID(tempCard.currentCardData.ID);
                tempCard.inventorys.currentNum--;
                
                handCards.Add(tempCardObj);
                tempCard.UpdateCardInfo();
            }
        }
    }
}
