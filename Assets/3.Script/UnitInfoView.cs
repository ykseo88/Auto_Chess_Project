using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitInfoView : MonoBehaviour
{
    public GameObject unitInfoPanel;

    public UnitData currentUnitData;
    public GameObject currentUnit;
    
    public Image unitType;
    public Image unitRank;
    public TMP_Text unitName;
    public TMP_Text unitHp;
    public TMP_Text unitDamage;
    public TMP_Text unitMoveSpeed;
    public TMP_Text unitAttackSpeedRate;
    public TMP_Text unitAttackRange;
    
    public Sprite vikingSprite;
    public Sprite knightSprite;
    public Sprite westernSprite;
    
    public Sprite[] RankSprites = new Sprite[6];

    public Vector3 worldOffSet = new Vector3(0, 2f, 0);
    
    public Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;
    }


    private void Update()
    {
        UpdateInfo();
        OnOffPanel();
        UpdatePanelPosition();
    }

    private void OnDisable()
    {
        unitInfoPanel.SetActive(false);
    }
    

    private void UpdateInfo()
    {
        if (currentUnitData != null)
        {
            if (currentUnitData.isDead)
            {
                unitName.text = $"<color=red>{currentUnitData.Name}</color>";
            }
            else
            {
                unitName.text = currentUnitData.Name;
            }
        
            unitHp.text = $"{currentUnitData.HP}/{currentUnitData.MaxHp}";
            unitDamage.text = $"{currentUnitData.Damage}";
            unitMoveSpeed.text = $"{currentUnitData.MoveSpeed}";
            unitAttackSpeedRate.text = $"{currentUnitData.AttackRate}";
            unitAttackRange.text = $"{currentUnitData.AttackDistance}";

            switch (currentUnitData.Type)
            {
                case "Viking":
                    unitType.sprite = vikingSprite;
                    break;
                case "Knight":
                    unitType.sprite = knightSprite;
                    break;
                case "Western":
                    unitType.sprite = westernSprite;
                    break;
                default:
                    unitType.sprite = vikingSprite;
                    break;
            }

            unitRank.sprite = RankSprites[currentUnitData.Rank - 1];
        }
        
    }

    private void UpdatePanelPosition()
    {
        if (currentUnit != null)
        {
            Vector3 screenPos = mainCam.WorldToScreenPoint(currentUnit.transform.position + worldOffSet);

            if (screenPos.z > 0)
            {
                unitInfoPanel.transform.position = screenPos;
            }
            else
            {
                unitInfoPanel.SetActive(false);
            }
        }
    }

    private void OnOffPanel()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hit; // 레이캐스트 결과 정보를 저장할 변수
            float maxRayDistance = 50f;

            if (Physics.Raycast(ray, out hit, maxRayDistance))
            {
                // 레이가 어떤 오브젝트와 충돌했을 때 실행될 코드
                Debug.Log("레이가 오브젝트와 충돌했습니다: " + hit.collider.name);
                Debug.Log("충돌 지점: " + hit.point);
                Debug.Log("충돌 노멀: " + hit.normal);

                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("BodyParts") || hit.collider.gameObject.layer == LayerMask.NameToLayer("Weapon") || hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    hit.collider.transform.root.TryGetComponent(out UnitData targetUnitData);
                    
                    currentUnit = hit.collider.gameObject;
                    currentUnit.transform.root.TryGetComponent(out currentUnitData);
                    
                    /*
                    currentUnitData.Name = targetUnitData.Name;
                    currentUnitData.Rank = targetUnitData.Rank;
                    currentUnitData.Type = targetUnitData.Type;
                    currentUnitData.HP = targetUnitData.HP;
                    currentUnitData.MaxHp =  targetUnitData.MaxHp;
                    currentUnitData.Damage = targetUnitData.Damage;
                    currentUnitData.MoveSpeed = targetUnitData.MoveSpeed;
                    currentUnitData.AttackDistance = targetUnitData.AttackDistance;
                    currentUnitData.AttackRate = targetUnitData.AttackRate;
                    currentUnitData.isDead =  targetUnitData.isDead;
                    */
                    unitInfoPanel.SetActive(true);
                }
                
                
            }
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            unitInfoPanel.SetActive(false);
        }
    }


}
