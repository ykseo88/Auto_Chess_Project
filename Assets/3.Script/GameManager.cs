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

    public bool isEnemyWin = false;
    public bool isPlayerWin = false;

    public Field playerField;

    private void Awake()
    {
        Instance = this;
        
    }

    private void Start()
    {
        PlayerUnitSpawn();
        StartCoroutine(CountDown());
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
        for (int i = 0; i < playerField.fieldCards.Length; i++)
        {
            if (playerField.fieldCards[i].currentCardData != null)
            {
                for (int j = 0; j < playerField.fieldCards[i].currentCardData.Units.Count; j++)
                {
                    for (int k = 0; k < playerField.fieldCards[i].currentCardData.Units[j].UnitAmount; k++)
                    {
                        GameObject temp = Instantiate(playerField.fieldCards[i].currentCardData.Units[j].Unit, Vector3.zero, Quaternion.identity);
                        temp.transform.TryGetComponent(out UnitData unitData);
                        unitData.Team = playerTeam;
                        playerTeam.PlayerUnitList.Add(temp);
                        
                        temp = Instantiate(playerField.fieldCards[i].currentCardData.Units[j].Unit, Vector3.right*10, Quaternion.identity);
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
    