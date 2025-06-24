using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WRunState : IWeaponState
{
    private static readonly int Run = Animator.StringToHash("Run");
    private UnitWeaponController weaponController;
    
    public WRunState(UnitWeaponController weaponController) => this.weaponController = weaponController;
    private Animator animator;
    
    public void Enter()
    {
        animator = weaponController.animator;
        animator.SetTrigger(Run);
    }

    public void Update()
    {
        
    }

    public void Exit()
    {
        
    }
}
