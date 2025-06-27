using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WDieState : IWeaponState
{
    private static readonly int Die = Animator.StringToHash("Die");
    private UnitWeaponController weaponController;
    
    public WDieState(UnitWeaponController weaponController) => this.weaponController = weaponController;
    private Animator animator;
    private ParticleSystem GunShot;
    
    public void Enter()
    {
        animator = weaponController.animator;
        animator.SetTrigger(Die);
        weaponController.transform.TryGetComponent(out AnimationEvents animationEvents);
        GunShot = animationEvents.Gunshot;
        GunShot.Stop();
        GunShot.transform.position -= new Vector3(0, 3f, 0);
    }

    public void Update()
    {
        
    }

    public void Exit()
    {
        
    }
}
