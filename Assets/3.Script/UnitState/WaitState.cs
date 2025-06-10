using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitState : IUnitState
{
    private UnitController unitController;
    public WaitState(UnitController unitController) => this.unitController = unitController;
    private Animator animator;
    
    public void Enter()
    {
        animator = unitController.animator;
        //animator.SetTrigger("Idle");
    }

    public void Update()
    {
        if (GameManager.Instance.isFightStart)
        {
            unitController.ChangeState(new RunState(unitController));
        }
    }

    public void Exit()
    {
        
    }
}
