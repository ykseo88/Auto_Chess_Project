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
    public Transform target;
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
    }

    // Update is called once per frame
    void Update()
    {
        currentState.Update();
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
        targetDistance = Vector3.Distance(transform.position, target.position);
    }
}
