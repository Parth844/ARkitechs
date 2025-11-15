using UnityEngine;
using UnityEngine.InputSystem; // New Input System
using UnityEngine.UI;

public class ModelInteraction : MonoBehaviour
{
    private bool interactionEnabled = true; // Set to true for testing

    [Header("Rotation Settings")]
    public float rotationSpeed = 50f;

    [Header("Zoom Settings")]
    public float zoomSpeed = 1f;   // Lower = slower scaling via scroll
    public float minZoom = 2f;
    public float maxZoom = 4f;

    [Header("Pan Settings")]
    public float panSpeed = 0.01f;

    [Header("Virtual Joystick Settings")]
    public float joystickSensitivity = 1f;
    public float joystickDeadZone = 0.1f;

    [Header("UI Joystick References")]
    public Joystick rotationJoystick;    // For rotation control
    public Joystick panJoystick;        // For pan control
    public Slider zoomSlider;           // For zoom control
    public Button zoomInButton;         // Alternative zoom in
    public Button zoomOutButton;        // Alternative zoom out

    private Camera mainCamera;
    private Vector3 lastMousePosition;
    private Vector2 rotationInput;
    private Vector2 panInput;
    private float zoomInput;

    void Start()
    {
        mainCamera = Camera.main;
        
        // Verify all connections
        VerifyConnections();
        
        // Setup zoom slider
        if (zoomSlider != null)
        {
            zoomSlider.minValue = minZoom;
            zoomSlider.maxValue = maxZoom;
            zoomSlider.value = transform.localScale.x;
            zoomSlider.onValueChanged.AddListener(OnZoomSliderChanged);
        }
        
        // Setup zoom buttons
        if (zoomInButton != null)
            zoomInButton.onClick.AddListener(ZoomIn);
        if (zoomOutButton != null)
            zoomOutButton.onClick.AddListener(ZoomOut);
    }

    void Update()
    {
        if (!interactionEnabled) return;

        // Handle mouse interactions
        HandleMouseRotation();
        HandleMouseZoom();
        HandleMousePan();

        // Handle joystick interactions
        HandleJoystickRotation();
        HandleJoystickPan();
    }

    private void HandleMouseRotation()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            float rotX = delta.x * rotationSpeed * Time.deltaTime;
            float rotY = delta.y * rotationSpeed * Time.deltaTime;

            // Rotate model
            transform.Rotate(Vector3.up, -rotX, Space.World);
            transform.Rotate(Vector3.right, rotY, Space.World);
        }
    }

    private void HandleMouseZoom()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;
        if (Mathf.Abs(scroll) > 0.01f)
        {
            // Normalize scroll to small per-frame multiplier
            float direction = Mathf.Sign(scroll);
            float step = 1f + (direction * zoomSpeed * Time.deltaTime);
            float current = transform.localScale.x;
            float next = Mathf.Clamp(current * step, minZoom, maxZoom);
            transform.localScale = Vector3.one * next;
        }
    }

    private void HandleMousePan()
    {
        if (Mouse.current.rightButton.isPressed)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            Vector3 move = new Vector3(-delta.x, -delta.y, 0) * panSpeed;
            transform.Translate(move, Space.World);
        }
    }

    // Joystick input handlers
    private void HandleJoystickRotation()
    {
        if (rotationJoystick != null)
        {
            Vector2 joystickInput = rotationJoystick.Direction;
            
            // Apply dead zone
            if (joystickInput.magnitude > joystickDeadZone)
            {
                float rotX = joystickInput.x * rotationSpeed * joystickSensitivity * Time.deltaTime;
                float rotY = joystickInput.y * rotationSpeed * joystickSensitivity * Time.deltaTime;

                // Rotate model
                transform.Rotate(Vector3.up, -rotX, Space.World);
                transform.Rotate(Vector3.right, rotY, Space.World);
                
                // Debug joystick input (only log occasionally to avoid spam)
                if (Time.frameCount % 60 == 0) // Log every 60 frames (about once per second)
                {
                    Debug.Log("Rotation Joystick: " + joystickInput + " → Rotating by (" + rotX + ", " + rotY + ")");
                }
            }
        }
    }

    private void HandleJoystickPan()
    {
        if (panJoystick != null)
        {
            Vector2 joystickInput = panJoystick.Direction;
            
            // Apply dead zone
            if (joystickInput.magnitude > joystickDeadZone)
            {
                Vector3 move = new Vector3(-joystickInput.x, -joystickInput.y, 0) * panSpeed * joystickSensitivity;
                transform.Translate(move, Space.World);
                
                // Debug joystick input (only log occasionally to avoid spam)
                if (Time.frameCount % 60 == 0) // Log every 60 frames (about once per second)
                {
                    Debug.Log("Pan Joystick: " + joystickInput + " → Moving by " + move);
                }
            }
        }
    }

    // Zoom control methods
    public void OnZoomSliderChanged(float value)
    {
        if (interactionEnabled)
        {
            transform.localScale = Vector3.one * value;
        }
    }

    public void ZoomIn()
    {
        Debug.Log("🔵 ZOOM IN BUTTON CLICKED!");
        Debug.Log("  - Interaction Enabled: " + interactionEnabled);
        Debug.Log("  - Button Object: " + (zoomInButton != null ? zoomInButton.name : "NULL"));
        Debug.Log("  - Model Transform: " + (transform != null ? "Valid" : "NULL"));
        
        if (interactionEnabled)
        {
            float current = transform.localScale.x;
            float next = Mathf.Clamp(current + 0.1f, minZoom, maxZoom);
            transform.localScale = Vector3.one * next;
            
            // Update slider if it exists
            if (zoomSlider != null)
                zoomSlider.value = next;
            
            Debug.Log("✅ Zoomed in from " + current + " to " + next);
        }
        else
        {
            Debug.Log("❌ Interaction is disabled - button click ignored");
        }
    }

    public void ZoomOut()
    {
        Debug.Log("🔴 ZOOM OUT BUTTON CLICKED!");
        Debug.Log("  - Interaction Enabled: " + interactionEnabled);
        Debug.Log("  - Button Object: " + (zoomOutButton != null ? zoomOutButton.name : "NULL"));
        Debug.Log("  - Model Transform: " + (transform != null ? "Valid" : "NULL"));
        
        if (interactionEnabled)
        {
            float current = transform.localScale.x;
            float next = Mathf.Clamp(current - 0.1f, minZoom, maxZoom);
            transform.localScale = Vector3.one * next;
            
            // Update slider if it exists
            if (zoomSlider != null)
                zoomSlider.value = next;
            
            Debug.Log("✅ Zoomed out from " + current + " to " + next);
        }
        else
        {
            Debug.Log("❌ Interaction is disabled - button click ignored");
        }
    }

    /// <summary>
    /// Verify all control connections and log status
    /// </summary>
    private void VerifyConnections()
    {
        Debug.Log("=== MODEL INTERACTION VERIFICATION ===");
        Debug.Log("Model GameObject: " + gameObject.name);
        Debug.Log("Model Transform: " + (transform != null ? "✅ Connected" : "❌ Missing"));
        Debug.Log("Interaction Enabled: " + (interactionEnabled ? "✅ Yes" : "❌ No"));
        
        Debug.Log("\n--- JOYSTICK CONNECTIONS ---");
        Debug.Log("Rotation Joystick: " + (rotationJoystick != null ? "✅ Connected" : "❌ Missing"));
        if (rotationJoystick != null)
        {
            Debug.Log("  - Joystick Name: " + rotationJoystick.name);
            Debug.Log("  - Joystick Active: " + rotationJoystick.gameObject.activeInHierarchy);
        }
        
        Debug.Log("Pan Joystick: " + (panJoystick != null ? "✅ Connected" : "❌ Missing"));
        if (panJoystick != null)
        {
            Debug.Log("  - Joystick Name: " + panJoystick.name);
            Debug.Log("  - Joystick Active: " + panJoystick.gameObject.activeInHierarchy);
        }
        
        Debug.Log("\n--- ZOOM CONTROLS ---");
        Debug.Log("Zoom Slider: " + (zoomSlider != null ? "✅ Connected" : "❌ Missing"));
        if (zoomSlider != null)
        {
            Debug.Log("  - Slider Name: " + zoomSlider.name);
            Debug.Log("  - Slider Active: " + zoomSlider.gameObject.activeInHierarchy);
            Debug.Log("  - Min Value: " + zoomSlider.minValue);
            Debug.Log("  - Max Value: " + zoomSlider.maxValue);
        }
        
        Debug.Log("Zoom In Button: " + (zoomInButton != null ? "✅ Connected" : "❌ Missing"));
        if (zoomInButton != null)
        {
            Debug.Log("  - Button Name: " + zoomInButton.name);
            Debug.Log("  - Button Active: " + zoomInButton.gameObject.activeInHierarchy);
            Debug.Log("  - Button Interactable: " + zoomInButton.interactable);
        }
        
        Debug.Log("Zoom Out Button: " + (zoomOutButton != null ? "✅ Connected" : "❌ Missing"));
        if (zoomOutButton != null)
        {
            Debug.Log("  - Button Name: " + zoomOutButton.name);
            Debug.Log("  - Button Active: " + zoomOutButton.gameObject.activeInHierarchy);
            Debug.Log("  - Button Interactable: " + zoomOutButton.interactable);
        }
        
        Debug.Log("\n--- CAMERA CONNECTION ---");
        Debug.Log("Main Camera: " + (mainCamera != null ? "✅ Connected" : "❌ Missing"));
        if (mainCamera != null)
        {
            Debug.Log("  - Camera Name: " + mainCamera.name);
        }
        
        Debug.Log("=== VERIFICATION COMPLETE ===\n");
    }

    /// <summary>
    /// Test joystick input and log values
    /// </summary>
    public void TestJoystickInput()
    {
        if (rotationJoystick != null)
        {
            Vector2 rotInput = rotationJoystick.Direction;
            if (rotInput.magnitude > 0.01f)
            {
                Debug.Log("Rotation Joystick Input: " + rotInput + " (Magnitude: " + rotInput.magnitude + ")");
            }
        }
        
        if (panJoystick != null)
        {
            Vector2 panInput = panJoystick.Direction;
            if (panInput.magnitude > 0.01f)
            {
                Debug.Log("Pan Joystick Input: " + panInput + " (Magnitude: " + panInput.magnitude + ")");
            }
        }
    }

    /// <summary>
    /// Test model transformation
    /// </summary>
    public void TestModelTransform()
    {
        Debug.Log("=== MODEL TRANSFORM TEST ===");
        Debug.Log("Current Position: " + transform.position);
        Debug.Log("Current Rotation: " + transform.rotation.eulerAngles);
        Debug.Log("Current Scale: " + transform.localScale);
        
        // Test rotation
        transform.Rotate(Vector3.up, 10f, Space.World);
        Debug.Log("After 10° Y rotation: " + transform.rotation.eulerAngles);
        
        // Test scale
        Vector3 originalScale = transform.localScale;
        transform.localScale = Vector3.one * 2f;
        Debug.Log("After scale to 2: " + transform.localScale);
        transform.localScale = originalScale; // Reset
        Debug.Log("Reset to original scale: " + transform.localScale);
        
        Debug.Log("=== TRANSFORM TEST COMPLETE ===\n");
    }

    /// <summary>
    /// Test button functionality manually
    /// </summary>
    public void TestButtonFunctionality()
    {
        Debug.Log("=== BUTTON FUNCTIONALITY TEST ===");
        
        // Test if methods can be called directly
        Debug.Log("Testing ZoomIn method directly...");
        ZoomIn();
        
        Debug.Log("Testing ZoomOut method directly...");
        ZoomOut();
        
        Debug.Log("=== BUTTON TEST COMPLETE ===\n");
    }

    /// <summary>
    /// Check button event connections
    /// </summary>
    public void CheckButtonEvents()
    {
        Debug.Log("=== BUTTON EVENT CONNECTION CHECK ===");
        
        if (zoomInButton != null)
        {
            Debug.Log("Zoom In Button Details:");
            Debug.Log("  - Name: " + zoomInButton.name);
            Debug.Log("  - Active: " + zoomInButton.gameObject.activeInHierarchy);
            Debug.Log("  - Interactable: " + zoomInButton.interactable);
            Debug.Log("  - Event Count: " + zoomInButton.onClick.GetPersistentEventCount());
            
            // Check if our method is connected
            for (int i = 0; i < zoomInButton.onClick.GetPersistentEventCount(); i++)
            {
                string methodName = zoomInButton.onClick.GetPersistentMethodName(i);
                Object target = zoomInButton.onClick.GetPersistentTarget(i);
                Debug.Log("  - Event " + i + ": " + methodName + " on " + (target != null ? target.name : "NULL"));
            }
        }
        else
        {
            Debug.Log("❌ Zoom In Button is NULL");
        }
        
        if (zoomOutButton != null)
        {
            Debug.Log("Zoom Out Button Details:");
            Debug.Log("  - Name: " + zoomOutButton.name);
            Debug.Log("  - Active: " + zoomOutButton.gameObject.activeInHierarchy);
            Debug.Log("  - Interactable: " + zoomOutButton.interactable);
            Debug.Log("  - Event Count: " + zoomOutButton.onClick.GetPersistentEventCount());
            
            // Check if our method is connected
            for (int i = 0; i < zoomOutButton.onClick.GetPersistentEventCount(); i++)
            {
                string methodName = zoomOutButton.onClick.GetPersistentMethodName(i);
                Object target = zoomOutButton.onClick.GetPersistentTarget(i);
                Debug.Log("  - Event " + i + ": " + methodName + " on " + (target != null ? target.name : "NULL"));
            }
        }
        else
        {
            Debug.Log("❌ Zoom Out Button is NULL");
        }
        
        Debug.Log("=== BUTTON EVENT CHECK COMPLETE ===\n");
    }

    /// <summary>
    /// Toggle interaction from PlayModeToggle
    /// </summary>
    public void EnableInteraction(bool isOn)
    {
        interactionEnabled = isOn;
        Debug.Log("[ModelInteraction] Interaction " + (isOn ? "Enabled ✅" : "Disabled ❌"));
    }
}
