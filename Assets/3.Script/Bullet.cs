using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject parentUnitObj;
    private UnitController unitController;
    private UnitData unitData;
    private CapsuleCollider col;
    private Rigidbody rb;
    
    [SerializeField] private ParticleSystem Blood;
    [SerializeField] private ParticleSystem GroundImpact;
    
    // Start is called before the first frame update
    void Start()
    {
        transform.TryGetComponent(out col);
        transform.TryGetComponent(out rb);
        parentUnitObj.transform.TryGetComponent(out unitController);
        parentUnitObj.transform.TryGetComponent(out unitData);
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log(other.collider.name);
        other.transform.root.TryGetComponent(out UnitData targetUnitData);
        if (targetUnitData != null && targetUnitData.Team.teamName != unitData.Team.teamName)
        {
            targetUnitData.HP -= unitData.Damage;
            Instantiate(Blood, other.contacts[0].point, Quaternion.Euler(other.contacts[0].normal));
        }
        Destroy(gameObject);
    }
    
    void Update()
    {
        
    }
}
