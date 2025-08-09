using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : IUnitState
{
    private static readonly int ATTACK = Animator.StringToHash("Attack");
    private UnitController unitController;
    public AttackState(UnitController unitController) => this.unitController = unitController;
    private Animator animator;
    private AnimatorStateInfo stateInfo;
    private GameObject UnitObj;
    
    public void Enter()
    {
        animator = unitController.animator;
        animator.speed = unitController.unitData.AttackRate;
        animator.SetTrigger(ATTACK);
        UnitObj = unitController.gameObject;
    }

    public void Update()
    {
        if (unitController.unitData.isDead)
        {
            unitController.ChangeState(new DieState(unitController));
        }
        if (unitController.targetDistance > unitController.unitData.AttackDistance)
        {
            unitController.ChangeState(new RunState(unitController));
        }
        if (unitController.unitData.Team.EnemyTeam.PlayerUnitList.Count == 0)
        {
            unitController.ChangeState(new WinState(unitController));
        }
        
        UnitObj.transform.LookAt(unitController.closeTarget.transform);
    }

    public void Exit()
    {
        animator.speed = 1f;
    }
}
