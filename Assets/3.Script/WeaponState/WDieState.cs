using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WDieState : IWeaponState
{
    private static readonly int Die = Animator.StringToHash("Die");
    private UnitWeaponController weaponController;
    
    public WDieState(UnitWeaponController weaponController) => this.weaponController = weaponController;
    private Animator animator;
    
    public void Enter()
    {
        animator = weaponController.animator;
        animator.SetTrigger(Die);
    }

    public void Update()
    {
        
    }

    public void Exit()
    {
        
    }
}
