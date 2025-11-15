using UnityEngine;
using UnityEngine.EventSystems;

public class ARInteractable : MonoBehaviour
{
    private Vector3 initialScale;
    private float rotationSpeed = 100f;
    private float scaleFactor = 0.2f;

    void Start()
    {
        initialScale = transform.localScale;
    }

    void Update()
    {
        if (Input.touchCount == 1 && !IsPointerOverUIObject())
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                float rotY = touch.deltaPosition.x * rotationSpeed * Time.deltaTime;
                transform.Rotate(0, -rotY, 0, Space.World);
            }
        }
        else if (Input.touchCount == 2)
        {
            Touch t1 = Input.GetTouch(0);
            Touch t2 = Input.GetTouch(1);

            Vector2 prevDist = (t1.position - t1.deltaPosition) - (t2.position - t2.deltaPosition);
            Vector2 curDist = t1.position - t2.position;

            float delta = curDist.magnitude - prevDist.magnitude;
            float scale = 1 + delta / 500f;
            transform.localScale = Vector3.ClampMagnitude(transform.localScale * scale, 5f);
        }
    }

    private bool IsPointerOverUIObject()
    {
        if (EventSystem.current == null) return false;
        return EventSystem.current.IsPointerOverGameObject(0);
    }
}
