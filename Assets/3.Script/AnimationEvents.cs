using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    private Transform Unit;
    private UnitController unitController;
    private UnitData unitData;
    public GameObject projectilePrefab;
    public float projectileSpeed;
    public Transform shootPoint;
    
    public Rigidbody projectileRigidbody;
    public Transform targetTransform;
    public float launchAngleDegrees = 30f; // 발사 각도 (도)

    private void Start()
    {
        Unit = transform.parent;
        Unit.TryGetComponent(out unitController);
        Unit.TryGetComponent(out unitData);
    }

    private void GiveDamage()
    {
        unitController.closeTarget.TryGetComponent(out UnitData targetUnitData);
        targetUnitData.HP -= unitData.Damage;
    }

    private void GiveRangeDamage()
    {
        targetTransform = unitController.closeTarget.transform;
        GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.LookRotation(unitController.closeTarget.position));
        projectile.TryGetComponent(out Projectile projectileScript);
        projectile.TryGetComponent(out projectileRigidbody);
        projectileScript.parentUnitObj = Unit.gameObject;
        
        Vector3 startPos = projectileRigidbody.position;
        Vector3 targetPos = targetTransform.position + new Vector3(0, 1f, 0);

        // 1. 상대적인 위치 계산
        float R = Vector3.Distance(new Vector3(startPos.x, 0, startPos.z), new Vector3(targetPos.x, 0, targetPos.z));
        float H = targetPos.y - startPos.y;

        // 2. 중력 가속도
        float g = Physics.gravity.magnitude; // Unity의 중력 가속도 (절대값)

        // 3. 발사 각도를 라디안으로 변환
        float launchAngleRad = launchAngleDegrees * Mathf.Deg2Rad;

        // 4. 필요한 초기 속도 (v0) 계산
        // v0 = sqrt(g * R^2 / (2 * cos^2(theta) * (R * tan(theta) - H)))
        float term1 = 0.5f * g * R * R;
        float term2 = Mathf.Cos(launchAngleRad) * Mathf.Cos(launchAngleRad) * (R * Mathf.Tan(launchAngleRad) - H);

        if (term2 <= 0)
        {
            Debug.LogError("주어진 각도로는 목표 지점에 도달할 수 없습니다. 각도를 조절하거나, 목표 지점을 변경하세요.");
            return;
        }

        float v0 = Mathf.Sqrt(term1 / term2);

        // 5. 초기 속도 벡터 계산
        Vector3 horizontalDirection = new Vector3(targetPos.x - startPos.x, 0, targetPos.z - startPos.z).normalized;
        Vector3 initialVelocity = (horizontalDirection * v0 * Mathf.Cos(launchAngleRad)) + (Vector3.up * v0 * Mathf.Sin(launchAngleRad));

        // 6. 임펄스 계산 및 적용
        Vector3 impulseForce = initialVelocity * projectileRigidbody.mass;
        projectileRigidbody.AddForce(impulseForce, ForceMode.Impulse);

        Debug.Log($"Calculated Initial Velocity: {v0} m/s");
        Debug.Log($"Applied Impulse Force: {impulseForce}");
        
    }
}
