using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitAssignTrun : MonoBehaviour
{
    public Field field;
    
    public List<GameObject> SpawnCards= new List<GameObject>();
    
    public Button CombatStartButton;
    public Button ResetButton;
    
    public GameObject SpawnCardPrefab;

    public GameObject CardPanel;
    
    public SpawnCard currentSpawnCard;
    
    private Queue<GameObject> assignUnitList = new Queue<GameObject>();


    
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        CombatStartButton.onClick.AddListener(CombatStart);
        ResetButton.onClick.AddListener(Reset);
        MakeSpawnCards();
    }

    private void OnEnable()
    {
        MakeSpawnCards();
    }

    private void MakeSpawnCards()
    {
        for (int i = 0; i < SpawnCards.Count; i++)
        {
            Destroy(SpawnCards[i]);
        }
        SpawnCards.Clear();
        for (int i = 0; i < field.fieldCards.Count; i++)
        {
            GameObject temp = Instantiate(SpawnCardPrefab);
            temp.transform.TryGetComponent(out SpawnCard tempCard);
            tempCard.ParentPanel = CardPanel.transform;
            temp.transform.SetParent(tempCard.ParentPanel);
            
            field.fieldCards[i].transform.TryGetComponent(out Card card);
            
            tempCard.InsertCard(card.currentCardData);

            for (int j = 0; j < card.BuffRateDicArray.Length; j++)
            {
                float tempBuff = 0;
                foreach (var buff in card.BuffRateDicArray[j])
                {
                    tempBuff += buff.Value;
                }
                tempCard.Buffs[j] =  tempBuff;
            }
            
            SpawnCards.Add(temp);
        }
        
        SpawnCards[0].transform.TryGetComponent(out SpawnCard tempSpawnCard);
        ChangeCurrentCard(tempSpawnCard);
    }

    // Update is called once per frame
    void Update()
    {
        UnitAssign();
    }

    private void CombatStart()
    {
        GameManager.Instance.isReadyTurn = false;
        GameManager.Instance.isAssignTurn = false;
        GameManager.Instance.isCombatTurn = true;
        GameManager.Instance.isFightStart = false;
        
        gameObject.SetActive(false);
    }

    private void UnitAssign()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hit; // 레이캐스트 결과 정보를 저장할 변수
            float maxRayDistance = 50f;

            if (Physics.Raycast(ray, out hit, maxRayDistance))
            {
                // 레이가 어떤 오브젝트와 충돌했을 때 실행될 코드
                Debug.Log("레이가 오브젝트와 충돌했습니다: " + hit.collider.name);
                Debug.Log("충돌 지점: " + hit.point);
                Debug.Log("충돌 노멀: " + hit.normal);

                if (hit.collider.gameObject.CompareTag("Ground"))
                {
                    GameObject temp = assignUnitList.Dequeue();
                    GameObject tempUnit = Instantiate(temp, hit.point, Quaternion.LookRotation(GameManager.Instance.PlayerSpawnPoint.transform.forward));
                    tempUnit.transform.TryGetComponent(out UnitData unitData);
                    
                    unitData.HP *= currentSpawnCard.Buffs[0];
                    unitData.Damage *= currentSpawnCard.Buffs[1];
                    unitData.MoveSpeed *= currentSpawnCard.Buffs[2];
                    unitData.AttackRate *= currentSpawnCard.Buffs[3];
                    unitData.AttackDistance *= currentSpawnCard.Buffs[4];
                    
                    unitData.Team = GameManager.Instance.playerTeam;
                    GameManager.Instance.playerTeam.PlayerUnitList.Add(tempUnit);
                    currentSpawnCard.GetUnitsByUnit(temp).UnitAmount--;
                    currentSpawnCard.UpdateCardInfo();
                    
                    Debug.Log($"소환된 유닛: {unitData.name} 데미지: {unitData.Damage}");
                }
            }
        }
    }

    public void ChangeCurrentCard(SpawnCard spawnCard)
    {
        currentSpawnCard = spawnCard;
        
        SAOCardDatabase.CardData card = spawnCard.currentCardData;
        
        assignUnitList.Clear();
        for (int i = 0; i < card.Units.Count; i++)
        {
            for (int j = 0; j < card.Units[i].UnitAmount; j++)
            {
                assignUnitList.Enqueue(card.Units[i].Unit);
            }
        }

        UpdateChoiceEffect();
    }

    public void Reset()
    {
        GameManager.Instance.DestroyPlayerUnits();
        foreach (GameObject card in SpawnCards)
        {
            Destroy(card);
        }
        SpawnCards.Clear();
        for (int i = 0; i < field.fieldCards.Count; i++)
        {
            GameObject temp = Instantiate(SpawnCardPrefab);
            temp.transform.TryGetComponent(out SpawnCard tempCard);
            tempCard.ParentPanel = CardPanel.transform;
            temp.transform.SetParent(tempCard.ParentPanel);
            
            field.fieldCards[i].transform.TryGetComponent(out Card card);
            
            tempCard.InsertCard(card.currentCardData);
            SpawnCards.Add(temp);
        }
        
        currentSpawnCard = SpawnCards[0].transform.GetComponent<SpawnCard>();
        ChangeCurrentCard(currentSpawnCard);
    }

    private void UpdateChoiceEffect()
    {
        for (int i = 0; i < SpawnCards.Count; i++)
        {
            SpawnCards[i].transform.TryGetComponent(out SpawnCard tempCard);
            Color color = tempCard.ChoiceBackGroundImage.color;
            color.b = 1;
            tempCard.ChoiceBackGroundImage.color = color;
        }

        Color onColor = currentSpawnCard.ChoiceBackGroundImage.color;
        onColor.b = 0;
        currentSpawnCard.ChoiceBackGroundImage.color = onColor;
    }
    
}
