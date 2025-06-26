using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AnimationEvents : MonoBehaviour
{
    private Transform Unit;
    private UnitController unitController;
    private UnitData unitData;
    public GameObject projectilePrefab;
    public ParticleSystem MagicPrefab;
    public float projectileSpeed;
    public Transform shootPoint;

    public ParticleSystem Gunshot;
    public ParticleSystem BulletImpact;
    public Transform FirePoint;
    
    public Rigidbody projectileRigidbody;
    public Transform targetTransform;
    public float launchAngleDegrees = 30f; // 발사 각도 (도)
    public float initialForceMagnitude = 10f; // 초기 힘의 크기 (N)

    public GameObject ThrowWeapon;

    public int ShotGunShellNum = 10;
    public float Spread = 1f;

    private float AttackTerm = 10f;
    public LayerMask killLayer;
    public float MagicRange;

    private void OnGunshotEffect()
    {
        Gunshot.Play();
        Gunshot.transform.position = FirePoint.position;
        Gunshot.transform.rotation = FirePoint.rotation;
    }
    
    private void Start()
    {
        Unit = transform.root;
        Unit.TryGetComponent(out unitController);
        Unit.TryGetComponent(out unitData);

        if (projectilePrefab != null)
        {
            PoolManager.Instance.PoolGameObjectQueuePlus(projectilePrefab, 10);
        }

        if (MagicPrefab != null)
        {
            PoolManager.Instance.PoolParticleSystemQueuePlus(MagicPrefab, 10);
        }
    }

    private void GiveDamage()
    {
        unitController.closeTarget.TryGetComponent(out UnitData targetUnitData);
        targetUnitData.HP -= unitData.Damage;
    }

    private void GiveRangeDamage()
    {
        targetTransform = unitController.closeTarget.transform;
        GameObject projectile = PoolManager.Instance.GameObjectPoolActive(projectilePrefab, shootPoint.position, Quaternion.LookRotation(unitController.closeTarget.position));
        projectile.TryGetComponent(out Projectile projectileScript);
        projectile.TryGetComponent(out projectileRigidbody);
        projectileScript.parentUnitObj = Unit.gameObject;
        projectileScript.unitController =  unitController;
        projectileScript.unitData = unitData;
        
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

        //Debug.Log($"Calculated Initial Velocity: {v0} m/s");
        //Debug.Log($"Applied Impulse Force: {impulseForce}");
    }
    
    private void LaunchProjectileWithCalculatedAnglesHigh()
    {
        // 1. 투사체 생성 및 Rigidbody 참조
        targetTransform = unitController.closeTarget.transform;
        GameObject projectile = PoolManager.Instance.GameObjectPoolActive(projectilePrefab, shootPoint.position, Quaternion.LookRotation(unitController.closeTarget.position));
        projectile.TryGetComponent(out Projectile projectileScript);
        projectile.TryGetComponent(out projectileRigidbody);
        projectileScript.parentUnitObj = Unit.gameObject;
        projectileScript.unitController =  unitController;
        projectileScript.unitData = unitData;
        
        if (!projectile.TryGetComponent(out projectileRigidbody))
        {
            Debug.LogError("투사체 프리랩에 Rigidbody 컴포넌트가 없습니다.");
            Destroy(projectile); // Rigidbody 없으면 투사체 제거
            return;
        }

        // Rigidbody 설정 (필요에 따라 드래그 값 조절)
        projectileRigidbody.drag = 0f; // 정확한 포물선 계산을 위해 드래그를 0으로 설정 권장
        projectileRigidbody.angularDrag = 0.05f; // 회전 드래그는 유지해도 무방

        // 2. 시작/목표 위치 및 상대적 거리 계산
        Vector3 startPos = shootPoint.position;
        // 목표 지점의 Y값을 조절하여 원하는 높이로 설정할 수 있습니다.
        // 예: 타겟의 머리 위 (약간 더 높게)
        Vector3 targetPos = targetTransform.position + new Vector3(0, 1f, 0); 

        // 수평 거리 R 계산 (XZ 평면에서의 거리)
        float R = Vector3.Distance(new Vector3(startPos.x, 0, startPos.z), new Vector3(targetPos.x, 0, targetPos.z));
        // 수직 거리 H 계산
        float H = targetPos.y - startPos.y;

        // 3. 중력 가속도 (절대값)
        float g = Physics.gravity.magnitude;

        // 4. 초기 속도 크기 (v0) 계산
        // F = m * v0 (Impulse), 따라서 v0 = F / m
        float v0 = initialForceMagnitude / projectileRigidbody.mass;
        
        // 5. 2차 방정식 계수 계산 (A * tan^2(theta) + B * tan(theta) + C = 0)
        float A = (0.5f * g * R * R) / (v0 * v0);
        float B = -R;
        float C = H + A; // H + (g * R * R) / (2 * v0 * v0)

        // 6. 판별식 (D = B^2 - 4AC) 계산
        float discriminant = B * B - 4 * A * C;

        if (discriminant < 0)
        {
            Debug.LogError($"주어진 힘({initialForceMagnitude}N)으로는 목표 지점에 도달할 수 없습니다. 힘을 늘리거나, 목표 지점을 변경하세요. 판별식: {discriminant}");
            Destroy(projectile);
            return;
        }

        // 7. 두 가지 tan(theta) 값 계산
        float tanTheta1 = (-B + Mathf.Sqrt(discriminant)) / (2 * A);
        float tanTheta2 = (-B - Mathf.Sqrt(discriminant)) / (2 * A);

        // 8. 두 가지 발사 각도 (라디안) 계산
        float angleRadLow = Mathf.Atan(tanTheta1); // 낮은 각도
        float angleRadHigh = Mathf.Atan(tanTheta2); // 높은 각도

        // 9. 초기 속도 벡터 결정 및 힘 적용 (여기서는 낮은 각도를 적용)
        // 실제 게임에서는 상황에 따라 낮은 각도 또는 높은 각도를 선택하거나,
        // 둘 중 하나를 무작위로 선택할 수 있습니다.

        // 목표 지점 방향으로의 수평 단위 벡터
        Vector3 horizontalDirection = (new Vector3(targetPos.x, 0, targetPos.z) - new Vector3(startPos.x, 0, startPos.z)).normalized;

        // 선택된 각도로 초기 속도 벡터 생성
        // 여기서는 낮은 각도로 발사
        Vector3 initialVelocity = (horizontalDirection * v0 * Mathf.Cos(angleRadLow)) + (Vector3.up * v0 * Mathf.Sin(angleRadLow));
        
        // 임펄스 적용
        projectileRigidbody.AddForce(initialVelocity * projectileRigidbody.mass, ForceMode.Impulse);

        //Debug.Log($"Calculated Initial Force Magnitude: {initialForceMagnitude} N");
        //Debug.Log($"Calculated Initial Velocity Magnitude: {v0} m/s");
        //Debug.Log($"Possible Launch Angles (Degrees): Lower Angle = {angleRadLow * Mathf.Rad2Deg:F2}°, Higher Angle = {angleRadHigh * Mathf.Rad2Deg:F2}°");
        //Debug.Log($"Applied Impulse Force with Lower Angle: {initialVelocity * projectileRigidbody.mass}");

        // 만약 두 번째 각도로도 발사하고 싶다면, 새로운 투사체를 생성하고 angleRad2를 적용하면 됩니다.
        // 예를 들어, 디버깅 목적으로 두 번째 투사체를 쏴볼 수 있습니다.
        /*
        GameObject projectile2 = Instantiate(projectilePrefab, shootPoint.position + Vector3.right, Quaternion.identity); // 약간 옆으로 발사
        Rigidbody rb2;
        projectile2.TryGetComponent(out rb2);
        rb2.drag = 0f;
        
        Vector3 initialVelocity2 = (horizontalDirection * v0 * Mathf.Cos(angleRad2)) + (Vector3.up * v0 * Mathf.Sin(angleRad2));
        rb2.AddForce(initialVelocity2 * rb2.mass, ForceMode.Impulse);
        Debug.Log($"Applied Impulse Force with Higher Angle: {initialVelocity2 * rb2.mass}");
        */
    }
    
    private void LaunchProjectileWithCalculatedAnglesLow()
    {
        // 1. 투사체 생성 및 Rigidbody 참조
        targetTransform = unitController.closeTarget.transform;
        Vector3 ShootDir = targetTransform.position - shootPoint.position;
        GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.LookRotation(ShootDir.normalized));
        projectile.TryGetComponent(out Projectile projectileScript);
        projectile.TryGetComponent(out projectileRigidbody);
        projectileScript.parentUnitObj = Unit.gameObject;
        projectileScript.unitController =  unitController;
        projectileScript.unitData = unitData;
        
        if (!projectile.TryGetComponent(out projectileRigidbody))
        {
            Debug.LogError("투사체 프리랩에 Rigidbody 컴포넌트가 없습니다.");
            Destroy(projectile); // Rigidbody 없으면 투사체 제거
            return;
        }

        // Rigidbody 설정 (필요에 따라 드래그 값 조절)
        projectileRigidbody.drag = 0f; // 정확한 포물선 계산을 위해 드래그를 0으로 설정 권장
        projectileRigidbody.angularDrag = 0.05f; // 회전 드래그는 유지해도 무방

        // 2. 시작/목표 위치 및 상대적 거리 계산
        Vector3 startPos = shootPoint.position;
        // 목표 지점의 Y값을 조절하여 원하는 높이로 설정할 수 있습니다.
        // 예: 타겟의 머리 위 (약간 더 높게)
        Vector3 targetPos = targetTransform.position + new Vector3(0, 1f, 0); 

        // 수평 거리 R 계산 (XZ 평면에서의 거리)
        float R = Vector3.Distance(new Vector3(startPos.x, 0, startPos.z), new Vector3(targetPos.x, 0, targetPos.z));
        // 수직 거리 H 계산
        float H = targetPos.y - startPos.y;

        // 3. 중력 가속도 (절대값)
        float g = Physics.gravity.magnitude;

        // 4. 초기 속도 크기 (v0) 계산
        // F = m * v0 (Impulse), 따라서 v0 = F / m
        float v0 = initialForceMagnitude / projectileRigidbody.mass;
        
        // 5. 2차 방정식 계수 계산 (A * tan^2(theta) + B * tan(theta) + C = 0)
        float A = (0.5f * g * R * R) / (v0 * v0);
        float B = -R;
        float C = H + A; // H + (g * R * R) / (2 * v0 * v0)

        // 6. 판별식 (D = B^2 - 4AC) 계산
        float discriminant = B * B - 4 * A * C;

        if (discriminant < 0)
        {
            Debug.LogError($"주어진 힘({initialForceMagnitude}N)으로는 목표 지점에 도달할 수 없습니다. 힘을 늘리거나, 목표 지점을 변경하세요. 판별식: {discriminant}");
            Destroy(projectile);
            return;
        }

        // 7. 두 가지 tan(theta) 값 계산
        float tanTheta1 = (-B + Mathf.Sqrt(discriminant)) / (2 * A);
        float tanTheta2 = (-B - Mathf.Sqrt(discriminant)) / (2 * A);

        // 8. 두 가지 발사 각도 (라디안) 계산
        float angleRadLow = Mathf.Atan(tanTheta1); // 낮은 각도
        float angleRadHigh = Mathf.Atan(tanTheta2); // 높은 각도

        // 9. 초기 속도 벡터 결정 및 힘 적용 (여기서는 낮은 각도를 적용)
        // 실제 게임에서는 상황에 따라 낮은 각도 또는 높은 각도를 선택하거나,
        // 둘 중 하나를 무작위로 선택할 수 있습니다.

        // 목표 지점 방향으로의 수평 단위 벡터
        Vector3 horizontalDirection = (new Vector3(targetPos.x, 0, targetPos.z) - new Vector3(startPos.x, 0, startPos.z)).normalized;

        // 선택된 각도로 초기 속도 벡터 생성
        // 여기서는 낮은 각도로 발사
        Vector3 initialVelocity = (horizontalDirection * v0 * Mathf.Cos(angleRadHigh)) + (Vector3.up * v0 * Mathf.Sin(angleRadHigh));
        
        // 임펄스 적용
        projectileRigidbody.AddForce(initialVelocity * projectileRigidbody.mass, ForceMode.Impulse);

        //Debug.Log($"Calculated Initial Force Magnitude: {initialForceMagnitude} N");
        //Debug.Log($"Calculated Initial Velocity Magnitude: {v0} m/s");
        //Debug.Log($"Possible Launch Angles (Degrees): Lower Angle = {angleRadHigh * Mathf.Rad2Deg:F2}°, Higher Angle = {angleRadLow * Mathf.Rad2Deg:F2}°");
        //Debug.Log($"Applied Impulse Force with Lower Angle: {initialVelocity * projectileRigidbody.mass}");

        // 만약 두 번째 각도로도 발사하고 싶다면, 새로운 투사체를 생성하고 angleRad2를 적용하면 됩니다.
        // 예를 들어, 디버깅 목적으로 두 번째 투사체를 쏴볼 수 있습니다.
        /*
        GameObject projectile2 = Instantiate(projectilePrefab, shootPoint.position + Vector3.right, Quaternion.identity); // 약간 옆으로 발사
        Rigidbody rb2;
        projectile2.TryGetComponent(out rb2);
        rb2.drag = 0f;
        
        Vector3 initialVelocity2 = (horizontalDirection * v0 * Mathf.Cos(angleRad2)) + (Vector3.up * v0 * Mathf.Sin(angleRad2));
        rb2.AddForce(initialVelocity2 * rb2.mass, ForceMode.Impulse);
        Debug.Log($"Applied Impulse Force with Higher Angle: {initialVelocity2 * rb2.mass}");
        */
    }

    

    private void OffThrowWeapon()
    {
        ThrowWeapon.SetActive(false);
    }
    
    private void OnThrowWeapon()
    {
        ThrowWeapon.SetActive(true);
    }

    private void LaunchProjectileWithCalculatedAnglesLowGunShot()
    {
        Gunshot.transform.position = FirePoint.position;
        Gunshot.transform.rotation = FirePoint.rotation;
        Gunshot.Play();
        
        // 1. 투사체 생성 및 Rigidbody 참조
        targetTransform = unitController.closeTarget.transform;
        GameObject projectile = PoolManager.Instance.GameObjectPoolActive(projectilePrefab, shootPoint.position, Quaternion.LookRotation(unitController.closeTarget.position));
        projectile.TryGetComponent(out Bullet projectileScript);
        projectile.TryGetComponent(out projectileRigidbody);
        projectileScript.parentUnitObj = Unit.gameObject;
        projectileScript.unitController =  unitController;
        projectileScript.unitData = unitData;
        
        if (!projectile.TryGetComponent(out projectileRigidbody))
        {
            Debug.LogError("투사체 프리랩에 Rigidbody 컴포넌트가 없습니다.");
            Destroy(projectile); // Rigidbody 없으면 투사체 제거
            return;
        }

        // Rigidbody 설정 (필요에 따라 드래그 값 조절)
        projectileRigidbody.drag = 0f; // 정확한 포물선 계산을 위해 드래그를 0으로 설정 권장
        projectileRigidbody.angularDrag = 0.05f; // 회전 드래그는 유지해도 무방

        // 2. 시작/목표 위치 및 상대적 거리 계산
        Vector3 startPos = shootPoint.position;
        // 목표 지점의 Y값을 조절하여 원하는 높이로 설정할 수 있습니다.
        // 예: 타겟의 머리 위 (약간 더 높게)
        Vector3 targetPos = targetTransform.position + new Vector3(0, 1f, 0); 

        // 수평 거리 R 계산 (XZ 평면에서의 거리)
        float R = Vector3.Distance(new Vector3(startPos.x, 0, startPos.z), new Vector3(targetPos.x, 0, targetPos.z));
        // 수직 거리 H 계산
        float H = targetPos.y - startPos.y;

        // 3. 중력 가속도 (절대값)
        float g = Physics.gravity.magnitude;

        // 4. 초기 속도 크기 (v0) 계산
        // F = m * v0 (Impulse), 따라서 v0 = F / m
        float v0 = initialForceMagnitude / projectileRigidbody.mass;
        
        // 5. 2차 방정식 계수 계산 (A * tan^2(theta) + B * tan(theta) + C = 0)
        float A = (0.5f * g * R * R) / (v0 * v0);
        float B = -R;
        float C = H + A; // H + (g * R * R) / (2 * v0 * v0)

        // 6. 판별식 (D = B^2 - 4AC) 계산
        float discriminant = B * B - 4 * A * C;

        if (discriminant < 0)
        {
            Debug.LogError($"주어진 힘({initialForceMagnitude}N)으로는 목표 지점에 도달할 수 없습니다. 힘을 늘리거나, 목표 지점을 변경하세요. 판별식: {discriminant}");
            Destroy(projectile);
            return;
        }

        // 7. 두 가지 tan(theta) 값 계산
        float tanTheta1 = (-B + Mathf.Sqrt(discriminant)) / (2 * A);
        float tanTheta2 = (-B - Mathf.Sqrt(discriminant)) / (2 * A);

        // 8. 두 가지 발사 각도 (라디안) 계산
        float angleRadLow = Mathf.Atan(tanTheta1); // 낮은 각도
        float angleRadHigh = Mathf.Atan(tanTheta2); // 높은 각도

        // 9. 초기 속도 벡터 결정 및 힘 적용 (여기서는 낮은 각도를 적용)
        // 실제 게임에서는 상황에 따라 낮은 각도 또는 높은 각도를 선택하거나,
        // 둘 중 하나를 무작위로 선택할 수 있습니다.

        // 목표 지점 방향으로의 수평 단위 벡터
        Vector3 horizontalDirection = (new Vector3(targetPos.x, 0, targetPos.z) - new Vector3(startPos.x, 0, startPos.z)).normalized;

        // 선택된 각도로 초기 속도 벡터 생성
        // 여기서는 낮은 각도로 발사
        Vector3 initialVelocity = (horizontalDirection * v0 * Mathf.Cos(angleRadHigh)) + (Vector3.up * v0 * Mathf.Sin(angleRadHigh));
        
        // 임펄스 적용
        projectileRigidbody.AddForce(initialVelocity * projectileRigidbody.mass, ForceMode.Impulse);

        //Debug.Log($"Calculated Initial Force Magnitude: {initialForceMagnitude} N");
        //Debug.Log($"Calculated Initial Velocity Magnitude: {v0} m/s");
        //Debug.Log($"Possible Launch Angles (Degrees): Lower Angle = {angleRadHigh * Mathf.Rad2Deg:F2}°, Higher Angle = {angleRadLow * Mathf.Rad2Deg:F2}°");
        //Debug.Log($"Applied Impulse Force with Lower Angle: {initialVelocity * projectileRigidbody.mass}");

        // 만약 두 번째 각도로도 발사하고 싶다면, 새로운 투사체를 생성하고 angleRad2를 적용하면 됩니다.
        // 예를 들어, 디버깅 목적으로 두 번째 투사체를 쏴볼 수 있습니다.
        /*
        GameObject projectile2 = Instantiate(projectilePrefab, shootPoint.position + Vector3.right, Quaternion.identity); // 약간 옆으로 발사
        Rigidbody rb2;
        projectile2.TryGetComponent(out rb2);
        rb2.drag = 0f;
        
        Vector3 initialVelocity2 = (horizontalDirection * v0 * Mathf.Cos(angleRad2)) + (Vector3.up * v0 * Mathf.Sin(angleRad2));
        rb2.AddForce(initialVelocity2 * rb2.mass, ForceMode.Impulse);
        Debug.Log($"Applied Impulse Force with Higher Angle: {initialVelocity2 * rb2.mass}");
        */
    }

    private void LaunchProjectileWithCalculatedAnglesHighGunShot()
    {
        if (Gunshot == null)
        {
            Instantiate(Gunshot).transform.TryGetComponent(out ParticleSystem temp);
            Gunshot = temp;
        }

        
        Gunshot.transform.position = FirePoint.position;
        Gunshot.transform.rotation = FirePoint.rotation;
        
         // 1. 투사체 생성 및 Rigidbody 참조
        targetTransform = unitController.closeTarget.transform;
        GameObject projectile = PoolManager.Instance.GameObjectPoolActive(projectilePrefab, shootPoint.position, Quaternion.LookRotation(unitController.closeTarget.position));
        projectile.TryGetComponent(out Bullet projectileScript);
        projectile.TryGetComponent(out projectileRigidbody);
        projectileScript.parentUnitObj = Unit.gameObject;
        projectileScript.unitController =  unitController;
        projectileScript.unitData = unitData;
        
        if (!projectile.TryGetComponent(out projectileRigidbody))
        {
            Debug.LogError("투사체 프리랩에 Rigidbody 컴포넌트가 없습니다.");
            Destroy(projectile); // Rigidbody 없으면 투사체 제거
            return;
        }

        // Rigidbody 설정 (필요에 따라 드래그 값 조절)
        projectileRigidbody.drag = 0f; // 정확한 포물선 계산을 위해 드래그를 0으로 설정 권장
        projectileRigidbody.angularDrag = 0.05f; // 회전 드래그는 유지해도 무방

        // 2. 시작/목표 위치 및 상대적 거리 계산
        Vector3 startPos = shootPoint.position;
        // 목표 지점의 Y값을 조절하여 원하는 높이로 설정할 수 있습니다.
        // 예: 타겟의 머리 위 (약간 더 높게)
        Vector3 targetPos = targetTransform.position + new Vector3(0, 1f, 0); 

        // 수평 거리 R 계산 (XZ 평면에서의 거리)
        float R = Vector3.Distance(new Vector3(startPos.x, 0, startPos.z), new Vector3(targetPos.x, 0, targetPos.z));
        // 수직 거리 H 계산
        float H = targetPos.y - startPos.y;

        // 3. 중력 가속도 (절대값)
        float g = Physics.gravity.magnitude;

        // 4. 초기 속도 크기 (v0) 계산
        // F = m * v0 (Impulse), 따라서 v0 = F / m
        float v0 = initialForceMagnitude / projectileRigidbody.mass;
        
        // 5. 2차 방정식 계수 계산 (A * tan^2(theta) + B * tan(theta) + C = 0)
        float A = (0.5f * g * R * R) / (v0 * v0);
        float B = -R;
        float C = H + A; // H + (g * R * R) / (2 * v0 * v0)

        // 6. 판별식 (D = B^2 - 4AC) 계산
        float discriminant = B * B - 4 * A * C;

        if (discriminant < 0)
        {
            Debug.LogError($"주어진 힘({initialForceMagnitude}N)으로는 목표 지점에 도달할 수 없습니다. 힘을 늘리거나, 목표 지점을 변경하세요. 판별식: {discriminant}");
            Destroy(projectile);
            return;
        }

        // 7. 두 가지 tan(theta) 값 계산
        float tanTheta1 = (-B + Mathf.Sqrt(discriminant)) / (2 * A);
        float tanTheta2 = (-B - Mathf.Sqrt(discriminant)) / (2 * A);

        // 8. 두 가지 발사 각도 (라디안) 계산
        float angleRadLow = Mathf.Atan(tanTheta1); // 낮은 각도
        float angleRadHigh = Mathf.Atan(tanTheta2); // 높은 각도

        // 9. 초기 속도 벡터 결정 및 힘 적용 (여기서는 낮은 각도를 적용)
        // 실제 게임에서는 상황에 따라 낮은 각도 또는 높은 각도를 선택하거나,
        // 둘 중 하나를 무작위로 선택할 수 있습니다.

        // 목표 지점 방향으로의 수평 단위 벡터
        Vector3 horizontalDirection = (new Vector3(targetPos.x, 0, targetPos.z) - new Vector3(startPos.x, 0, startPos.z)).normalized;

        // 선택된 각도로 초기 속도 벡터 생성
        // 여기서는 낮은 각도로 발사
        Vector3 initialVelocity = (horizontalDirection * v0 * Mathf.Cos(angleRadLow)) + (Vector3.up * v0 * Mathf.Sin(angleRadLow));
        
        // 임펄스 적용
        projectileRigidbody.AddForce(initialVelocity * projectileRigidbody.mass, ForceMode.Impulse);

        //Debug.Log($"Calculated Initial Force Magnitude: {initialForceMagnitude} N");
        //Debug.Log($"Calculated Initial Velocity Magnitude: {v0} m/s");
        //Debug.Log($"Possible Launch Angles (Degrees): Lower Angle = {angleRadLow * Mathf.Rad2Deg:F2}°, Higher Angle = {angleRadHigh * Mathf.Rad2Deg:F2}°");
        //Debug.Log($"Applied Impulse Force with Lower Angle: {initialVelocity * projectileRigidbody.mass}");

        // 만약 두 번째 각도로도 발사하고 싶다면, 새로운 투사체를 생성하고 angleRad2를 적용하면 됩니다.
        // 예를 들어, 디버깅 목적으로 두 번째 투사체를 쏴볼 수 있습니다.
        /*
        GameObject projectile2 = Instantiate(projectilePrefab, shootPoint.position + Vector3.right, Quaternion.identity); // 약간 옆으로 발사
        Rigidbody rb2;
        projectile2.TryGetComponent(out rb2);
        rb2.drag = 0f;
        
        Vector3 initialVelocity2 = (horizontalDirection * v0 * Mathf.Cos(angleRad2)) + (Vector3.up * v0 * Mathf.Sin(angleRad2));
        rb2.AddForce(initialVelocity2 * rb2.mass, ForceMode.Impulse);
        Debug.Log($"Applied Impulse Force with Higher Angle: {initialVelocity2 * rb2.mass}");
        */
    }

    private void ChainGunShotHigh()
    {
        Gunshot.Play();
        Gunshot.transform.position = FirePoint.position;
        Gunshot.transform.rotation = FirePoint.rotation;
        
         // 1. 투사체 생성 및 Rigidbody 참조
        targetTransform = unitController.closeTarget.transform;
        GameObject projectile = PoolManager.Instance.GameObjectPoolActive(projectilePrefab, shootPoint.position, Quaternion.LookRotation(unitController.closeTarget.position));
        projectile.TryGetComponent(out Bullet projectileScript);
        projectile.TryGetComponent(out projectileRigidbody);
        projectileScript.parentUnitObj = Unit.gameObject;
        projectileScript.unitController =  unitController;
        projectileScript.unitData = unitData;
        
        if (!projectile.TryGetComponent(out projectileRigidbody))
        {
            Debug.LogError("투사체 프리랩에 Rigidbody 컴포넌트가 없습니다.");
            Destroy(projectile); // Rigidbody 없으면 투사체 제거
            return;
        }

        // Rigidbody 설정 (필요에 따라 드래그 값 조절)
        projectileRigidbody.drag = 0f; // 정확한 포물선 계산을 위해 드래그를 0으로 설정 권장
        projectileRigidbody.angularDrag = 0.05f; // 회전 드래그는 유지해도 무방

        // 2. 시작/목표 위치 및 상대적 거리 계산
        Vector3 startPos = shootPoint.position;
        // 목표 지점의 Y값을 조절하여 원하는 높이로 설정할 수 있습니다.
        // 예: 타겟의 머리 위 (약간 더 높게)
        
        float randomX = Random.Range(-Spread, Spread);
        float randomY = Random.Range(-Spread, Spread);
        float randomZ = Random.Range(-Spread, Spread);
        Vector3 targetPos = targetTransform.position + new Vector3(randomX, 1f, randomZ); 

        // 수평 거리 R 계산 (XZ 평면에서의 거리)
        float R = Vector3.Distance(new Vector3(startPos.x, 0, startPos.z), new Vector3(targetPos.x, 0, targetPos.z));
        // 수직 거리 H 계산
        float H = targetPos.y - startPos.y;
        
        

        // 3. 중력 가속도 (절대값)
        float g = Physics.gravity.magnitude;

        // 4. 초기 속도 크기 (v0) 계산
        // F = m * v0 (Impulse), 따라서 v0 = F / m
        float v0 = initialForceMagnitude / projectileRigidbody.mass;
        
        // 5. 2차 방정식 계수 계산 (A * tan^2(theta) + B * tan(theta) + C = 0)
        float A = (0.5f * g * R * R) / (v0 * v0);
        float B = -R;
        float C = H + A; // H + (g * R * R) / (2 * v0 * v0)

        // 6. 판별식 (D = B^2 - 4AC) 계산
        float discriminant = B * B - 4 * A * C;

        if (discriminant < 0)
        {
            Debug.LogError($"주어진 힘({initialForceMagnitude}N)으로는 목표 지점에 도달할 수 없습니다. 힘을 늘리거나, 목표 지점을 변경하세요. 판별식: {discriminant}");
            Destroy(projectile);
            return;
        }

        // 7. 두 가지 tan(theta) 값 계산
        float tanTheta1 = (-B + Mathf.Sqrt(discriminant)) / (2 * A);
        float tanTheta2 = (-B - Mathf.Sqrt(discriminant)) / (2 * A);

        // 8. 두 가지 발사 각도 (라디안) 계산
        float angleRadLow = Mathf.Atan(tanTheta1); // 낮은 각도
        float angleRadHigh = Mathf.Atan(tanTheta2); // 높은 각도

        // 9. 초기 속도 벡터 결정 및 힘 적용 (여기서는 낮은 각도를 적용)
        // 실제 게임에서는 상황에 따라 낮은 각도 또는 높은 각도를 선택하거나,
        // 둘 중 하나를 무작위로 선택할 수 있습니다.

        // 목표 지점 방향으로의 수평 단위 벡터
        Vector3 horizontalDirection = (new Vector3(targetPos.x, 0, targetPos.z) - new Vector3(startPos.x, 0, startPos.z)).normalized;

        // 선택된 각도로 초기 속도 벡터 생성
        // 여기서는 낮은 각도로 발사
        Vector3 initialVelocity = (horizontalDirection * v0 * Mathf.Cos(angleRadLow)) + (Vector3.up * v0 * Mathf.Sin(angleRadLow));
        
        // 임펄스 적용
        projectileRigidbody.AddForce((initialVelocity + new Vector3(randomX, randomY, randomZ)) * projectileRigidbody.mass, ForceMode.Impulse);

        //Debug.Log($"Calculated Initial Force Magnitude: {initialForceMagnitude} N");
        //Debug.Log($"Calculated Initial Velocity Magnitude: {v0} m/s");
        //Debug.Log($"Possible Launch Angles (Degrees): Lower Angle = {angleRadLow * Mathf.Rad2Deg:F2}°, Higher Angle = {angleRadHigh * Mathf.Rad2Deg:F2}°");
        //Debug.Log($"Applied Impulse Force with Lower Angle: {initialVelocity * projectileRigidbody.mass}");

        // 만약 두 번째 각도로도 발사하고 싶다면, 새로운 투사체를 생성하고 angleRad2를 적용하면 됩니다.
        // 예를 들어, 디버깅 목적으로 두 번째 투사체를 쏴볼 수 있습니다.
        /*
        GameObject projectile2 = Instantiate(projectilePrefab, shootPoint.position + Vector3.right, Quaternion.identity); // 약간 옆으로 발사
        Rigidbody rb2;
        projectile2.TryGetComponent(out rb2);
        rb2.drag = 0f;
        
        Vector3 initialVelocity2 = (horizontalDirection * v0 * Mathf.Cos(angleRad2)) + (Vector3.up * v0 * Mathf.Sin(angleRad2));
        rb2.AddForce(initialVelocity2 * rb2.mass, ForceMode.Impulse);
        Debug.Log($"Applied Impulse Force with Higher Angle: {initialVelocity2 * rb2.mass}");
        */
    }
    
    private void ChainGunShotLow()
    { 
        
         // 1. 투사체 생성 및 Rigidbody 참조
        targetTransform = unitController.closeTarget.transform;
        GameObject projectile = PoolManager.Instance.GameObjectPoolActive(projectilePrefab, shootPoint.position, Quaternion.LookRotation(unitController.closeTarget.position));
        projectile.TryGetComponent(out Bullet projectileScript);
        projectile.TryGetComponent(out projectileRigidbody);
        projectileScript.parentUnitObj = Unit.gameObject;
        projectileScript.unitController =  unitController;
        projectileScript.unitData = unitData;
        
        if (!projectile.TryGetComponent(out projectileRigidbody))
        {
            Debug.LogError("투사체 프리랩에 Rigidbody 컴포넌트가 없습니다.");
            Destroy(projectile); // Rigidbody 없으면 투사체 제거
            return;
        }

        // Rigidbody 설정 (필요에 따라 드래그 값 조절)
        projectileRigidbody.drag = 0f; // 정확한 포물선 계산을 위해 드래그를 0으로 설정 권장
        projectileRigidbody.angularDrag = 0.05f; // 회전 드래그는 유지해도 무방

        // 2. 시작/목표 위치 및 상대적 거리 계산
        Vector3 startPos = shootPoint.position;
        // 목표 지점의 Y값을 조절하여 원하는 높이로 설정할 수 있습니다.
        // 예: 타겟의 머리 위 (약간 더 높게)
        
        float randomX = Random.Range(-Spread, Spread);
        float randomY = Random.Range(-Spread, Spread);
        float randomZ = Random.Range(-Spread, Spread);
        Vector3 targetPos = targetTransform.position + new Vector3(0, 1f, 0); 

        // 수평 거리 R 계산 (XZ 평면에서의 거리)
        float R = Vector3.Distance(new Vector3(startPos.x, 0, startPos.z), new Vector3(targetPos.x, 0, targetPos.z));
        // 수직 거리 H 계산
        float H = targetPos.y - startPos.y;
        
        

        // 3. 중력 가속도 (절대값)
        float g = Physics.gravity.magnitude;

        // 4. 초기 속도 크기 (v0) 계산
        // F = m * v0 (Impulse), 따라서 v0 = F / m
        float v0 = initialForceMagnitude / projectileRigidbody.mass;
        
        // 5. 2차 방정식 계수 계산 (A * tan^2(theta) + B * tan(theta) + C = 0)
        float A = (0.5f * g * R * R) / (v0 * v0);
        float B = -R;
        float C = H + A; // H + (g * R * R) / (2 * v0 * v0)

        // 6. 판별식 (D = B^2 - 4AC) 계산
        float discriminant = B * B - 4 * A * C;

        if (discriminant < 0)
        {
            Debug.LogError($"주어진 힘({initialForceMagnitude}N)으로는 목표 지점에 도달할 수 없습니다. 힘을 늘리거나, 목표 지점을 변경하세요. 판별식: {discriminant}");
            Destroy(projectile);
            return;
        }

        // 7. 두 가지 tan(theta) 값 계산
        float tanTheta1 = (-B + Mathf.Sqrt(discriminant)) / (2 * A);
        float tanTheta2 = (-B - Mathf.Sqrt(discriminant)) / (2 * A);

        // 8. 두 가지 발사 각도 (라디안) 계산
        float angleRadHigh = Mathf.Atan(tanTheta1); // 낮은 각도
        float angleRadLow = Mathf.Atan(tanTheta2); // 높은 각도

        // 9. 초기 속도 벡터 결정 및 힘 적용 (여기서는 낮은 각도를 적용)
        // 실제 게임에서는 상황에 따라 낮은 각도 또는 높은 각도를 선택하거나,
        // 둘 중 하나를 무작위로 선택할 수 있습니다.

        // 목표 지점 방향으로의 수평 단위 벡터
        Vector3 horizontalDirection = (new Vector3(targetPos.x, 0, targetPos.z) - new Vector3(startPos.x, 0, startPos.z)).normalized;

        // 선택된 각도로 초기 속도 벡터 생성
        // 여기서는 낮은 각도로 발사
        Vector3 initialVelocity = (horizontalDirection * v0 * Mathf.Cos(angleRadLow)) + (Vector3.up * v0 * Mathf.Sin(angleRadLow));
        
        // 임펄스 적용
        projectileRigidbody.AddForce((initialVelocity + new Vector3(randomX, randomY, randomZ)) * projectileRigidbody.mass, ForceMode.Impulse);

        //Debug.Log($"Calculated Initial Force Magnitude: {initialForceMagnitude} N");
        //Debug.Log($"Calculated Initial Velocity Magnitude: {v0} m/s");
        //Debug.Log($"Possible Launch Angles (Degrees): Lower Angle = {angleRadLow * Mathf.Rad2Deg:F2}°, Higher Angle = {angleRadHigh * Mathf.Rad2Deg:F2}°");
        //Debug.Log($"Applied Impulse Force with Lower Angle: {initialVelocity * projectileRigidbody.mass}");

        // 만약 두 번째 각도로도 발사하고 싶다면, 새로운 투사체를 생성하고 angleRad2를 적용하면 됩니다.
        // 예를 들어, 디버깅 목적으로 두 번째 투사체를 쏴볼 수 있습니다.
        /*
        GameObject projectile2 = Instantiate(projectilePrefab, shootPoint.position + Vector3.right, Quaternion.identity); // 약간 옆으로 발사
        Rigidbody rb2;
        projectile2.TryGetComponent(out rb2);
        rb2.drag = 0f;
        
        Vector3 initialVelocity2 = (horizontalDirection * v0 * Mathf.Cos(angleRad2)) + (Vector3.up * v0 * Mathf.Sin(angleRad2));
        rb2.AddForce(initialVelocity2 * rb2.mass, ForceMode.Impulse);
        Debug.Log($"Applied Impulse Force with Higher Angle: {initialVelocity2 * rb2.mass}");
        */
    }
    
    private void ShotGunShotLow()
    { 
        Gunshot.transform.position = FirePoint.position;
        Gunshot.transform.rotation = FirePoint.rotation;
        Gunshot.Play();
        
         // 1. 투사체 생성 및 Rigidbody 참조
         for (int i = 0; i < ShotGunShellNum; i++)
         {
             targetTransform = unitController.closeTarget.transform;
             GameObject projectile = PoolManager.Instance.GameObjectPoolActive(projectilePrefab, shootPoint.position, Quaternion.LookRotation(unitController.closeTarget.position));
             projectile.TryGetComponent(out Bullet projectileScript);
             projectile.TryGetComponent(out projectileRigidbody);
            projectileScript.parentUnitObj = Unit.gameObject;
            projectileScript.unitController =  unitController;
            projectileScript.unitData = unitData;
        
            if (!projectile.TryGetComponent(out projectileRigidbody))
            {
                Debug.LogError("투사체 프리랩에 Rigidbody 컴포넌트가 없습니다.");
                Destroy(projectile); // Rigidbody 없으면 투사체 제거
                return;
            }

            // Rigidbody 설정 (필요에 따라 드래그 값 조절)
            projectileRigidbody.drag = 0f; // 정확한 포물선 계산을 위해 드래그를 0으로 설정 권장
            projectileRigidbody.angularDrag = 0.05f; // 회전 드래그는 유지해도 무방

            // 2. 시작/목표 위치 및 상대적 거리 계산
            Vector3 startPos = shootPoint.position;
            // 목표 지점의 Y값을 조절하여 원하는 높이로 설정할 수 있습니다.
            // 예: 타겟의 머리 위 (약간 더 높게)
            
            float randomX = Random.Range(-Spread, Spread);
            float randomY = Random.Range(-Spread, Spread);
            float randomZ = Random.Range(-Spread, Spread);
            Vector3 targetPos = targetTransform.position + new Vector3(0, 1f, 0); 
            
            // 수평 거리 R 계산 (XZ 평면에서의 거리)
            float R = Vector3.Distance(new Vector3(startPos.x, 0, startPos.z), new Vector3(targetPos.x, 0, targetPos.z));
            // 수직 거리 H 계산
            float H = targetPos.y - startPos.y;
            
            
            
            // 3. 중력 가속도 (절대값)
            float g = Physics.gravity.magnitude;
            
            // 4. 초기 속도 크기 (v0) 계산
            // F = m * v0 (Impulse), 따라서 v0 = F / m
            float v0 = initialForceMagnitude / projectileRigidbody.mass;
            
            // 5. 2차 방정식 계수 계산 (A * tan^2(theta) + B * tan(theta) + C = 0)
            float A = (0.5f * g * R * R) / (v0 * v0);
            float B = -R;
            float C = H + A; // H + (g * R * R) / (2 * v0 * v0)
            
            // 6. 판별식 (D = B^2 - 4AC) 계산
            float discriminant = B * B - 4 * A * C;
            
            if (discriminant < 0)
            {
                Debug.LogError($"주어진 힘({initialForceMagnitude}N)으로는 목표 지점에 도달할 수 없습니다. 힘을 늘리거나, 목표 지점을 변경하세요. 판별식: {discriminant}");
                Destroy(projectile);
                return;
            }
            
            // 7. 두 가지 tan(theta) 값 계산
            float tanTheta1 = (-B + Mathf.Sqrt(discriminant)) / (2 * A);
            float tanTheta2 = (-B - Mathf.Sqrt(discriminant)) / (2 * A);
            
            // 8. 두 가지 발사 각도 (라디안) 계산
            float angleRadHigh = Mathf.Atan(tanTheta1); // 낮은 각도
            float angleRadLow = Mathf.Atan(tanTheta2); // 높은 각도
            
            // 9. 초기 속도 벡터 결정 및 힘 적용 (여기서는 낮은 각도를 적용)
            // 실제 게임에서는 상황에 따라 낮은 각도 또는 높은 각도를 선택하거나,
            // 둘 중 하나를 무작위로 선택할 수 있습니다.
            
            // 목표 지점 방향으로의 수평 단위 벡터
            Vector3 horizontalDirection = (new Vector3(targetPos.x, 0, targetPos.z) - new Vector3(startPos.x, 0, startPos.z)).normalized;
            
            // 선택된 각도로 초기 속도 벡터 생성
            // 여기서는 낮은 각도로 발사
            Vector3 initialVelocity = (horizontalDirection * v0 * Mathf.Cos(angleRadLow)) + (Vector3.up * v0 * Mathf.Sin(angleRadLow));
            
            // 임펄스 적용
            projectileRigidbody.AddForce((initialVelocity + new Vector3(randomX, randomY, randomZ)) * projectileRigidbody.mass, ForceMode.Impulse);
            
            //Debug.Log($"Calculated Initial Force Magnitude: {initialForceMagnitude} N");
            //Debug.Log($"Calculated Initial Velocity Magnitude: {v0} m/s");
            //Debug.Log($"Possible Launch Angles (Degrees): Lower Angle = {angleRadLow * Mathf.Rad2Deg:F2}°, Higher Angle = {angleRadHigh * Mathf.Rad2Deg:F2}°");
            //Debug.Log($"Applied Impulse Force with Lower Angle: {initialVelocity * projectileRigidbody.mass}");
            
            // 만약 두 번째 각도로도 발사하고 싶다면, 새로운 투사체를 생성하고 angleRad2를 적용하면 됩니다.
            // 예를 들어, 디버깅 목적으로 두 번째 투사체를 쏴볼 수 있습니다.
            /*
            GameObject projectile2 = Instantiate(projectilePrefab, shootPoint.position + Vector3.right, Quaternion.identity); // 약간 옆으로 발사
            Rigidbody rb2;
            projectile2.TryGetComponent(out rb2);
            rb2.drag = 0f;
            
            Vector3 initialVelocity2 = (horizontalDirection * v0 * Mathf.Cos(angleRad2)) + (Vector3.up * v0 * Mathf.Sin(angleRad2));
            rb2.AddForce(initialVelocity2 * rb2.mass, ForceMode.Impulse);
            Debug.Log($"Applied Impulse Force with Higher Angle: {initialVelocity2 * rb2.mass}");
            */
         }
    }
    
    private void BombThrowHigh()
    {
        // 1. 투사체 생성 및 Rigidbody 참조
        targetTransform = unitController.closeTarget.transform;
        GameObject projectile = PoolManager.Instance.GameObjectPoolActive(projectilePrefab, shootPoint.position, Quaternion.LookRotation(unitController.closeTarget.position));
        projectile.TryGetComponent(out Bomb projectileScript);
        projectile.TryGetComponent(out projectileRigidbody);
        projectileScript.parentUnitObj = Unit.gameObject;
        projectileScript.unitController =  unitController;
        projectileScript.unitData = unitData;
        
        if (!projectile.TryGetComponent(out projectileRigidbody))
        {
            Debug.LogError("투사체 프리랩에 Rigidbody 컴포넌트가 없습니다.");
            Destroy(projectile); // Rigidbody 없으면 투사체 제거
            return;
        }

        // Rigidbody 설정 (필요에 따라 드래그 값 조절)
        projectileRigidbody.drag = 0f; // 정확한 포물선 계산을 위해 드래그를 0으로 설정 권장
        projectileRigidbody.angularDrag = 0.05f; // 회전 드래그는 유지해도 무방

        // 2. 시작/목표 위치 및 상대적 거리 계산
        Vector3 startPos = shootPoint.position;
        // 목표 지점의 Y값을 조절하여 원하는 높이로 설정할 수 있습니다.
        // 예: 타겟의 머리 위 (약간 더 높게)
        Vector3 targetPos = targetTransform.position + new Vector3(0, 1f, 0); 

        // 수평 거리 R 계산 (XZ 평면에서의 거리)
        float R = Vector3.Distance(new Vector3(startPos.x, 0, startPos.z), new Vector3(targetPos.x, 0, targetPos.z));
        // 수직 거리 H 계산
        float H = targetPos.y - startPos.y;

        // 3. 중력 가속도 (절대값)
        float g = Physics.gravity.magnitude;

        // 4. 초기 속도 크기 (v0) 계산
        // F = m * v0 (Impulse), 따라서 v0 = F / m
        float v0 = initialForceMagnitude / projectileRigidbody.mass;
        
        // 5. 2차 방정식 계수 계산 (A * tan^2(theta) + B * tan(theta) + C = 0)
        float A = (0.5f * g * R * R) / (v0 * v0);
        float B = -R;
        float C = H + A; // H + (g * R * R) / (2 * v0 * v0)

        // 6. 판별식 (D = B^2 - 4AC) 계산
        float discriminant = B * B - 4 * A * C;

        if (discriminant < 0)
        {
            Debug.LogError($"주어진 힘({initialForceMagnitude}N)으로는 목표 지점에 도달할 수 없습니다. 힘을 늘리거나, 목표 지점을 변경하세요. 판별식: {discriminant}");
            Destroy(projectile);
            return;
        }

        // 7. 두 가지 tan(theta) 값 계산
        float tanTheta1 = (-B + Mathf.Sqrt(discriminant)) / (2 * A);
        float tanTheta2 = (-B - Mathf.Sqrt(discriminant)) / (2 * A);

        // 8. 두 가지 발사 각도 (라디안) 계산
        float angleRadLow = Mathf.Atan(tanTheta1); // 낮은 각도
        float angleRadHigh = Mathf.Atan(tanTheta2); // 높은 각도

        // 9. 초기 속도 벡터 결정 및 힘 적용 (여기서는 낮은 각도를 적용)
        // 실제 게임에서는 상황에 따라 낮은 각도 또는 높은 각도를 선택하거나,
        // 둘 중 하나를 무작위로 선택할 수 있습니다.

        // 목표 지점 방향으로의 수평 단위 벡터
        Vector3 horizontalDirection = (new Vector3(targetPos.x, 0, targetPos.z) - new Vector3(startPos.x, 0, startPos.z)).normalized;

        // 선택된 각도로 초기 속도 벡터 생성
        // 여기서는 낮은 각도로 발사
        Vector3 initialVelocity = (horizontalDirection * v0 * Mathf.Cos(angleRadLow)) + (Vector3.up * v0 * Mathf.Sin(angleRadLow));
        
        // 임펄스 적용
        projectileRigidbody.AddForce(initialVelocity * projectileRigidbody.mass, ForceMode.Impulse);

        //Debug.Log($"Calculated Initial Force Magnitude: {initialForceMagnitude} N");
        //Debug.Log($"Calculated Initial Velocity Magnitude: {v0} m/s");
        //Debug.Log($"Possible Launch Angles (Degrees): Lower Angle = {angleRadLow * Mathf.Rad2Deg:F2}°, Higher Angle = {angleRadHigh * Mathf.Rad2Deg:F2}°");
        //Debug.Log($"Applied Impulse Force with Lower Angle: {initialVelocity * projectileRigidbody.mass}");

        // 만약 두 번째 각도로도 발사하고 싶다면, 새로운 투사체를 생성하고 angleRad2를 적용하면 됩니다.
        // 예를 들어, 디버깅 목적으로 두 번째 투사체를 쏴볼 수 있습니다.
        /*
        GameObject projectile2 = Instantiate(projectilePrefab, shootPoint.position + Vector3.right, Quaternion.identity); // 약간 옆으로 발사
        Rigidbody rb2;
        projectile2.TryGetComponent(out rb2);
        rb2.drag = 0f;
        
        Vector3 initialVelocity2 = (horizontalDirection * v0 * Mathf.Cos(angleRad2)) + (Vector3.up * v0 * Mathf.Sin(angleRad2));
        rb2.AddForce(initialVelocity2 * rb2.mass, ForceMode.Impulse);
        Debug.Log($"Applied Impulse Force with Higher Angle: {initialVelocity2 * rb2.mass}");
        */
    }

    private void MagicBoom()
    {
        targetTransform = unitController.closeTarget.transform;
        ParticleSystem magic = PoolManager.Instance.ParticleSystemPoolActive(MagicPrefab, targetTransform.position, Quaternion.identity, 5f);
        Collider[] detectedColliders = Physics.OverlapSphere(transform.position, MagicRange, killLayer);

        foreach (Collider detectedCollider in detectedColliders)
        {
            Debug.Log(detectedCollider.gameObject.name);
            detectedCollider.transform.TryGetComponent(out UnitData targetUnitData);
            if (targetUnitData.Team.teamName != unitData.Team.teamName)
            {
                targetUnitData.HP -= unitData.Damage;
            }
        }
    }
}
