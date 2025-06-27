using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WRunState : IWeaponState
{
    private static readonly int Run = Animator.StringToHash("Run");
    private UnitWeaponController weaponController;
    
    public WRunState(UnitWeaponController weaponController) => this.weaponController = weaponController;
    private Animator animator;
    private ParticleSystem GunShot;
    
    public void Enter()
    {
        animator = weaponController.animator;
        animator.SetTrigger(Run);
        weaponController.transform.TryGetComponent(out AnimationEvents animationEvents);
        GunShot = animationEvents.Gunshot;
        GunShot.Pause();
        GunShot.transform.position -= new Vector3(0, 3f, 0);
    }

    public void Update()
    {
        if (weaponController.unitController.unitData.isDead)
        {
            weaponController.ChangeState(new WDieState(weaponController));
        }
        
        if (weaponController.unitController.targetDistance <= weaponController.unitController.unitData.AttackDistance)
        {
            weaponController.ChangeState(new WAttackState(weaponController));
        }
        if (weaponController.unitController.unitData.Team.EnemyTeam.UnitAmount == 0)
        {
            weaponController.ChangeState(new WWinState(weaponController));
        }
    }

    public void Exit()
    {
        
    }
}
