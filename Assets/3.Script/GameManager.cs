using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public SaveData loadData = new SaveData();
    public bool isContinue = false;
    
    public SAOCardDatabase cardDatabase;
    public SAOUnitPrefabDatabase unitDatabase;
    public SAOSpriteDatabase spriteDatabase;

    public int pesonalIDNum = 0;

    public Team playerTeam;
    public Team enemyTeam;

    public int startCountDown = 3;
    public int endCountDown = 3;
    
    public bool isFightStart = false;
    public bool isCombatTurn = false;
    public bool isReadyTurn = false;
    public bool isAssignTurn = false;
    public bool isTitle = false;

    public bool isEnemyWin = false;
    public bool isPlayerWin = false;

    public Field field;
    public Shop shop;
    public Hand hand;
    public RoundManager roundManager;
    public GameObject readyUI;
    
    public Transform PlayerSpawnPoint;
    public Transform EnemySpawnPoint;

    public GameObject Camera;
    public Transform cameraStartTransform;
    public bool isCameraAssign = false;
    
    public Transform[] playerSpawnPoints;
    public Transform[] enemySpawnPoints;
    
    public int maxPlusGold = 6;
    
    public CombatTurn combatTurn;
    
    public CameraController cameraController;
    
    public bool isTimeChange = false;
    
    public SaveManager saveManager;
    
    public bool isSave = false;

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
            EnemyUnitSpawn();
            StartCoroutine(StartCountDown());
            isCombatTurn = false;
        }

        if (isPlayerWin)
        {
            combatTurn.resultImage.sprite = combatTurn.victorySprite;
            combatTurn.resultImage.gameObject.SetActive(true);

            if (roundManager.currentRoundIndex == 15)
            {
                if (!isSave)
                {
                    saveManager.SaveLog(true);
                    saveManager.ClearBattleSave();
                    saveManager.PushSaveData();
                    isSave = true;
                }
                combatTurn.RoundScore.gameObject.SetActive(true);
                combatTurn.backToTitle.gameObject.SetActive(true);
            }
            else
            {
                StartCoroutine(EndCountDown());
                isPlayerWin = false;
            }
        }
        else if(isEnemyWin)
        {
            if (!isSave)
            {
                saveManager.SaveLog(false);
                saveManager.ClearBattleSave();
                saveManager.PushSaveData();
                isSave = true;
            }
            combatTurn.resultImage.sprite = combatTurn.defeatSprite;
            combatTurn.resultImage.gameObject.SetActive(true);
            combatTurn.RoundScore.gameObject.SetActive(true);
            combatTurn.backToTitle.gameObject.SetActive(true);
        }
        
        TimeControll();
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
        
        combatTurn.countDownImage.gameObject.SetActive(false);
        cameraController.OffCusor();
        startCountDown = BackupCount;
        isFightStart = true;
    }

    private void TimeControll()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (isTimeChange)
            {
                Time.timeScale = 1;
                isTimeChange = false;
            }
            else
            {
                Time.timeScale *= 0.01f;
                isTimeChange = true;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (isTimeChange)
            {
                Time.timeScale = 1;
                isTimeChange = false;
            }
            else
            {
                Time.timeScale *= 0.01f;
                isTimeChange = true;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (isTimeChange)
            {
                Time.timeScale = 1;
                isTimeChange = false;
            }
            else
            {
                Time.timeScale *= 3f;
                isTimeChange = true;
            }
        }
    }

    private void TimeSlow()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (isTimeChange)
            {
                Time.timeScale = 1;
                isTimeChange = false;
            }
            else
            {
                Time.timeScale *= 3f;
                isTimeChange = true;
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
        combatTurn.resultImage.gameObject.SetActive(false);
        readyUI.SetActive(true);
        isReadyTurn = true;
        isFightStart = false;
        roundManager.currentRoundIndex++;
        roundManager.currentRoundInfo = roundManager.roundInfos[roundManager.currentRoundIndex-1];
        cameraController.OnCusor();
        endCountDown = BackupCount;
        if (isTimeChange)
        {
            Time.timeScale = 1;
            isTimeChange = false;
        }
        
        DestroyProjectile();
        OnCardTurnStart();
    }

    private IEnumerator GameOverCountDown()
    {
        WaitForSeconds wfs = new WaitForSeconds(1f);
        int BackupCount = endCountDown;

        while (endCountDown >= 0)
        {
            yield return wfs;
            Debug.Log(endCountDown);
            endCountDown--;
        }
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
                        //Debug.Log(card.currentCardData.Units[j].UnitName);
                        //GameObject temp = Instantiate(card.currentCardData.Units[j].Unit, PlayerSpawnPoint.position, Quaternion.identity);
                        //temp.transform.TryGetComponent(out UnitData unitData);
                        //unitData.Team = playerTeam;
                        //playerTeam.PlayerUnitList.Add(temp);
                        //Debug.Log("Player Unit Spawned");
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

    public void DestroyPlayerUnits()
    {
        for (int i = 0; i < playerTeam.PlayerUnitList.Count; i++)
        {
            Destroy(playerTeam.PlayerUnitList[i]);
        }
        playerTeam.PlayerUnitList.Clear();
        
    }

    private void DestroyProjectile()
    {
        GameObject[] DestroyList = GameObject.FindGameObjectsWithTag("Projectile");
        for (int i = 0; i < DestroyList.Length; i++)
        {
            Destroy(DestroyList[i]);
        }
    }

    public void DestroyAllChildren(Transform parent)
    {
        List<GameObject> childrenToDestroy = new List<GameObject>();
        foreach (Transform child in parent) // 'transform'은 이 스크립트가 붙은 오브젝트의 Transform
        {
            childrenToDestroy.Add(child.gameObject);
        }

        // 2. 임시 리스트에 있는 자식들을 파괴
        foreach (GameObject child in childrenToDestroy)
        {
            Destroy(child); // 또는 DestroyImmediate(child); 에디터에서 즉시 파괴할 때
        }

        Debug.Log(transform.name + "의 모든 자식 오브젝트가 파괴되었습니다.");
    }
}
    