using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RunState : IUnitState
{
    private static readonly int RUN = Animator.StringToHash("Run");
    private UnitController unitController;
    public RunState(UnitController unitController) => this.unitController = unitController;
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    
    
    public void Enter()
    {
        animator = unitController.animator;
        animator.SetTrigger(RUN);
        navMeshAgent = unitController.agent;
        navMeshAgent.speed = unitController.unitData.MoveSpeed;
    }

    public void Update()
    {
        navMeshAgent.SetDestination(unitController.closeTarget.position);
        if (unitController.targetDistance <= unitController.unitData.AttackDistance)
        {
            unitController.ChangeState(new AttackState(unitController));
        }
    }

    public void Exit()
    {
        navMeshAgent.ResetPath();
    }
}
