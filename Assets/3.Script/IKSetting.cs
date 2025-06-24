using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKSetting : MonoBehaviour
{
    public Transform LH;
    public Transform RH;
    
    private Animator animator;
    
    // Start is called before the first frame update
    void Start()
    {
        transform.TryGetComponent(out animator);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnAnimatorIK(int layerIndex)
    {
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);
            
        animator.SetIKPosition(AvatarIKGoal.LeftHand, LH.position);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, LH.rotation);
        
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
            
        animator.SetIKPosition(AvatarIKGoal.RightHand, RH.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, RH.rotation);
    }
}
