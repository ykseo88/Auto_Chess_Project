using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WAttackState : IWeaponState
{
    private static readonly int Attack = Animator.StringToHash("Attack");
    private UnitWeaponController weaponController;
    
    public WAttackState(UnitWeaponController weaponController) => this.weaponController = weaponController;
    private Animator animator;
    
    public void Enter()
    {
        animator = weaponController.animator;
        animator.SetTrigger(Attack);
    }

    public void Update()
    {
        
    }

    public void Exit()
    {
        
    }
}
