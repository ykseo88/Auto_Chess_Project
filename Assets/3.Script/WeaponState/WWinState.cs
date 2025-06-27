using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WWinState : IWeaponState
{
    private static readonly int Win = Animator.StringToHash("Win");
    private UnitWeaponController weaponController;
    
    public WWinState(UnitWeaponController weaponController) => this.weaponController = weaponController;
    private Animator animator;
    private ParticleSystem GunShot;
    
    public void Enter()
    {
        animator = weaponController.animator;
        animator.SetTrigger(Win);
        weaponController.transform.TryGetComponent(out AnimationEvents animationEvents);
        GunShot = animationEvents.Gunshot;
        GunShot.Pause();
        GunShot.transform.position -= new Vector3(0, 3f, 0);
    }

    public void Update()
    {
        
    }

    public void Exit()
    {
        
    }
}
