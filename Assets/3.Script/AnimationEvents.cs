using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    private Transform Unit;
    private UnitController unitController;
    private UnitData unitData;
    public GameObject projectilePrefab;
    public float projectileSpeed;

    private void Start()
    {
        Unit = transform.parent;
        Unit.TryGetComponent(out unitController);
        Unit.TryGetComponent(out unitData);
    }

    private void GiveDamage()
    {
        unitController.closeTarget.TryGetComponent(out UnitData targetUnitData);
        targetUnitData.HP -= unitData.Damage;
    }

    private void GiveRangeDamage()
    {
        
    }
}
