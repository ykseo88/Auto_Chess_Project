using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaitState : IUnitState
{
    private UnitController unitController;
    public WaitState(UnitController unitController) => this.unitController = unitController;
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    
    public void Enter()
    {
        animator = unitController.animator;
        //animator.SetTrigger("Idle");
        unitController.transform.TryGetComponent(out navMeshAgent);
    }

    public void Update()
    {
        if (unitController.unitData.isDead)
        {
            unitController.ChangeState(new DieState(unitController));
        }
        if (GameManager.Instance.isFightStart)
        {
            unitController.ChangeState(new RunState(unitController));
            if (unitController.unitData.Team.EnemyTeam.UnitAmount == 0)
            {
                unitController.ChangeState(new WinState(unitController));
            }
        }
        
    }

    public void Exit()
    {
        
    }
}
