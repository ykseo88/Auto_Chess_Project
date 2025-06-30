using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    public List<GameObject> fieldCards;
    public int maxGold = 3;
    public int Gold = 10;
    public ReadyTurn readyTurn;
    public int consumedGold = 0;
    public int takeGold = 0;
    public int SellCardCount = 0;
    public int BuyCardCount = 0;
    public int preGold = 0;
    
    public GameObject cardPrefab;
    

    private void Start()
    {
        SetStart();
    }

    public void SetStart()
    {
        transform.root.TryGetComponent(out readyTurn);

        if (GameManager.Instance.isContinue)
        {
            SaveData loadData = LoadManager.Instance.loadData;
            
            maxGold = loadData.battleSave.saveMaxGold;
            Gold = loadData.battleSave.saveCurrentGold;
            consumedGold = loadData.battleSave.saveConsumeGold;
            takeGold = loadData.battleSave.saveTakeGold;
            SellCardCount = loadData.battleSave.saveSellCardCount;
            BuyCardCount = loadData.battleSave.saveBuyCardCount;
            preGold = loadData.battleSave.savePreGold;

            for (int i = 0; i < loadData.battleSave.saveFields.Count; i++)
            {
                GameObject tempCardObj = Instantiate(cardPrefab);
                tempCardObj.transform.SetParent(transform);
                tempCardObj.transform.TryGetComponent(out Card tempCard);
                
                LoadManager.Instance.LoadCard(tempCard, loadData.battleSave.saveFields[i]);

                tempCard.inventorys = GameManager.Instance.shop.GetInvenByID(tempCard.currentCardData.ID);
                tempCard.inventorys.currentNum--;
                tempCard.parentList = fieldCards;
                fieldCards.Add(tempCardObj);
                
                tempCard.UpdateCardInfo();
            }
        }
        
        UpdateGold();
    }

    public void UpdateGold()
    {
        readyTurn.Gold.text = Gold.ToString();
    }

    private void CheckGold()
    {
        if (!Gold.Equals(preGold))
        {
            int tempGold = Gold - preGold;
            if (tempGold < 0)
            {
                consumedGold += Mathf.Abs(tempGold);
            }
            else if (tempGold > 0)
            {
                takeGold += tempGold;
            }
        }

        preGold = Gold;
    }

    private void Update()
    {
        CheckGold();
        DebugLogTest();
    }

    public void UpadateFieldCardInfo()
    {
        foreach (GameObject card in fieldCards)
        {
            card.transform.TryGetComponent(out Card cardInfo);
            cardInfo.UpdateCardInfo();
        }
    }

    private void DebugLogTest()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log($"Gold: {Gold}, Consumed Gold: {consumedGold}, Take Gold: {takeGold}");
            Debug.Log($"Field Cards Count: {fieldCards.Count}");
            foreach (var card in fieldCards)
            {
                if (card != null)
                {
                    if (card.TryGetComponent(out Card cardComponent))
                    {
                        Debug.Log($"Card Name: {cardComponent.currentCardData.Name}");
                    }
                }
            }
        }
        
    }
}
