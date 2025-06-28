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
            Log tempLog = loadData.logSave.Pop();
            GameObject tempObj = Instantiate(LogIndexPrefab, transform);
            tempObj.transform.TryGetComponent(out LogIndex tempIndex);
            tempIndex.finalFieldPanel = finalFieldPanel;

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
            
            for(int j = 0; j < tempLog.finalField.Count; j++)
            {
                Card card = new Card();
                tempIndex.finalFieldList.Add(card);
                LoadManager.Instance.LoadCard(card, tempLog.finalField[j]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CloseFieldView()
    {
        DestroyAllChildren(finalFieldPanel.transform);
        finalFieldPanel.SetActive(false);
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
