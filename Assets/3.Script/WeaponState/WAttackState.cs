using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WAttackState : IWeaponState
{
    private static readonly int Attack = Animator.StringToHash("Attack");
    private UnitWeaponController weaponController;
    
    public WAttackState(UnitWeaponController weaponController) => this.weaponController = weaponController;
    private Animator animator;
    private ParticleSystem GunShot;
    
    public void Enter()
    {
        animator = weaponController.animator;
        animator.SetTrigger(Attack);
        weaponController.transform.TryGetComponent(out AnimationEvents animationEvents);
        GunShot = animationEvents.Gunshot;
        
    }

    public void Update()
    {
        if (weaponController.unitController.unitData.isDead)
        {
            weaponController.ChangeState(new WDieState(weaponController));
        }
        if (weaponController.unitController.targetDistance > weaponController.unitController.unitData.AttackDistance)
        {
            weaponController.ChangeState(new WRunState(weaponController));
        }
        if (weaponController.unitController.unitData.Team.EnemyTeam.UnitAmount == 0)
        {
            weaponController.ChangeState(new WWinState(weaponController));
        }
    }

    public void Exit()
    {
        if (GunShot != null)
        {
            GunShot.Stop();
            GunShot.transform.position -= new Vector3(0, 3f, 0);
        }
    }
}
