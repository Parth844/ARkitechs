using UnityEngine;
using UnityEngine.EventSystems;

public class ARObjectInteraction : MonoBehaviour
{
    [Header("UI Panel")]
    public GameObject infoPanel;  // assign the Panel itself

    [Header("Interaction")]
    [Tooltip("Only these layers will be considered for tap raycasts.")]
    public LayerMask interactableLayers = ~0; // default: everything

    [Header("Scaling")]
    [Tooltip("How quickly the object scales with a pinch. Lower = slower.")]
    public float pinchScaleSpeed = 0.1f;
    [Tooltip("Minimum uniform scale when pinching.")]
    public float minScale = 0.01f;
    [Tooltip("Maximum uniform scale when pinching.")]
    public float maxScale = 2f;

    private Vector3 initialScale;
    private Vector3 initialRotation;

    private void Start()
    {
        initialScale = transform.localScale;
        initialRotation = transform.eulerAngles;

        HideInfoPanel(); // hide panel at start
    }

    private void Update()
    {
        // Touch input (device)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Ignore when touching over UI
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                return;

            if (touch.phase == TouchPhase.Began)
            {
                if (TryHandleTapAtScreenPosition(touch.position))
                {
                    ToggleInfoPanel();
                }
            }

            // Rotate object with finger swipe
            if (touch.phase == TouchPhase.Moved)
            {
                transform.Rotate(0, -touch.deltaPosition.x * 0.5f, 0, Space.World);
            }

            // Two-finger pinch to scale (smoothed and clamped)
            if (Input.touchCount == 2)
            {
                Touch t0 = Input.GetTouch(0);
                Touch t1 = Input.GetTouch(1);

                Vector2 prevDist = (t0.position - t0.deltaPosition) - (t1.position - t1.deltaPosition);
                Vector2 currDist = t0.position - t1.position;

                float rawDelta = currDist.magnitude / prevDist.magnitude;
                // Convert raw delta to a limited per-frame change around 1.0
                float maxStep = 1f + pinchScaleSpeed;   // e.g., 1.1 for +10%
                float minStep = 1f - pinchScaleSpeed;   // e.g., 0.9 for -10%
                float step = Mathf.Clamp(rawDelta, minStep, maxStep);

                // Apply uniform scale with clamps
                float currentUniform = transform.localScale.x;
                float newUniform = Mathf.Clamp(currentUniform * step, minScale, maxScale);
                transform.localScale = Vector3.one * newUniform;
            }
        }
        else
        {
            // Mouse input (Editor/Desktop testing)
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                    return;

                if (TryHandleTapAtScreenPosition(Input.mousePosition))
                {
                    ToggleInfoPanel();
                }
            }
        }
    }

    private bool TryHandleTapAtScreenPosition(Vector2 screenPosition)
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
            return false;

        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000f, interactableLayers))
        {
            // Accept taps on this transform or any of its children (common for imported models)
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                return true;
            }
        }
        return false;
    }

    // Toggle info panel visibility
    public void ToggleInfoPanel()
    {
        if (infoPanel != null)
            infoPanel.SetActive(!infoPanel.activeSelf);
    }

    // Explicit method to show the panel
    public void ShowInfoPanel()
    {
        if (infoPanel != null)
            infoPanel.SetActive(true);
    }

    // Explicit method to hide the panel
    public void HideInfoPanel()
    {
        if (infoPanel != null)
            infoPanel.SetActive(false);
    }

    // Optional: reset transform
    public void ResetTransform()
    {
        transform.localScale = initialScale;
        transform.eulerAngles = initialRotation;
        HideInfoPanel();
    }
}
