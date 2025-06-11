using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{
    public string teamName;
    public List<GameObject> EnemyUnitList;
    public List<GameObject> PlayerUnitList;
    public int UnitAmount = 0;
    public int EnemyUnitAmount = 0;
    public Team EnemyTeam;

    private void Start()
    {
        /*
        GameObject[] temp = GameObject.FindGameObjectsWithTag("Unit");
        for (int i = 0; i < temp.Length; i++)
        {
            temp[i].transform.TryGetComponent(out UnitData unitData);
            if (unitData.Team != this)
            {
                EnemyUnitList.Add(temp[i]);
            }
        }
        */
        EnemyUnitList = EnemyTeam.PlayerUnitList;
    }

    private void Update()
    {
        UpdateEnemyList();
    }

    protected void UpdateEnemyList()
    {
        for(int i = 0; i < EnemyUnitList.Count; i++)
        {
            EnemyUnitList[i].transform.TryGetComponent(out UnitData unitData);
            if (unitData.isDead)
            {
                EnemyUnitList.RemoveAt(i);
                i--;
            }
        }
    }

    private void WinSign()
    {
        if(EnemyUnitList.Count == 0)
        {
            if (teamName == "Player")
            {
                GameManager.Instance.isPlayerWin = true;
            }
            else if (teamName == "Enemy")
            {
                GameManager.Instance.isEnemyWin = true;
            }
            
            GameManager.Instance.isFightStart = false;
            Debug.Log($"{teamName} 승리!");
        }
    }
}
