using System;
using System.Collections;
using System.Collections.Generic;
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
    public GameObject[] targets;
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
        targets = GameObject.FindGameObjectsWithTag("Unit");
        
    }

    // Update is called once per frame
    void Update()
    {
        currentState.Update();
        
        FindCloseTarget();
        UpdateTargetDistance();
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
        foreach (GameObject target in targets)
        {
            target.transform.TryGetComponent(out UnitData targetUnitData);
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance < minDistance && targetUnitData.Team != unitData.Team && !unitData.isDead)
            {
                minDistance = distance;
                closeTarget = target.transform;
            }
        }
    }
}
