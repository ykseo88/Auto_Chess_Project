using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public GameObject parentUnitObj;
    public UnitController unitController;
    public UnitData unitData;
    private CapsuleCollider col;
    private Rigidbody rb;
    public LayerMask killLayer;

    public float BoomRange = 10f;
    
    [SerializeField] private ParticleSystem Boom;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        transform.TryGetComponent(out col);
        transform.TryGetComponent(out rb);
        PoolManager.Instance.PoolParticleSystemQueuePlus(Boom, 10);
    }
    
    private void OnCollisionEnter(Collision other)
    {
        PoolManager.Instance.ParticleSystemPoolActive(Boom, other.contacts[0].point, Quaternion.Euler(other.contacts[0].normal), 15f);
        Collider[] detectedColliders = Physics.OverlapSphere(transform.position, BoomRange, killLayer);

        foreach (Collider detectedCollider in detectedColliders)
        {
            Debug.Log(detectedCollider.gameObject.name);
            detectedCollider.transform.TryGetComponent(out UnitData targetUnitData);
            if (targetUnitData.Team.teamName != unitData.Team.teamName)
            {
                targetUnitData.HP -= unitData.Damage;
            }
        }
        gameObject.SetActive(false);
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
