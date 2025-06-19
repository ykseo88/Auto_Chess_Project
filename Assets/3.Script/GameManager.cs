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
    public bool isFightStart = false;
    public bool isCombatTurn = false;
    public bool isReadyTurn = false;

    public bool isEnemyWin = false;
    public bool isPlayerWin = false;

    public Field playerField;
    
    public Transform PlayerSpawnPoint;
    public Transform EnemySpawnPoint;

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
            PlayerUnitSpawn();
            StartCoroutine(CountDown());
            isCombatTurn = false;
        }
    }

    private IEnumerator CountDown()
    {
        WaitForSeconds wfs = new WaitForSeconds(1f);

        while (startCountDown >= 0)
        {
            yield return wfs;
            Debug.Log(startCountDown);
            startCountDown--;
        }

        isFightStart = true;
    }

    private void PlayerUnitSpawn()
    {
        Debug.Log(playerField.fieldCards);
        for (int i = 0; i < playerField.fieldCards.Count; i++)
        {
            playerField.fieldCards[i].transform.TryGetComponent(out Card card);
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
                        
                        temp = Instantiate(card.currentCardData.Units[j].Unit, EnemySpawnPoint.position, Quaternion.identity);
                        temp.transform.TryGetComponent(out unitData);
                        unitData.Team = enemyTeam;
                        enemyTeam.PlayerUnitList.Add(temp);
                        Debug.Log("Spawned");
                    }
                }
            }
        }
    }
}
    