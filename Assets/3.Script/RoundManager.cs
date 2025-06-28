using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public Shop shop;
    public Field field;
    public Hand hand;
    
    public RoundInfo currentRoundInfo;
    public int currentRoundIndex = 1;
    
    public SAOCardDatabase cardDatabase;
    
    [System.Serializable]
    public class RoundInfo
    {
        public int roundIndex;
        public List<SAOCardDatabase.UnitElement> EnemyUnits = new List<SAOCardDatabase.UnitElement>();
        public GameObject Map;
    }
    
    public RoundInfo[] roundInfos = new RoundInfo[15];

    private void Start()
    {

        SetStartRound();

    }

    public void SetStartRound()
    {
        
        if (GameManager.Instance.isContinue)
        {
            currentRoundIndex = LoadManager.Instance.loadData.battleSave.saveRoundIndex;
        }
        
        currentRoundInfo = roundInfos[currentRoundIndex-1];
    }
}
