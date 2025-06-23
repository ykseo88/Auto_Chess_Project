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
    
    public GameObject SpawnCardPrefab;

    public GameObject CardPanel;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        CombatStartButton.onClick.AddListener(CombatStart);
    }

    private void OnEnable()
    {
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CombatStart()
    {
        GameManager.Instance.isReadyTurn = false;
        GameManager.Instance.isAssignTurn = false;
        GameManager.Instance.isCombatTurn = true;
        GameManager.Instance.isFightStart = false;
        
        gameObject.SetActive(false);
    }
}
