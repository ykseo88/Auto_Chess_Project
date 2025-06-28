using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public int personalID;
    public SAOCardDatabase cardDatabase;
    
    public SAOCardDatabase.CardData currentCardData;
    
    public List<GameObject> parentList;
    public GameObject returnParent;
    public Shop.CardInven inventorys;
    
    public Image BackGroundImage;
    public Image TitleImage;
    public TMP_Text Title;
    public Image CardImage;
    public Image AmountImage;
    public TMP_Text Amount;
    public int AllAmount;
    public Image RankImage;
    public Image TypeImage;
    public int SellPrice = 1;
    public int BuyPrice = 3;

    private Shop shop;
    private Field field;
    private Hand hand;
    private ReadyTurn readyTurn;
    private TripleSystem tripleSystem;
    
    private CanvasGroup canvasGroup;
    private Canvas canvas;

    public int intAbilityValue1;
    public int intAbilityValue2;
    public float floatAbilityValue = 0f;
    public bool boolAbilityValue = false;

    public bool isChoiceCard = false;
    public GameObject choicePanel;
    
    public bool turnEnd = false;
    public bool turnStart = false;
    public bool isWaitDestroy = false;
    public bool isSellNow = false;
    public bool isFieldChanged = false;
    
    public Dictionary<int, float> HealthRateDic = new Dictionary<int, float>();
    public Dictionary<int, float> DamageRateDic = new Dictionary<int, float>();
    public Dictionary<int, float> AttackSpeedRateDic = new Dictionary<int, float>();
    public Dictionary<int, float> AttackRangeRateDic = new Dictionary<int, float>();
    public Dictionary<int, float> MoveSpeedRateDic = new Dictionary<int, float>();
    
    public Dictionary<int, float>[] BuffRateDicArray = new Dictionary<int, float>[5];

    [SerializeField] private float customScale = 0.7f;

    public bool isMove = true;
    public bool isClick = true;
    
    void Start()
    {
        SetStarting();
        personalID = GameManager.Instance.pesonalIDNum;
        GameManager.Instance.pesonalIDNum++;
        
        BuffRateDicArray[0] = HealthRateDic;
        BuffRateDicArray[1] = DamageRateDic;
        BuffRateDicArray[2] = MoveSpeedRateDic;
        BuffRateDicArray[3] = AttackSpeedRateDic;
        BuffRateDicArray[4] = AttackRangeRateDic;
        
        HealthRateDic.Add(-1, 1f);
        DamageRateDic.Add(-1, 1f);
        AttackSpeedRateDic.Add(-1, 1f);
        AttackRangeRateDic.Add(-1, 1f);
        MoveSpeedRateDic.Add(-1, 1f);
    }

    public void SetStarting()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        GameObject tempShop = GameObject.Find("Shop");
        GameObject tempField = GameObject.Find("Field");
        GameObject tempHand = GameObject.Find("Hand");
        GameObject tripleSystemObject = GameObject.Find("TripleSystem");
        tempShop.transform.TryGetComponent(out shop);
        tempField.transform.TryGetComponent(out field);
        tempHand.transform.TryGetComponent(out hand);
        tripleSystemObject.TryGetComponent(out tripleSystem);
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
        //SetScale();
    }

    public void InsertCard(SAOCardDatabase.CardData insertData)
    {
        
        InputCardData(insertData);
        UpdateCardInfo();
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
            currentCardData.Units.Add(new SAOCardDatabase.UnitElement(insertData.Units[i].Unit, insertData.Units[i].UnitName, insertData.Units[i].UnitAmount));
        }
        currentCardData.Description = insertData.Description;
        currentCardData.GoldenDescription = insertData.GoldenDescription;
        currentCardData.Image = insertData.Image;
        currentCardData.TypeImage = insertData.TypeImage;
        currentCardData.RankImage = insertData.RankImage;
        currentCardData.BackgroundImage = insertData.BackgroundImage;
        currentCardData.Golden = insertData.Golden;
        currentCardData.MaxNum = insertData.MaxNum;
        
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isMove)
        {
            if (!isChoiceCard)
            {
                returnParent = transform.parent.gameObject;
                transform.SetParent(transform.root);
                canvasGroup.blocksRaycasts = false;
                if (parentList == shop.currentSellCards)
                {
                    inventorys.currentNum--;
                }
            }
        }
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
                if (field.fieldCards.Count == 7)
                {
                    returnToParent();
                    break;
                }
                if (parentList == shop.currentSellCards && field.Gold >= BuyPrice)
                {
                    field.Gold -= BuyPrice;
                    field.BuyCardCount++;
                    shop.currentSellCards.Remove(gameObject);
                    shop.SetSellCards();
                }
                else if (parentList == hand.handCards)
                {
                    
                }
                else
                {
                    returnToParent();
                    break;
                }
                transform.SetParent(target.transform);
                parentList.Remove(gameObject);
                parentList = field.fieldCards;
                parentList.Add(gameObject);
                break;
            case "Hand":
                if (hand.handCards.Count >= 6)
                {
                    returnToParent();
                    break;
                }
                if (parentList == shop.currentSellCards && field.Gold >= BuyPrice)
                {
                    field.Gold -= BuyPrice;
                    field.BuyCardCount++;
                    shop.currentSellCards.Remove(gameObject);
                    shop.SetSellCards();
                }
                else
                {
                    returnToParent();
                    break;
                }
                transform.SetParent(target.transform);
                parentList.Remove(gameObject);
                parentList = hand.handCards;
                parentList.Add(gameObject);
                break;
            case "Shop":
                if (parentList == field.fieldCards || parentList == hand.handCards)
                {
                    field.SellCardCount++;
                    parentList.Remove(gameObject);
                    inventorys.currentNum++;
                    field.Gold += SellPrice;
                    OnSell();
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
        tripleSystem.CheckTriple();
        canvasGroup.blocksRaycasts = true;
    }
    
    public void returnToParent()
    {
        transform.SetParent(returnParent.transform);
        if (parentList == shop.currentSellCards)
        {
            inventorys.currentNum++;
        }
    }

    public void SellCard()
    {
        
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
        AllAmount = 0;
        for (int i = 0; i < currentCardData.Units.Count; i++)
        {
            AllAmount += currentCardData.Units[i].UnitAmount;
        }
        
        Title.text = currentCardData.Name;
        CardImage.sprite = currentCardData.Image;
        Amount.text = AllAmount.ToString();
        RankImage.sprite = currentCardData.RankImage;
        TypeImage.sprite = currentCardData.TypeImage;
        ChangeColorFromType();
    }

    private void OnAbility()
    {
        if (parentList == field.fieldCards && AbilityManager.Instance.abilityDictionary.ContainsKey(currentCardData.Name) && !isChoiceCard)
        {
            //Debug.Log($"{currentCardData.Name} 카드의 능력이 발동 중입니다.");
            AbilityManager.Instance.abilityDictionary[currentCardData.Name](gameObject);
        }
    }

    private void ChangeColorFromType()
    {
        if (currentCardData.Golden)
        {
            BackGroundImage.color = new Color(1f, 1f, 0, 1f); // 회색
        }
        else
        {
            switch (currentCardData.Type)
            {
                case SAOCardDatabase.Type.Knight:
                    BackGroundImage.color = new Color(0.5f, 0.5f, 0.5f, 1f); // 회색
                    break;
                case SAOCardDatabase.Type.Viking:
                    BackGroundImage.color = new Color(0.6f, 0.8f, 1f, 1f); // 하늘색
                    break;
                case SAOCardDatabase.Type.Western:
                    BackGroundImage.color = new Color(1f, 0.2f, 0.2f, 1f); // 붉은색
                    break;
                default:
                    Debug.Log("알 수 없는 카드 타입입니다.");
                    break;
            }
        }
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isChoiceCard)
        {
            if (Input.GetMouseButtonDown(0))
            {
                GameObject choiceCard = eventData.pointerCurrentRaycast.gameObject;
                if (hand.handCards.Count < 6)
                {
                    Debug.Log("카드 선택됨: " + currentCardData.Name);
                    GameObject tempCard = Instantiate(gameObject, hand.transform);
                    tempCard.transform.localScale = new Vector3(0.7f, 0.7f, 1);
                    tempCard.transform.TryGetComponent(out Card tempCardComponent);
                    tempCardComponent.InsertCard(currentCardData);
                    tempCardComponent.isChoiceCard = false;
                    tempCardComponent.parentList = hand.handCards;
                    tempCardComponent.inventorys = inventorys;
                    inventorys.currentNum--;
                    tempCardComponent.SetStarting();
                    tempCardComponent.parentList.Add(tempCard);
                
                }
                choicePanel.SetActive(false);
            }
        }
    }

    private void SetScale()
    {
        if (transform.localScale.x != customScale)
        {
            transform.localScale = new Vector3(customScale, customScale, 1);
        }
    }

    public void OnSell()
    {
        if (!isWaitDestroy)
        {
            Destroy(gameObject);
        }
        else
        {
            isSellNow = true;
        }
    }

    
    
}
