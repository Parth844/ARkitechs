using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class ARPlanePlacement : MonoBehaviour
{
    [Header("AR Components")]
    public ARPlaneManager planeManager;
    public ARRaycastManager raycastManager;
    
    [Header("Model Settings")]
    public GameObject modelPrefab; // Assign your 3D model prefab here
    public LocalModelManager localModelManager; // Reference to your model manager
    
    [Header("UI")]
    public UnityEngine.UI.Button placeModelButton;
    public UnityEngine.UI.Text statusText;
    
    private GameObject placedModel;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    
    void Start()
    {
        // Try to find AR components if not assigned
        if (planeManager == null)
        {
            planeManager = FindObjectOfType<ARPlaneManager>();
            if (planeManager == null)
                Debug.LogError("[ARPlanePlacement] ARPlaneManager not found! Make sure ARPlaneManager is in the scene.");
        }
        
        if (raycastManager == null)
        {
            raycastManager = FindObjectOfType<ARRaycastManager>();
            if (raycastManager == null)
                Debug.LogError("[ARPlanePlacement] ARRaycastManager not found! Make sure ARRaycastManager is in the scene.");
        }
        
        // Setup button
        if (placeModelButton != null)
            placeModelButton.onClick.AddListener(PlaceModelOnPlane);
        else
            Debug.LogWarning("[ARPlanePlacement] Place Model Button not assigned!");
            
        // Update status
        UpdateStatus("Point camera at a flat surface to detect planes");
    }
    
    void Update()
    {
        // Handle touch input for placing models
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                PlaceModelAtTouch(touch.position);
            }
        }
        
        // Mouse input for editor testing
        if (Input.GetMouseButtonDown(0))
        {
            PlaceModelAtTouch(Input.mousePosition);
        }
    }
    
    void PlaceModelAtTouch(Vector2 screenPosition)
    {
        // Check if raycast manager is available
        if (raycastManager == null)
        {
            Debug.LogError("[ARPlanePlacement] ARRaycastManager is null! Cannot perform raycast.");
            return;
        }
        
        // Raycast against detected planes
        if (raycastManager.Raycast(screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;
            Debug.Log($"[ARPlanePlacement] Raycast hit plane at {hitPose.position}");
            PlaceModelAtPosition(hitPose.position, hitPose.rotation);
        }
    }
    
    public void PlaceModelOnPlane()
    {
        // Check if plane manager is available
        if (planeManager == null)
        {
            UpdateStatus("ARPlaneManager not found! Check AR setup.");
            return;
        }
        
        // Place model at center of first detected plane
        if (planeManager.trackables.count > 0)
        {
            // TrackableCollection does not expose an indexer; iterate to get the first plane
            foreach (var plane in planeManager.trackables)
            {
                PlaceModelAtPosition(plane.transform.position, plane.transform.rotation);
                break;
            }
        }
        else
        {
            UpdateStatus("No planes detected. Point camera at a flat surface.");
        }
    }
    
    void PlaceModelAtPosition(Vector3 position, Quaternion rotation)
    {
        // Remove existing model
        if (placedModel != null)
            Destroy(placedModel);
            
        // Create new model
        if (modelPrefab != null)
        {
            placedModel = Instantiate(modelPrefab, position, rotation);
            UpdateStatus("Model placed on plane!");
        }
        else if (localModelManager != null)
        {
            // Move the already-loaded model (user should load via UI first)
            localModelManager.PlaceModelAtPosition(position, rotation);
            UpdateStatus("Placed loaded model on plane (if loaded). Load first if not visible.");
        }
        else
        {
            UpdateStatus("No model prefab assigned!");
        }
    }
    
    void UpdateStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
        else
            Debug.LogWarning("[ARPlanePlacement] Status Text UI not assigned!");
        Debug.Log("[ARPlanePlacement] " + message);
    }
    
    // Called when planes are detected
    public void OnPlanesChanged(ARPlanesChangedEventArgs eventArgs)
    {
        if (eventArgs.added.Count > 0)
        {
            UpdateStatus("Plane detected! Tap to place model or use button.");
        }
    }
}