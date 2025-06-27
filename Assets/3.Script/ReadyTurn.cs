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

    public GameObject unitAssignTurn;
    public Camera Camera;
    
    public Field field;
    public Shop shop;
    public Hand hand;
    public RoundManager roundManager;
    
    public Choice choice;
    public TMP_Text Gold;
    public Text ReRollGold;
    public TMP_Text currentRound;
    
    public TimtLimit timerLimit;

    private IEnumerator ChangeToCombat()
    {
        float currentTime = 0f;
        
        while (currentTime < 2f)
        {
            currentTime += Time.deltaTime;
            yield return null;
        }

        Camera.transform.position = GameManager.Instance.cameraStartTransform.position;
        Camera.transform.rotation = GameManager.Instance.cameraStartTransform.rotation;
        GameManager.Instance.isReadyTurn = false;
        GameManager.Instance.isAssignTurn = true;
        GameManager.Instance.isCombatTurn = false;
        GameManager.Instance.isFightStart = false;
        GameManager.Instance.isCameraAssign = true;
        shop.UpgradePrice[shop.shopRank - 1]--;
        shop.UpdateUpgradeButton();
        unitAssignTurn.SetActive(true);
        gameObject.SetActive(false);
    }
    
    public void CombatStart()
    {
        transform.TryGetComponent(out ToolTip toolTip);
        toolTip.ToolTipPanel.SetActive(false);
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
        currentRound.text = roundManager.currentRoundIndex.ToString();
        timerLimit.tempTimeLimitSecond = timerLimit.inputTimeLimitSecond;
    }

    private void Start()
    {
        Camera = Camera.main;
        roundManager = GameObject.Find("RoundManager").GetComponent<RoundManager>();
        ReRollGold = ReRollButton.transform.GetChild(0).GetComponent<Text>();
        StartButton.onClick.AddListener(CombatStart);
        ReRollButton.onClick.AddListener(shop.ReRoll);
        UpgradeButton.onClick.AddListener(shop.UpgradeShop);
        LockButton.onClick.AddListener(shop.LockShop);
    }

    private void Update()
    {
        UpdateUIValue();
    }

    private void UpdateUIValue()
    {
        currentRound.text = roundManager.currentRoundIndex.ToString();
        ReRollGold.text = shop.ReRollPrice.ToString();
    }
}
