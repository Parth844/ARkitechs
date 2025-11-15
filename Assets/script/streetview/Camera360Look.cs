using UnityEngine;

public class Camera360Look : MonoBehaviour
{
    public float sensitivity = 2f;

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = -Input.GetAxis("Mouse Y") * sensitivity;

        transform.Rotate(mouseY, mouseX, 0, Space.Self);

        // Lock Z rotation
        Vector3 euler = transform.eulerAngles;
        euler.z = 0;
        transform.eulerAngles = euler;
    }
}
