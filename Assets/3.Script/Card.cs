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

    private Shop shop;
    private Field field;
    
    private CanvasGroup canvasGroup;
    private Canvas canvas;
    
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        shop = GetComponentInParent<Shop>();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            InsertCard(cardDatabase.cards[0]);
        }
    }

    public void InsertCard(SAOCardDatabase.CardData insertData)
    {
        
        currentCardData = insertData;
        
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

    public void OnBeginDrag(PointerEventData eventData)
    {
        returnParent = transform.parent.gameObject;
        transform.SetParent(transform.root);
        canvasGroup.blocksRaycasts = false;
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
                if (targetField.fieldCards.Count == 7)
                {
                    returnToParent();
                    break;
                }
                transform.SetParent(target.transform);
                parentList.Remove(gameObject);
                targetField.fieldCards.Add(gameObject);
                parentList.Add(gameObject);
                shop.currentSellCards.Remove(gameObject);
                inventorys.currentNum--;
                shop.SetSellCards();
                break;
            case "Shop":
                parentList.Remove(gameObject);
                inventorys.currentNum++;
                Destroy(gameObject);
                break;
            default:
                returnToParent();
                break;
        }
        canvasGroup.blocksRaycasts = true;
    }
    
    public void returnToParent()
    {
        transform.SetParent(returnParent.transform);
    }
}
