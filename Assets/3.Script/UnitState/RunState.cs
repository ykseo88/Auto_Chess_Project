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
        if (unitController.unitData.isDead)
        {
            unitController.ChangeState(new DieState(unitController));
        }
        navMeshAgent.SetDestination(unitController.closeTarget.position);
        if (unitController.targetDistance <= unitController.unitData.AttackDistance)
        {
            unitController.ChangeState(new AttackState(unitController));
        }
        if (unitController.unitData.Team.EnemyTeam.UnitAmount == 0)
        {
            unitController.ChangeState(new WinState(unitController));
        }
    }

    public void Exit()
    {
        navMeshAgent.ResetPath();
    }
}
