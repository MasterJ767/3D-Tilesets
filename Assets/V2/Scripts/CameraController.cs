using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public float movementSpeed = 120f;

    private float XRotation = 0;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.fixedDeltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.fixedDeltaTime;
        
        XRotation -= mouseY;
        XRotation = Mathf.Clamp(XRotation, -50f, 90f);

        Vector3 YRotation = Vector3.up * mouseX;
            
        transform.localRotation = Quaternion.Euler(XRotation, 0f, 0f);
        transform.parent.Rotate(YRotation);

        Vector3 relativeMovement = (Input.GetAxisRaw("Vertical") * transform.forward) + (Input.GetAxisRaw("Horizontal") * transform.right);
        Vector3 upMovement = Vector3.zero;
        if (Input.GetButton("Jump")) { upMovement += Vector3.up; }
        else if (Input.GetButton("Descend")) { upMovement -= Vector3.up; }

        Vector3 movement = (relativeMovement + upMovement) * movementSpeed * Time.deltaTime;
        transform.parent.position = Vector3.MoveTowards(transform.parent.position, transform.parent.position + movement, 2.0f * movementSpeed * Time.deltaTime);
    }
}
