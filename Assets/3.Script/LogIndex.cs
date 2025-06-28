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

    public int FinalFieldIndex;
    
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
        SaveData loadData = LoadManager.Instance.loadData;
        for (int i = 0; i < loadData.logSave[FinalFieldIndex].finalField.Count; i++)
        {
            GameObject tempCard = Instantiate(cardPrefab, finalFieldPanel.transform);
            tempCard.transform.TryGetComponent(out Card card);
            card.isMove = false;
            LoadManager.Instance.LoadCard(card, loadData.logSave[FinalFieldIndex].finalField[i]);
            
        }
        finalFieldPanel.SetActive(true);
        CloseButton.SetActive(true);
    }
}
