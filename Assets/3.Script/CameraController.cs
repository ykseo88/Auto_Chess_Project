using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("카메라 이동 속도 (미터/초)")]
    public float movementSpeed = 5.0f; 
    [Tooltip("Shift 키를 눌렀을 때의 이동 속도 배율")]
    public float sprintMultiplier = 3.0f;

    [Header("Rotation Settings")]
    [Tooltip("카메라 회전 속도")]
    public float rotationSpeed = 2.0f; 

    private float currentMovementSpeed;
    private float yaw = 0.0f; // Y축 회전 (좌우)
    private float pitch = 0.0f; // X축 회전 (상하)
    
    private bool cursorLocked = true; // 커서 잠금 상태

    void Start()
    {
        // 현재 카메라의 회전 값으로 초기화 (시작 시 튀는 현상 방지)
        Vector3 rot = transform.eulerAngles;
        yaw = rot.y;
        pitch = rot.x;
    }

    void Update()
    {
        if (GameManager.Instance.isFightStart)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            cursorLocked = true;
            HandleMovement();
            HandleRotation();
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            cursorLocked = false;
        }

        if (GameManager.Instance.isAssignTurn)
        {
            UnitAssignControll();
        }
        
        
    }

    private void UnitAssignControll()
    {
        
        float horizontalInput = Input.GetAxisRaw("Horizontal"); // A/D 또는 좌/우 화살표
        float verticalInput = Input.GetAxisRaw("Vertical");   // W/S 또는 상/하 화살표
        
        Vector3 moveDirection = Vector3.zero;
        
        moveDirection += transform.up * verticalInput;
        moveDirection += transform.right * horizontalInput;
        
        transform.position += moveDirection.normalized * (currentMovementSpeed * Time.deltaTime);
        Debug.Log($"배치모드 카메라 실행중: {transform.position}, 움직임: {moveDirection}");
    }

    void HandleMovement()
    {
        // Shift 키를 누르면 빠르게 이동
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentMovementSpeed = movementSpeed * sprintMultiplier;
        }
        else
        {
            currentMovementSpeed = movementSpeed;
        }

        // 키 입력에 따른 이동 벡터 계산
        float horizontalInput = Input.GetAxisRaw("Horizontal"); // A/D 또는 좌/우 화살표
        float verticalInput = Input.GetAxisRaw("Vertical");   // W/S 또는 상/하 화살표
        
        Vector3 moveDirection = Vector3.zero;

        // 전후좌우 이동
        moveDirection += transform.forward * verticalInput;
        moveDirection += transform.right * horizontalInput;

        // 위아래 이동 (Q/E 또는 Space/Ctrl)
        if (Input.GetKey(KeyCode.Q)) // 아래로 이동
        {
            moveDirection += Vector3.down;
        }
        if (Input.GetKey(KeyCode.E)) // 위로 이동
        {
            moveDirection += Vector3.up;
        }
        // 또는 Space/Ctrl 조합:
        // if (Input.GetKey(KeyCode.Space)) // 위로 이동
        // {
        //     moveDirection += Vector3.up;
        // }
        // if (Input.GetKey(KeyCode.LeftControl)) // 아래로 이동
        // {
        //     moveDirection += Vector3.down;
        // }

        // 이동 벡터 정규화 (대각선 이동 시 속도 증가 방지) 및 속도 적용
        transform.position += moveDirection.normalized * (currentMovementSpeed * Time.deltaTime);
    }

    void HandleRotation()
    {
        // 마우스 입력에 따른 회전 값 계산
        yaw += Input.GetAxis("Mouse X") * rotationSpeed;
        pitch -= Input.GetAxis("Mouse Y") * rotationSpeed; // Y축 마우스 이동은 상하 회전 (Pitch)

        // 상하 회전(Pitch) 값 제한 (너무 많이 위/아래로 보지 못하게)
        pitch = Mathf.Clamp(pitch, -90f, 90f);

        // 계산된 회전 값을 카메라에 적용
        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }
}