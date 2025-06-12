using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Shop : MonoBehaviour
{
    public SAOCardDatabase cardDatabase;
    public GameObject cardPrefab;

    public class CardPool
    {
        public SAOCardDatabase.CardData card;
        public int maxNum;
        public int currentNum;
        
        public CardPool(SAOCardDatabase.CardData card, int maxNum, int currentNum)
        {
            this.card = card;
            this.maxNum = maxNum;
            this.currentNum = currentNum;
        }
    }

    private Dictionary<int, List<CardPool>> cardPool = new Dictionary<int, List<CardPool>>();
    private List<CardPool> currentAllowCards = new List<CardPool>();
    
    [SerializeField] private int[] shopCardNum = new int[6] { 3, 4, 4, 5, 5, 6 };
    
    private ReadyTurn readyTurn;
    public Dictionary<int, Card[]> sellCards = new Dictionary<int, Card[]>();

    public int shopRank = 1;
    public int maxRank = 6;
    
    public Image currentShopRankImage;
    public Sprite[] shopRankImages;
    

    public List<GameObject> currentSellCards;

    private void Start()
    {
        transform.root.TryGetComponent(out readyTurn);
        currentShopRankImage.sprite = shopRankImages[shopRank - 1];
        
        
        for (int i = 0; i < maxRank; i++)
        {
            sellCards.Add(i+1, new Card[shopCardNum[i]]);
            cardPool.Add(i+1, new List<CardPool>());
        }
        
        for (int i = 0; i < cardDatabase.cards.Count; i++)
        {
            CardPool temp = new CardPool(cardDatabase.cards[i], cardDatabase.cards[i].MaxNum, cardDatabase.cards[i].MaxNum);
            int tempRank = cardDatabase.cards[i].Rank;
            
            cardPool[tempRank].Add(temp);
        }
        
        SetSellCards();
        ReRoll();
    }

    public void ReRoll()
    {
        while (currentSellCards.Count < shopCardNum[shopRank - 1])
        {
            GameObject tempCard = Instantiate(cardPrefab);
            tempCard.transform.SetParent(transform);
            currentSellCards.Add(tempCard);
        }
        
        CardPool[] backUpList = currentAllowCards.ToArray();

        for (int i = 0; i < currentSellCards.Count; i++)
        {
            currentSellCards[i].transform.TryGetComponent(out Card card);
            CardPool tempCardData = currentAllowCards[Random.Range(0, currentAllowCards.Count)];
            card.InsertCard(tempCardData.card);
            currentAllowCards.Remove(tempCardData);
        }
        
        currentAllowCards = backUpList.ToList();
    }
    
    public void UpgradeShop()
    {
        if (shopRank < maxRank) shopRank++;
        Debug.Log("Clicked Upgrade Shop");
        currentShopRankImage.sprite = shopRankImages[shopRank - 1];
        SetSellCards();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ShopInventoryDebugLog();
        }
    }

    public void ShopInventoryDebugLog()
    {
        Debug.Log("상점 재고 목록");
        Debug.Log($"현재 등록된 카드 수: {cardDatabase.cards.Count}");

        for (int i = 0; i < maxRank; i++)
        {
            Debug.Log($"{i+1}성 카드 목록");
            for (int j = 0; j < cardPool[i + 1].Count; j++)
            {
                Debug.Log($"{cardPool[i+1][j].card.Name}, {cardPool[i + 1][j].currentNum} / {cardPool[i + 1][j].maxNum}");
            }
        }
    }

    private void SetSellCards()
    {
        currentAllowCards.Clear();
        for (int i = 0; i < shopRank; i++)
        {
            for (int j = 0; j < cardPool[i + 1].Count; j++)
            {
                for (int k = 0; k < cardPool[i + 1][j].currentNum; k++)
                {
                    currentAllowCards.Add(cardPool[i + 1][j]);
                }
            }
        }
        
        Debug.Log("현재 허용된 카드 목록: ");
        for (int i = 0; i < currentAllowCards.Count; i++)
        {
            Debug.Log(currentAllowCards[i].card.Name);
        }
    }
}
