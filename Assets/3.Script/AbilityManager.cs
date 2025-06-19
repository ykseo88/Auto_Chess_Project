using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance;
    
    public Dictionary<string, Action<GameObject>> abilityDictionary = new Dictionary<string, Action<GameObject>>();
    public SAOCardDatabase cardDatabase;
    public Field field;
    public Hand hand;
    public Shop shop;
    public ReadyTurn readyTurn;
    
    

    public void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InsertAbilityDictionary("SwordMan", SwordManAbility);
    }

    public void InsertAbilityDictionary(string CardName, Action<GameObject> abilityAction)
    {
        if (!abilityDictionary.ContainsKey(CardName))
        {
            abilityDictionary.Add(CardName, abilityAction);
        }
        else
        {
            Debug.LogWarning($"Ability {CardName} already exists in the dictionary.");
        }
    }
    
    public void SwordManAbility(GameObject cardObj)
    {
        // 3골드를 사용할 때마다 카드에 SwordMan 2기 추가
        int plusUnitNum = 2;
        int checkGold = 3;
        cardObj.transform.TryGetComponent(out Card card);

        if (card.intAbilityValue2 < 0)
        {
            card.intAbilityValue2 = field.consumedGold;
            card.intAbilityValue1 = 0;
        }
        
        card.intAbilityValue1 = field.consumedGold - card.intAbilityValue2;
        
        if(card.intAbilityValue1 >= checkGold)
        {
            Debug.Log("늘어남!");

            for (int j = 0; j < Mathf.FloorToInt(card.intAbilityValue1 / checkGold); j++)
            {
                for (int i = 0; i < card.currentCardData.Units.Count; i++)
                {
                    if (card.currentCardData.Units[i].UnitName == "SwordMan")
                    {
                        card.currentCardData.Units[i].UnitAmount += plusUnitNum;
                        break;
                    }
                }
            }

            int tempLeast = card.intAbilityValue1 % checkGold;
            int tempInt = card.intAbilityValue1 - tempLeast;
            card.intAbilityValue2 += tempInt;
            card.intAbilityValue1 = tempLeast;
            card.UpdateCardInfo();
        }
    }
}
