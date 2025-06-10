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
    
    public void Enter()
    {
        animator = unitController.animator;
        animator.speed = unitController.unitData.AttackRate;
        animator.SetTrigger(ATTACK);
    }

    public void Update()
    {
        if (unitController.targetDistance > unitController.unitData.AttackDistance)
        {
            unitController.ChangeState(new RunState(unitController));
        }
    }

    public void Exit()
    {
        animator.speed = 1f;
    }
}
