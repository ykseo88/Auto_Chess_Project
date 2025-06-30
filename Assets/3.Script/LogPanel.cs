using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogPanel : MonoBehaviour
{
    public GameObject LogIndexPrefab;

    public Button FinalFieldViewCloseButton;

    public int MaxActiveNum = 5;
    
    public GameObject finalFieldPanel;
    public GameObject cardPrefab;
    
    void Start()
    {
        FinalFieldViewCloseButton.onClick.AddListener(CloseFieldView);
        
        SaveData loadData = LoadManager.Instance.loadData;
        
        for (int i = 0; i < MaxActiveNum; i++)
        {
            if (loadData.logSave.Count <= i) break;
            Log tempLog = loadData.logSave[i];
            GameObject tempObj = Instantiate(LogIndexPrefab, transform);
            tempObj.transform.TryGetComponent(out LogIndex tempIndex);
            tempIndex.finalFieldPanel = finalFieldPanel;
            tempIndex.CloseButton = FinalFieldViewCloseButton.gameObject;

            switch (tempLog.isWin)
            {
                case true:
                    tempIndex.result.text = "승리";
                    tempIndex.result.color = Color.yellow;
                    break;
                case false:
                    tempIndex.result.text = "패배";
                    tempIndex.result.color = Color.red;
                    break;
            }
            
            tempIndex.score.text = "달성 라운드: " + tempLog.score.ToString();

            tempIndex.FinalFieldIndex = i;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CloseFieldView()
    {
        GameManager.Instance.DestroyAllChildren(finalFieldPanel.transform);
        finalFieldPanel.SetActive(false);
    }
    
    
}
