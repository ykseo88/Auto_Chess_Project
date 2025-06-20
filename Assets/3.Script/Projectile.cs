using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject parentUnitObj;
    private UnitController unitController;
    private UnitData unitData;
    private Rigidbody rb;
    public float stuckDepth = 0.1f;
    
    // Start is called before the first frame update
    void Start()
    {
        transform.TryGetComponent(out rb);
        parentUnitObj.transform.TryGetComponent(out unitController);
        parentUnitObj.transform.TryGetComponent(out unitData);
    }

    private void OnCollisionEnter(Collision other)
    {
        rb.isKinematic = true;
        transform.SetParent(other.transform);
        other.transform.root.TryGetComponent(out UnitData targetUnitData);
        if (targetUnitData != null)
        {
            targetUnitData.HP -= unitData.Damage;
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
    }
}
