using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using Object = UnityEngine.Object;


public class CardDatabase : MonoBehaviour
{
    public static CardDatabase Instance;

    public Image testImage;
    public Sprite testSprite;
    
    public Dictionary<string , CardData> cardDatas = new Dictionary<string, CardData>();

    private void Awake()
    {
        Instance = this;
        LoadCardDatasCsv();
    }

    private void LoadCardDatasCsv()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("Data/CardDatas");
        if (csvFile == null)
        {
            Debug.LogError("CSV 파일이 없어요 확인 부탁드려요. 경로:Resources/Data/CardDatas.csv");
            return;
        }

        string[] lines = csvFile.text.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] parts = lines[i].Split(',');

            CardData card = new CardData
            {
                ID = parts[0],
                Name = parts[1],
                Rank = int.Parse(parts[2]),
                Unit = Resources.Load<GameObject>(parts[3]),
                Type = Enum.Parse<Type>(parts[4].Trim()),
                UnitAmount = int.Parse(parts[5]),
                Description = parts[6],
                Image = Resources.Load<Sprite>(parts[7]),
            };
            
            cardDatas.Add(card.ID, card);
        }
        
        Debug.Log($"로드된 카드 갯수: {cardDatas.Count}");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            testImage.sprite = cardDatas["Card_Knight_Lv1_1"].Image;
            Debug.Log("ID: "+cardDatas["Card_Knight_Lv1_1"].ID);
            Debug.Log("Name: "+cardDatas["Card_Knight_Lv1_1"].Name);
            Debug.Log("Rank: "+cardDatas["Card_Knight_Lv1_1"].Rank);
            Debug.Log("Unit: "+cardDatas["Card_Knight_Lv1_1"].Unit);
            Debug.Log("Type: "+cardDatas["Card_Knight_Lv1_1"].Type);
            Debug.Log("UnitAmount: "+cardDatas["Card_Knight_Lv1_1"].UnitAmount);
            Debug.Log("Description: "+cardDatas["Card_Knight_Lv1_1"].Description);
            Debug.Log(cardDatas["Card_Knight_Lv1_1"].Image);
        }
    }
}
