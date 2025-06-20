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
    private int preGold = 0;
    

    private void Start()
    {
        transform.root.TryGetComponent(out readyTurn);
        UpdateGold();
    }

    public void UpdateGold()
    {
        readyTurn.Gold.text = $"{Gold.ToString()} Gold";
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
