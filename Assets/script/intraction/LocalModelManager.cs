using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using GLTFast;   // ✅ comes from the glTFast package

public class LocalModelManager : MonoBehaviour
{
    [Header("UI References")]
    public PlayModeToggle playModeToggle; // Assign from Inspector
    public Text statusText;               // Optional UI status feedback

    [Header("Spawn Settings")]
    public Vector3 spawnPosition = new Vector3(0, 0, 1f); // Place in front of camera
    public float defaultScale = 1f;   // Increased scale for better visibility
    public bool usePlanePlacement = true; // Whether to place on detected planes

    private GameObject currentModel;

    // Example: Call from UI Buttons
    public void LoadTajMahal()
    {
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, "Models/Taj_mahel.glb");
        StartCoroutine(LoadAndPlace(path));
    }

    public void LoadIndiaGate()
    {
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, "Models/India_gate.glb");
        StartCoroutine(LoadAndPlace(path));
    }

    // Debug method to test with a simple cube
    public void LoadTestCube()
    {
        Debug.Log("📦 Creating test cube...");
        
        // Cleanup previous model
        if (currentModel != null)
            Destroy(currentModel);

        // Create a simple test cube
        currentModel = GameObject.CreatePrimitive(PrimitiveType.Cube);
        currentModel.name = "TestCube";
        currentModel.transform.position = spawnPosition;
        currentModel.transform.localScale = Vector3.one * defaultScale;
        
        // Add a bright material (URP compatible)
        var renderer = currentModel.GetComponent<Renderer>();
        var material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        material.color = Color.red;
        renderer.material = material;

        // Add interaction script
        ModelInteraction interaction = currentModel.GetComponent<ModelInteraction>();
        if (interaction == null)
            interaction = currentModel.AddComponent<ModelInteraction>();

        // Hook to PlayModeToggle
        if (playModeToggle != null)
            playModeToggle.modelInteraction = interaction;

        Debug.Log("✅ Test cube created and ready!");
        if (statusText != null) statusText.text = "✅ Test cube ready!";
    }

    // Method to place model on a specific position (for plane placement)
    public void PlaceModelAtPosition(Vector3 position, Quaternion rotation)
    {
        if (currentModel != null)
        {
            currentModel.transform.position = position;
            currentModel.transform.rotation = rotation;
            Debug.Log($"📦 Model placed at position: {position}");
            if (statusText != null) statusText.text = "✅ Model placed on plane!";
        }
        else
        {
            Debug.LogWarning("⚠️ No model loaded to place. Load a model first.");
            if (statusText != null) statusText.text = "⚠️ Load a model first!";
        }
    }

    private IEnumerator LoadAndPlace(string path)
    {
        // Cleanup previous model
        if (currentModel != null)
            Destroy(currentModel);

        Debug.Log("📂 Loading local model: " + path);
        Debug.Log("📂 File exists: " + System.IO.File.Exists(path));
        Debug.Log("📂 StreamingAssets path: " + Application.streamingAssetsPath);
        if (statusText != null) statusText.text = "Loading model...";

        // Check if file exists
        if (!System.IO.File.Exists(path))
        {
            Debug.LogError("❌ Model file not found at: " + path);
            if (statusText != null) statusText.text = "❌ Model file not found!";
            yield break;
        }

        // ✅ Load .glb using GLTFast
        var loader = new GltfImport();
        Debug.Log("📂 Created GltfImport loader");
        
        var loadTask = loader.Load(path);
        Debug.Log("📂 Started loading task");
        
        while (!loadTask.IsCompleted) 
        {
            Debug.Log("📂 Loading progress...");
            yield return null;
        }

        Debug.Log("📂 Load task completed. Result: " + loadTask.Result);

        if (!loadTask.Result)
        {
            Debug.LogError("❌ Failed to load model from: " + path);
            if (statusText != null) statusText.text = "❌ Failed to load model.";
            yield break;
        }

        // Create parent object
        currentModel = new GameObject("LoadedModel");
        Debug.Log("📂 Created parent GameObject: " + currentModel.name);

        var instTask = loader.InstantiateMainSceneAsync(currentModel.transform);
        Debug.Log("📂 Started instantiation task");
        
        while (!instTask.IsCompleted) 
        {
            Debug.Log("📂 Instantiation progress...");
            yield return null;
        }

        Debug.Log("📂 Instantiation task completed. Result: " + instTask.Result);

        if (!instTask.Result)
        {
            Debug.LogError("❌ Failed to instantiate model!");
            if (statusText != null) statusText.text = "❌ Failed to instantiate.";
            yield break;
        }

        // ✅ Place at floor & scale down
        currentModel.transform.position = spawnPosition;
        currentModel.transform.localScale = Vector3.one * defaultScale;
        Debug.Log("📂 Model positioned at: " + spawnPosition + " with scale: " + (Vector3.one * defaultScale));

        // Check if model has renderers
        var renderers = currentModel.GetComponentsInChildren<Renderer>();
        Debug.Log("📂 Found " + renderers.Length + " renderers in model");
        foreach (var renderer in renderers)
        {
            Debug.Log("📂 Renderer: " + renderer.name + " - Enabled: " + renderer.enabled + " - Visible: " + renderer.isVisible);
        }

        // ✅ Add interaction script
        ModelInteraction interaction = currentModel.GetComponent<ModelInteraction>();
        if (interaction == null)
            interaction = currentModel.AddComponent<ModelInteraction>();

        // ✅ Hook to PlayModeToggle
        if (playModeToggle != null)
            playModeToggle.modelInteraction = interaction;

        Debug.Log("✅ Model instantiated & ready for interaction!");
        if (statusText != null) statusText.text = "✅ Model ready! Toggle Play Mode to interact.";
    }
}
