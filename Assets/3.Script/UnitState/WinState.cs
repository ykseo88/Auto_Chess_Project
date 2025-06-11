using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WinState : IUnitState
{
    private static readonly int WIN = Animator.StringToHash("Win");
    private UnitController unitController;
    public WinState(UnitController unitController) => this.unitController = unitController;
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    
    
    public void Enter()
    {
        animator = unitController.animator;
        animator.SetTrigger(WIN);
        navMeshAgent = unitController.agent;
        navMeshAgent.enabled = false;
    }

    public void Update()
    {
        
    }

    public void Exit()
    {

    }
}
