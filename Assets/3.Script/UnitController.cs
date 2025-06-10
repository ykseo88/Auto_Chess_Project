using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    public Animation AttackAnimation;
    public Animation IdleAnimation;
    public Animation RunAnimation;
    public Animation DieAnimation;
    
    private Animator animator;
    private UnitData unitData;
    
    public IUnitState currentState;

    private void Awake()
    {
        transform.TryGetComponent(out animator);
        transform.TryGetComponent(out unitData);
        ChangeState(new WaitState(this));
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentState.Update();
    }

    public void ChangeState(IUnitState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
        Debug.Log($"현재 상태: {currentState}");
    }
}
