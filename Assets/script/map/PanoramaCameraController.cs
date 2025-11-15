using UnityEngine;
using UnityEngine.EventSystems;

public class PanoramaCameraController : MonoBehaviour
{
    public float speed = 0.15f;
    float yaw, pitch;

    void Update()
    {
        if (Input.touchCount == 1 && !IsPointerOverUI())
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Moved)
            {
                yaw += t.deltaPosition.x * speed;
                pitch -= t.deltaPosition.y * speed;
                pitch = Mathf.Clamp(pitch, -85f, 85f);
                transform.localRotation = Quaternion.Euler(pitch, yaw, 0);
            }
        }
    }

    bool IsPointerOverUI() =>
        EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
}
