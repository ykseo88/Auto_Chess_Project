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
    public Button LockButton;
    
    public Field field;
    public Shop shop;
    public Hand hand;
    public Choice choice;
    public TMP_Text Gold;

    private IEnumerator ChangeToCombat()
    {
        float currentTime = 0f;
        
        while (currentTime < 2f)
        {
            currentTime += Time.deltaTime;
            yield return null;
        }
        
        
        
        GameManager.Instance.isReadyTurn = false;
        GameManager.Instance.isCombatTurn = true;
        GameManager.Instance.isFightStart = false;
        shop.UpgradePrice[shop.shopRank - 1]--;
        shop.UpdateUpgradeButton();
        gameObject.SetActive(false);
    }
    
    public void CombatStart()
    {
        GameManager.Instance.OnCardTurnEne();
        field.UpadateFieldCardInfo();
        StartCoroutine(ChangeToCombat());
    }

    private void OnEnable()
    {
        field = transform.GetComponentInChildren<Field>();
        shop = transform.GetComponentInChildren<Shop>();
        field.readyTurn = this;
        field.Gold = field.maxGold;
        field.consumedGold = 0;
        field.takeGold = 0;
        field.UpdateGold();
        if (!shop.isLocked)
        {
            shop.FreeReRoll();
        }
        else
        {
            shop.LockedReRoll();
        }
        field.UpdateGold();
    }

    private void Start()
    {
        StartButton.onClick.AddListener(CombatStart);
        ReRollButton.onClick.AddListener(shop.ReRoll);
        UpgradeButton.onClick.AddListener(shop.UpgradeShop);
        LockButton.onClick.AddListener(shop.LockShop);
    }

    private void Update()
    {
        
    }
}
