using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WWinState : IWeaponState
{
    private static readonly int Win = Animator.StringToHash("Win");
    private UnitWeaponController weaponController;
    
    public WWinState(UnitWeaponController weaponController) => this.weaponController = weaponController;
    private Animator animator;
    
    public void Enter()
    {
        animator = weaponController.animator;
        animator.SetTrigger(Win);
    }

    public void Update()
    {
        
    }

    public void Exit()
    {
        
    }
}
