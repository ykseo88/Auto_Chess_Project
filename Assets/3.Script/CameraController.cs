using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float MoveSpeed = 5f;
    
    [Range(10, 50)]
    public float Height = 40f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInput();
    }

    private void UpdateInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Height <= 10f)
            {
                Height = 50f;
            }
            else
            {
                Height -= 10f;
            }
        }
        
        Vector3 moveDirection = new Vector3(moveX, 0, moveZ);
        
        transform.position += moveDirection * (Time.deltaTime * MoveSpeed);
        transform.position = new Vector3(transform.position.x, Height, transform.position.z);
        
    }
}
