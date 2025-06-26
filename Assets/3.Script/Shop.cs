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
    public CardInventroy cardInventroy;
    
    public Sprite lockImage;
    public Sprite unlockImage;
    public bool isLocked = false;
    
    private ToolTip toolTip;

    [System.Serializable]
    public class CardInven
    {
        public SAOCardDatabase.CardData card;
        public int maxNum;
        public int currentNum;
        
        public CardInven(SAOCardDatabase.CardData card, int maxNum, int currentNum)
        {
            this.card = card;
            this.maxNum = maxNum;
            this.currentNum = currentNum;
        }
    }

    public Dictionary<int, List<CardInven>> cardPool = new Dictionary<int, List<CardInven>>();
    public List<CardInven> currentAllowCards = new List<CardInven>();
    
    [SerializeField] private int[] shopCardNum = new int[6] { 3, 4, 4, 5, 5, 6 };
    
    private ReadyTurn readyTurn;
    
    public Dictionary<int, Card[]> sellCards = new Dictionary<int, Card[]>();

    public int shopRank = 1;
    public int maxRank = 6;
    public int ReRollPrice = 1;
    public int[] UpgradePrice = new int[5] { 5, 7, 8, 10, 10};
    public Field field;
    
    public Image currentShopRankImage;
    public Sprite[] shopRankImages;
    

    public List<GameObject> currentSellCards;

    private void Start()
    {
        transform.root.TryGetComponent(out toolTip);
        transform.root.TryGetComponent(out readyTurn);
        currentShopRankImage.sprite = shopRankImages[shopRank - 1];
        
        
        for (int i = 0; i < maxRank; i++)
        {
            sellCards.Add(i+1, new Card[shopCardNum[i]]);
            cardPool.Add(i+1, new List<CardInven>());
        }
        
        for (int i = 0; i < cardDatabase.cards.Count; i++)
        {
            CardInven temp = new CardInven(cardDatabase.cards[i], cardDatabase.cards[i].MaxNum, cardDatabase.cards[i].MaxNum);
            int tempRank = cardDatabase.cards[i].Rank;
            
            cardPool[tempRank].Add(temp);
        }
        
        SetSellCards();
        FreeReRoll();
        UpdateUpgradeButton();
    }

    public void LockShop()
    {
        isLocked = !isLocked;
        if (isLocked)
        {
            readyTurn.LockButton.GetComponent<Image>().sprite = lockImage;
        }
        else
        {
            readyTurn.LockButton.GetComponent<Image>().sprite = unlockImage;
        }
    }

    public void ReRoll()
    {
        
        if (toolTip.ToolTipPanel.activeSelf)
        {
            toolTip.ToolTipPanel.SetActive(false);
        }
        SetSellCards();
        if (field.Gold >= ReRollPrice)
        {
            field.Gold -= ReRollPrice;
            field.UpdateGold();
            while(currentSellCards.Count < shopCardNum[shopRank - 1] && currentSellCards.Count != currentAllowCards.Count)
            {
                GameObject tempCard = Instantiate(cardPrefab);
                tempCard.transform.SetParent(transform);
                currentSellCards.Add(tempCard);
            }
        
            CardInven[] backUpList = currentAllowCards.ToArray();
            for (int i = 0; i < currentSellCards.Count; i++)
            {
                currentSellCards[i].transform.TryGetComponent(out Card card);
                CardInven tempCardData = currentAllowCards[Random.Range(0, currentAllowCards.Count)];
                card.InsertCard(tempCardData.card);
                card.parentList = currentSellCards;
                card.inventorys = tempCardData;
                currentAllowCards.Remove(tempCardData);
            }
            currentAllowCards = backUpList.ToList();
            ReRollPrice = 1;
        }
    }

    public void FreeReRoll()
    {
        SetSellCards();
        while(currentSellCards.Count < shopCardNum[shopRank - 1] && currentSellCards.Count != currentAllowCards.Count)
        {
            GameObject tempCard = Instantiate(cardPrefab);
            tempCard.transform.SetParent(transform);
            currentSellCards.Add(tempCard);
        }
        
        CardInven[] backUpList = currentAllowCards.ToArray();
        for (int i = 0; i < currentSellCards.Count; i++)
        {
            currentSellCards[i].transform.TryGetComponent(out Card card);
            CardInven tempCardData = currentAllowCards[Random.Range(0, currentAllowCards.Count)];
            card.InsertCard(tempCardData.card);
            card.parentList = currentSellCards;
            card.inventorys = tempCardData;
            currentAllowCards.Remove(tempCardData);
        }
        currentAllowCards = backUpList.ToList();
    }
    
    public void LockedReRoll()
    {
        SetSellCards();
        List<GameObject> plusSellCards = new List<GameObject>();
        while(currentSellCards.Count < shopCardNum[shopRank - 1] && currentSellCards.Count != currentAllowCards.Count)
        {
            GameObject tempCard = Instantiate(cardPrefab);
            tempCard.transform.SetParent(transform);
            currentSellCards.Add(tempCard);
            plusSellCards.Add(tempCard);
        }
        
        CardInven[] backUpList = currentAllowCards.ToArray();
        for (int i = 0; i < plusSellCards.Count; i++)
        {
            plusSellCards[i].transform.TryGetComponent(out Card card);
            CardInven tempCardData = currentAllowCards[Random.Range(0, currentAllowCards.Count)];
            card.InsertCard(tempCardData.card);
            card.parentList = currentSellCards;
            card.inventorys = tempCardData;
            currentAllowCards.Remove(tempCardData);
        }
        currentAllowCards = backUpList.ToList();
    }
    
    public void UpgradeShop()
    {
        if (shopRank < maxRank && field.Gold >= UpgradePrice[shopRank - 1])
        {
            field.Gold -= UpgradePrice[shopRank - 1];
            shopRank++;
            UpdateUpgradeButton();
            field.UpdateGold();
        }
        Debug.Log("Clicked Upgrade Shop");
        currentShopRankImage.sprite = shopRankImages[shopRank - 1];
        SetSellCards();
    }
    
    public void UpdateUpgradeButton()
    {
        readyTurn.UpgradeButton.transform.GetChild(0).GetComponent<Text>().text = UpgradePrice[shopRank - 1].ToString();
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
        Debug.Log($"상점 등장 카드 수: {currentAllowCards.Count}");

        for (int i = 0; i < maxRank; i++)
        {
            Debug.Log($"{i+1}성 카드 목록");
            for (int j = 0; j < cardPool[i + 1].Count; j++)
            {
                Debug.Log($"{cardPool[i+1][j].card.Name}, {cardPool[i + 1][j].currentNum} / {cardPool[i + 1][j].maxNum}");
            }
        }
    }

    public void SetSellCards()
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

    public List<SAOCardDatabase.CardData> GetAllowCardList(int rank)
    {
        List<SAOCardDatabase.CardData> tempList = new List<SAOCardDatabase.CardData>();
        for (int i = 0; i < cardPool[rank].Count; i++)
        {
            for(int j = 0; j < cardPool[rank][i].currentNum; j++)
            {
                tempList.Add(cardPool[rank][i].card);
            }
        }
        return tempList;
    }
}
