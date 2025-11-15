using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlaneVisualizer : MonoBehaviour
{
    [Header("Visual Settings")]
    public Material planeMaterial;
    public Color planeColor = new Color(0, 1, 0, 0.3f); // Semi-transparent green
    
    private ARPlaneManager planeManager;
    
    void Start()
    {
        // Try to get ARPlaneManager from this GameObject first
        planeManager = GetComponent<ARPlaneManager>();
        
        // If not found, try to find it in the scene
        if (planeManager == null)
        {
            planeManager = FindObjectOfType<ARPlaneManager>();
        }
        
        // If still not found, log error and return
        if (planeManager == null)
        {
            Debug.LogError("[ARPlaneVisualizer] ARPlaneManager not found! Make sure ARPlaneManager is in the scene.");
            return;
        }
        
        // Create default plane material if none assigned
        if (planeMaterial == null)
        {
            planeMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            planeMaterial.color = planeColor;
            planeMaterial.SetFloat("_Surface", 1); // Transparent
            planeMaterial.SetFloat("_Blend", 0); // Alpha
        }
        
        // Subscribe to plane events
        planeManager.planesChanged += OnPlanesChanged;
        
        Debug.Log("[ARPlaneVisualizer] Successfully initialized with ARPlaneManager: " + planeManager.name);
    }
    
    void OnPlanesChanged(ARPlanesChangedEventArgs eventArgs)
    {
        // Add visualizer to new planes
        foreach (var plane in eventArgs.added)
        {
            AddPlaneVisualizer(plane);
        }
        
        // Remove visualizer from removed planes
        foreach (var plane in eventArgs.removed)
        {
            RemovePlaneVisualizer(plane);
        }
    }
    
    void AddPlaneVisualizer(ARPlane plane)
    {
        // Create a simple quad to visualize the plane
        GameObject planeVisualizer = GameObject.CreatePrimitive(PrimitiveType.Quad);
        planeVisualizer.name = "PlaneVisualizer_" + plane.trackableId.ToString();
        planeVisualizer.transform.SetParent(plane.transform, false);
        
        // Apply material
        var renderer = planeVisualizer.GetComponent<Renderer>();
        renderer.material = planeMaterial;
        
        // Make it face up
        planeVisualizer.transform.localRotation = Quaternion.Euler(90, 0, 0);
        
        // Scale to match plane size
        planeVisualizer.transform.localScale = new Vector3(plane.size.x, plane.size.y, 1);
        
        Debug.Log($"Plane detected: {plane.trackableId} at {plane.transform.position}");
    }
    
    void RemovePlaneVisualizer(ARPlane plane)
    {
        // Find and destroy the visualizer
        Transform visualizer = plane.transform.Find("PlaneVisualizer_" + plane.trackableId.ToString());
        if (visualizer != null)
        {
            Destroy(visualizer.gameObject);
        }
    }
    
    void OnDestroy()
    {
        if (planeManager != null)
            planeManager.planesChanged -= OnPlanesChanged;
    }
}