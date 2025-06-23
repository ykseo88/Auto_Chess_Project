using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Team playerTeam;
    public Team enemyTeam;

    public int startCountDown = 3;
    public int endCountDown = 3;
    
    public bool isFightStart = false;
    public bool isCombatTurn = false;
    public bool isReadyTurn = false;

    public bool isEnemyWin = false;
    public bool isPlayerWin = false;

    public Field field;
    public RoundManager roundManager;
    public GameObject readyUI;
    
    public Transform PlayerSpawnPoint;
    public Transform EnemySpawnPoint;

    public GameObject Camera;
    public Transform cameraStartTransform;
    
    public int maxPlusGold = 6;

    private void Awake()
    {
        Instance = this;
        
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if (isCombatTurn)
        {
            Camera.transform.position = cameraStartTransform.position;
            Camera.transform.rotation = cameraStartTransform.rotation;
            PlayerUnitSpawn();
            EnemyUnitSpawn();
            StartCoroutine(StartCountDown());
            isCombatTurn = false;
        }

        if (isPlayerWin)
        {
            StartCoroutine(EndCountDown());
            isPlayerWin = false;
        }
        
        TimeSlow();
    }

    private IEnumerator StartCountDown()
    {
        WaitForSeconds wfs = new WaitForSeconds(1f);
        int BackupCount = startCountDown;

        while (startCountDown >= 0)
        {
            yield return wfs;
            Debug.Log(startCountDown);
            startCountDown--;
        }

        startCountDown = BackupCount;
        isFightStart = true;
    }

    private void TimeSlow()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (Time.timeScale < 1)
            {
                Time.timeScale = 1;
            }
            else
            {
                Time.timeScale *= 0.01f;
            }
            
        }
    }
    
    private IEnumerator EndCountDown()
    {
        WaitForSeconds wfs = new WaitForSeconds(1f);
        int BackupCount = endCountDown;

        while (endCountDown >= 0)
        {
            yield return wfs;
            Debug.Log(endCountDown);
            endCountDown--;
        }

        ClearMap();
        if (maxPlusGold > 0)
        {
            field.maxGold++;
            maxPlusGold--;
        }
        readyUI.SetActive(true);
        isReadyTurn = true;
        isFightStart = false;
        roundManager.currentRoundIndex++;
        roundManager.currentRoundInfo = roundManager.roundInfos[roundManager.currentRoundIndex-1];
        endCountDown = BackupCount;
        OnCardTurnStart();
    }

    private void ClearMap()
    {
        playerTeam.PlayerUnitList.Clear();
        enemyTeam.PlayerUnitList.Clear();
        playerTeam.UnitAmount = 0;
        enemyTeam.UnitAmount = 0;
        playerTeam.EnemyUnitAmount = 0;
        enemyTeam.EnemyUnitAmount = 0;
        GameObject[] ClearList = GameObject.FindGameObjectsWithTag("Unit");
        for(int i = 0; i < ClearList.Length; i++)
        {
            Destroy(ClearList[i]);
        }
    }

    private void PlayerUnitSpawn()
    {
        Debug.Log(field.fieldCards);
        for (int i = 0; i < field.fieldCards.Count; i++)
        {
            field.fieldCards[i].transform.TryGetComponent(out Card card);
            if (card.currentCardData != null)
            {
                for (int j = 0; j < card.currentCardData.Units.Count; j++)
                {
                    for (int k = 0; k < card.currentCardData.Units[j].UnitAmount; k++)
                    {
                        Debug.Log(card.currentCardData.Units[j].UnitName);
                        GameObject temp = Instantiate(card.currentCardData.Units[j].Unit, PlayerSpawnPoint.position, Quaternion.identity);
                        temp.transform.TryGetComponent(out UnitData unitData);
                        unitData.Team = playerTeam;
                        playerTeam.PlayerUnitList.Add(temp);
                        Debug.Log("Player Unit Spawned");
                    }
                }
            }
        }
    }

    public void OnCardTurnEne()
    {
        foreach (GameObject cardObj in field.fieldCards)
        {
            cardObj.transform.TryGetComponent(out Card card);
            card.turnEnd = true;
            card.turnStart = false;
        }
    }
    
    private void OnCardTurnStart()
    {
        foreach (GameObject cardObj in field.fieldCards)
        {
            cardObj.transform.TryGetComponent(out Card card);
            card.turnStart = true;
            card.turnEnd = false;
        }
    }

    private void EnemyUnitSpawn()
    {
        for (int i = 0; i < roundManager.currentRoundInfo.EnemyUnits.Count; i++)
        {
            for (int j = 0; j < roundManager.currentRoundInfo.EnemyUnits[i].UnitAmount; j++)
            {
                GameObject temp = Instantiate(roundManager.currentRoundInfo.EnemyUnits[i].Unit,EnemySpawnPoint.position, Quaternion.identity);
                temp.transform.TryGetComponent(out UnitData unitData);
                unitData.Team = enemyTeam;
                enemyTeam.PlayerUnitList.Add(temp);
                Debug.Log("Enemy Unit Spawned");
            }
        }
    }
}
    