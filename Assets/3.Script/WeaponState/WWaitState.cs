using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WWaitState : IWeaponState
{
    private UnitWeaponController weaponController;
    
    public WWaitState(UnitWeaponController weaponController) => this.weaponController = weaponController;
    private Animator animator;
    
    public void Enter()
    {
        animator = weaponController.animator;
    }

    public void Update()
    {
        
    }

    public void Exit()
    {
        
    }
}
