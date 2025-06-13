using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public SAOCardDatabase cardDatabase;
    
    public SAOCardDatabase.CardData currentCardData;
    
    public List<GameObject> parentList;
    public GameObject returnParent;
    public Shop.CardInven inventorys;
    
    public Image TitleImage;
    public TMP_Text Title;
    public Image CardImage;
    public Image AmountImage;
    public TMP_Text Amount;
    public Image RankImage;
    public Image TypeImage;
    public int SellPrice = 1;
    public int BuyPrice = 3;

    private Shop shop;
    private Field field;
    private ReadyTurn readyTurn;
    
    private CanvasGroup canvasGroup;
    private Canvas canvas;
    
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        shop = GetComponentInParent<Shop>();
        GameObject tempField = GameObject.Find("Field");
        tempField.transform.TryGetComponent(out field);
        transform.root.TryGetComponent(out readyTurn);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            InsertCard(cardDatabase.cards[0]);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            UnitCountUpTest();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            DebugLogTest();
        }

        OnAbility();
    }

    public void InsertCard(SAOCardDatabase.CardData insertData)
    {
        
        InputCardData(insertData);
        
        int allAmount = 0;
        for (int i = 0; i < currentCardData.Units.Count; i++)
        {
            allAmount += currentCardData.Units[i].UnitAmount;
        }
        
        Title.text = currentCardData.Name;
        CardImage.sprite = currentCardData.Image;
        Amount.text = allAmount.ToString();
        RankImage.sprite = currentCardData.RankImage;
        TypeImage.sprite = currentCardData.TypeImage;
    }

    public void InputCardData(SAOCardDatabase.CardData insertData)
    {
        currentCardData.ID = insertData.ID;
        currentCardData.Name = insertData.Name;
        currentCardData.Rank = insertData.Rank;
        currentCardData.Type = insertData.Type;
        currentCardData.Units.Clear();
        for (int i = 0; i < insertData.Units.Count; i++)
        {
            currentCardData.Units.Add(new SAOCardDatabase.UnitElement());
            currentCardData.Units[i].UnitAmount = insertData.Units[i].UnitAmount;
            currentCardData.Units[i].UnitName = insertData.Units[i].UnitName;
            currentCardData.Units[i].Unit = insertData.Units[i].Unit;
        }
        currentCardData.Description = insertData.Description;
        currentCardData.Image = insertData.Image;
        currentCardData.TypeImage = insertData.TypeImage;
        currentCardData.RankImage = insertData.RankImage;
        currentCardData.BackgroundImage = insertData.BackgroundImage;
        currentCardData.Triple = insertData.Triple;
        currentCardData.MaxNum = insertData.MaxNum;
        
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        returnParent = transform.parent.gameObject;
        transform.SetParent(transform.root);
        canvasGroup.blocksRaycasts = false;
        inventorys.currentNum--;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GameObject target = eventData.pointerCurrentRaycast.gameObject;
        switch (target.name)
        {
            case "Field":
                target.transform.TryGetComponent(out Field targetField);
                if (targetField.fieldCards.Count == 7 || field.Gold < BuyPrice)
                {
                    returnToParent();
                    break;
                }
                transform.SetParent(target.transform);
                parentList.Remove(gameObject);
                parentList = targetField.fieldCards;
                parentList.Add(gameObject);
                shop.currentSellCards.Remove(gameObject);
                field.Gold -= BuyPrice;
                shop.SetSellCards();
                break;
            case "Shop":
                if (parentList == field.fieldCards)
                {
                    parentList.Remove(gameObject);
                    inventorys.currentNum++;
                    field.Gold += SellPrice;
                    Destroy(gameObject);
                }
                else
                {
                    returnToParent();
                }
                break;
            default:
                returnToParent();
                break;
        }
        field.UpdateGold();
        canvasGroup.blocksRaycasts = true;
    }
    
    public void returnToParent()
    {
        transform.SetParent(returnParent.transform);
        inventorys.currentNum++;
    }
    
    private void DebugLogTest()
    {
        Debug.Log($"카드 이름: {currentCardData.Name}");
        Debug.Log($"카드 타입: {currentCardData.Type}");
        Debug.Log($"카드 등급: {currentCardData.Rank}");
        Debug.Log($"카드 이미지: {currentCardData.Image.name}");
        Debug.Log($"카드 설명: {currentCardData.Description}");

        for (int i = 0; i < currentCardData.Units.Count; i++)
        {
            Debug.Log($"카드 유닛: {currentCardData.Units[i].UnitName}, 수량: {currentCardData.Units[i].UnitAmount}");
        }
    }

    private void UnitCountUpTest()
    {
        currentCardData.Units[0].UnitAmount++;
        UpdateCardInfo();
    }

    public void UpdateCardInfo()
    {
        int allAmount = 0;
        for (int i = 0; i < currentCardData.Units.Count; i++)
        {
            allAmount += currentCardData.Units[i].UnitAmount;
        }
        Title.text = currentCardData.Name;
        CardImage.sprite = currentCardData.Image;
        Amount.text = allAmount.ToString();
        RankImage.sprite = currentCardData.RankImage;
        TypeImage.sprite = currentCardData.TypeImage;
    }

    private void OnAbility()
    {
        if (parentList == field.fieldCards)
        {
            Debug.Log($"{currentCardData.Name} 카드의 능력이 발동 중입니다.");
            AbilityManager.Instance.abilityDictionary[currentCardData.Name](gameObject);
        }
    }
}
