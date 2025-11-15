using UnityEngine;

public class CameraJoystickControl : MonoBehaviour
{
    public Joystick joystick;
    public float rotationSpeed = 50f;

    private float verticalRotation = 0f;

    void Update()
    {
        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;

        // Horizontal rotation (Y axis)
        transform.Rotate(Vector3.up, horizontal * rotationSpeed * Time.deltaTime, Space.World);

        // Vertical rotation (X axis) with clamp
        verticalRotation -= vertical * rotationSpeed * Time.deltaTime;
        verticalRotation = Mathf.Clamp(verticalRotation, -80f, 80f);

        transform.localRotation = Quaternion.Euler(verticalRotation, transform.localEulerAngles.y, 0f);
    }
}
