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
        if (weaponController.unitController.unitData.isDead)
        {
            weaponController.ChangeState(new WDieState(weaponController));
        }
        if (GameManager.Instance.isFightStart)
        {
            weaponController.ChangeState(new WRunState(weaponController));
            if (weaponController.unitController.unitData.Team.EnemyTeam.UnitAmount == 0)
            {
                weaponController.ChangeState(new WWinState(weaponController));
            }
        }
    }

    public void Exit()
    {
        
    }
}
