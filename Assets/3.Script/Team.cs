using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{
    public string teamName;
    public List<GameObject> EnemyList;
    public int UnitAmount = 0;
    public Team EnemyTeam;

    private void Start()
    {
        GameObject[] temp = GameObject.FindGameObjectsWithTag("Unit");
        for (int i = 0; i < temp.Length; i++)
        {
            temp[i].transform.TryGetComponent(out UnitData unitData);
            if (unitData.Team != this)
            {
                EnemyList.Add(temp[i]);
            }
        }
    }

    private void Update()
    {
        UpdateEnemyList();
    }

    protected void UpdateEnemyList()
    {
        for(int i = 0; i < EnemyList.Count; i++)
        {
            EnemyList[i].transform.TryGetComponent(out UnitData unitData);
            if (unitData.isDead)
            {
                EnemyList.RemoveAt(i);
                i--; // Adjust index after removal
            }
        }
    }

    private void WinSign()
    {
        if(EnemyList.Count == 0)
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
