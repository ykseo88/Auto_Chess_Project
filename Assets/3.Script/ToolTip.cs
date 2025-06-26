using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour
{
    public GameObject unitAmountInfoPrefab;
    public Image rankImage;
    public Image typeImage;
    public TMP_Text Title;
    public TMP_Text Description;
    public GameObject UnitInfo;
    public GameObject ToolTipPanel;
    
    public SAOCardDatabase.CardData currentCardData;

    private void Update()
    {
        OnOffToolTip();
        InputToolTipInput(currentCardData);
    }

    private void UpdateTooltipInfo()
    {
        
    }

    private void OnOffToolTip()
    {
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            // 1. PointerEventData 객체 생성
            //    이 객체에 현재 마우스 커서의 위치 정보를 담습니다.
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition; // 마우스 스크린 좌표 할당

            // 2. Raycast 결과를 저장할 리스트 생성
            //    하나의 클릭 위치에 여러 UI 요소가 겹쳐 있을 수 있으므로 리스트로 받습니다.
            List<RaycastResult> results = new List<RaycastResult>();

            // 3. EventSystem을 통해 UI Raycast 수행
            //    마우스 위치에서 UI 레이캐스트를 발사하고, 결과를 'results' 리스트에 채웁니다.
            EventSystem.current.RaycastAll(pointerEventData, results);

            // 4. Raycast 결과 처리
            if (results.Count > 0)
            {
                Debug.Log("--- UI Raycast 결과 ---");
                Debug.Log($"클릭 지점에 총 {results.Count}개의 UI 요소가 감지되었습니다.");

                // results 리스트는 UI 렌더링 순서(depth)에 따라 정렬됩니다.
                // results[0]은 가장 앞에 있는(화면상으로 가장 위에 보이는) UI 요소입니다.
                RaycastResult topMostUI = results[0];
                
                Debug.Log("eventData.button");
                GameObject temp = topMostUI.gameObject;
                Card tempCard = topMostUI.gameObject.GetComponentInParent<Card>();
                SpawnCard tempSpawnCard = temp.GetComponent<SpawnCard>();
                if (temp.CompareTag("Card"))
                {
                    if(tempCard != null) currentCardData = tempCard.currentCardData;
                    if(tempSpawnCard != null) currentCardData = tempSpawnCard.currentCardData;
                    ToolTipPanel.transform.position  = temp.transform.position;
                    ToolTipPanel.gameObject.SetActive(true);
                }
                else if(temp.gameObject.Equals(ToolTipPanel))
                {
                    ToolTipPanel.gameObject.SetActive(false);
                }
                
            }
            else
            {
                Debug.Log("클릭된 UI 요소가 없습니다.");
            }
        }
    }

    private void InputToolTipInput(SAOCardDatabase.CardData inputData)
    {
        Title.text = inputData.Name;
        if (inputData.Golden)
        {
            Description.text = inputData.GoldenDescription;
        }
        else
        {
            Description.text = inputData.Description;
        }
        rankImage.sprite = inputData.RankImage;
        typeImage.sprite = inputData.TypeImage;

        DestroyAllChild();

        for (int i = 0; i < inputData.Units.Count; i++)
        {
            GameObject temp = Instantiate(unitAmountInfoPrefab);
            temp.transform.SetParent(UnitInfo.transform);
            temp.transform.TryGetComponent(out TMP_Text infoText);
            infoText.text = $"{inputData.Units[i].UnitName} : {inputData.Units[i].UnitAmount}";
        }


    }

    private void DestroyAllChild()
    {
        List<GameObject> childrens = new List<GameObject>();
        foreach (Transform child in UnitInfo.transform)
        {
            childrens.Add(child.gameObject);
        }

        foreach (GameObject child in childrens)
        {
            Destroy(child);
        }
    }
}
