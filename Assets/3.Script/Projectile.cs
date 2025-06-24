using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject parentUnitObj;
    public UnitController unitController;
    public UnitData unitData;
    private Rigidbody rb;
    private CapsuleCollider collider;
    private bool isStuck = false;
    public float stuckDepth = 0.1f;
    private Vector3 stuckPos;
    private Vector3 stuckDir;
    
    [SerializeField] private ParticleSystem Blood;
    [SerializeField] private ParticleSystem GroundImpact;
    
    // Start is called before the first frame update
    void Start()
    {
        transform.TryGetComponent(out rb);
        transform.TryGetComponent(out collider);
        parentUnitObj.transform.TryGetComponent(out unitController);
        parentUnitObj.transform.TryGetComponent(out unitData);
        PoolManager.Instance.PoolParticleSystemQueuePlus(Blood, 10);
        PoolManager.Instance.PoolParticleSystemQueuePlus(GroundImpact, 10);
    }

    private void OnCollisionEnter(Collision other)
    {
        isStuck = true;
        rb.isKinematic = true;
        collider.enabled = false;
        transform.SetParent(other.transform);
        stuckPos = transform.localPosition;
        stuckDir = transform.localRotation.eulerAngles;
        other.transform.root.TryGetComponent(out UnitData targetUnitData);
        if (targetUnitData != null && other.gameObject.layer == LayerMask.NameToLayer("BodyParts") && targetUnitData.Team.teamName != unitData.Team.teamName)
        {
            targetUnitData.HP -= unitData.Damage;
            PoolManager.Instance.ParticleSystemPoolActive(Blood, other.contacts[0].point, Quaternion.Euler(other.contacts[0].normal));
        }
        else
        {
            PoolManager.Instance.ParticleSystemPoolActive(GroundImpact, other.contacts[0].point, Quaternion.Euler(other.contacts[0].normal));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!rb.isKinematic)
        {
            Vector3 velocity = rb.velocity;
            transform.rotation = Quaternion.LookRotation(velocity, Vector3.up);
        }
        else
        {
            transform.localPosition = stuckPos;
            transform.localRotation = Quaternion.Euler(stuckDir);
        }
    }
}
