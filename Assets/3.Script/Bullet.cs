using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject parentUnitObj;
    public UnitController unitController;
    public UnitData unitData;
    private CapsuleCollider col;
    private Rigidbody rb;
    private TrailRenderer trail;
    
    [SerializeField] private ParticleSystem Blood;
    [SerializeField] private ParticleSystem GroundImpact;
    
    // Start is called before the first frame update
    void Start()
    {
        transform.TryGetComponent(out col);
        transform.TryGetComponent(out rb);
        
        PoolManager.Instance.PoolParticleSystemQueuePlus(Blood, 10);
        PoolManager.Instance.PoolParticleSystemQueuePlus(GroundImpact, 10);

        trail = transform.GetComponentInChildren<TrailRenderer>();
    }

    //private void OnEnable()
    //{
    //    trail.enabled = true;
    //}
    //
    //private void OnDisable()
    //{
    //    trail.enabled = false;
    //}

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log(other.collider.name);
        other.transform.root.TryGetComponent(out UnitData targetUnitData);
        if (targetUnitData != null && other.gameObject.layer == LayerMask.NameToLayer("BodyParts"))
        {
            if (targetUnitData.Team.teamName != unitData.Team.teamName)
            {
                targetUnitData.HP -= unitData.Damage;
                PoolManager.Instance.ParticleSystemPoolActive(Blood, other.contacts[0].point, Quaternion.Euler(other.contacts[0].normal));
            }
        }
        else
        {
            PoolManager.Instance.ParticleSystemPoolActive(GroundImpact, other.contacts[0].point, Quaternion.Euler(other.contacts[0].normal));
        }

        
        //trail.enabled = false;
        gameObject.SetActive(false);
    }
    
    void Update()
    {
        
    }
}
