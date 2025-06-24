using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitWeaponController : MonoBehaviour
{
    public Animator animator;
    public UnitData unitData;
    public NavMeshAgent agent;
    
    public IWeaponState currentState;
    
    public UnitController unitController;
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        transform.TryGetComponent(out unitData);
        ChangeState(new WWaitState(this));
        transform.root.TryGetComponent(out unitController);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateState();
    }
    
    public void ChangeState(IWeaponState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
        Debug.Log($"현재 상태: {currentState}");
    }

    public void UpdateState()
    {
        switch (unitController.currentState)
        {
            case WaitState ws:
                ChangeState(new WWaitState(this));
                break;
            case DieState ws:
                ChangeState(new WDieState(this));
                break;
            case AttackState ws:
                ChangeState(new WAttackState(this));
                break;
            case RunState ws:
                ChangeState(new WRunState(this));
                break;
            case WinState ws:
                ChangeState(new WWinState(this));
                break;
        }
    }
}
