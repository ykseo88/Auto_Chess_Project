using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogIndex : MonoBehaviour
{
    public TMP_Text result;
    public TMP_Text score;
    public Button finalFieldView;

    public List<Card> finalFieldList = new List<Card>();
    
    public GameObject finalFieldPanel;
    public GameObject CloseButton;
    public GameObject cardPrefab;
    
    void Start()
    {
        finalFieldView.onClick.AddListener(ActiveFieldPanel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ActiveFieldPanel()
    {
        for (int i = 0; i < finalFieldList.Count; i++)
        {
            GameObject tempCard = Instantiate(cardPrefab, finalFieldPanel.transform);
            tempCard.transform.TryGetComponent(out Card card);
            card.InputCardData(finalFieldList[i].currentCardData);
            card.isMove = false;
        }
        finalFieldPanel.SetActive(true);
        CloseButton.SetActive(true);
    }
}
