using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class UnitController : MonoBehaviour
{
    public Animation AttackAnimation;
    public Animation IdleAnimation;
    public Animation RunAnimation;
    public Animation DieAnimation;
    
    public Animator animator;
    public UnitData unitData;
    public NavMeshAgent agent;
    
    public IUnitState currentState;
    //public List<GameObject> targets;
    public Transform closeTarget;
    public float targetDistance;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        transform.TryGetComponent(out unitData);
        transform.TryGetComponent(out agent);
    }

    void Start()
    {
        ChangeState(new WaitState(this));
        unitData.Team.UnitAmount++;
    }

    // Update is called once per frame
    void Update()
    {
        currentState.Update();
        
        FindCloseTarget();
        UpdateTargetDistance();
        UpdateDead();
    }

    public void ChangeState(IUnitState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
        Debug.Log($"현재 상태: {currentState}");
    }

    private void UpdateTargetDistance()
    {
        targetDistance = Vector3.Distance(transform.position, closeTarget.position);
    }
    
    private void FindCloseTarget()
    {
        float minDistance = float.MaxValue;
        foreach (GameObject target in unitData.Team.EnemyList.ToList())
        {
            target.transform.TryGetComponent(out UnitData targetUnitData);
            
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closeTarget = target.transform;
            }
        }
    }

    private void UpdateDead()
    {
        if(unitData.HP <= 0f && !unitData.isDead)
        {
            unitData.isDead = true;
        }
    }
}
