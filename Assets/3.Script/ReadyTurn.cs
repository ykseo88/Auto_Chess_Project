using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReadyTurn : MonoBehaviour
{
    public Button StartButton;
    public Button ReRollButton;
    public Button UpgradeButton;
    
    public Field field;
    public Shop shop;
    public Hand hand;
    public Choice choice;
    public TMP_Text Gold;

    private void CombatStart()
    {
        GameManager.Instance.isReadyTurn = false;
        GameManager.Instance.isCombatTurn = true;
        GameManager.Instance.isFightStart = false;
        
        gameObject.SetActive(false);
    }


    private void Start()
    {
        StartButton.onClick.AddListener(CombatStart);
        ReRollButton.onClick.AddListener(shop.ReRoll);
        UpgradeButton.onClick.AddListener(shop.UpgradeShop);
    }

    private void Update()
    {
        
    }
}
