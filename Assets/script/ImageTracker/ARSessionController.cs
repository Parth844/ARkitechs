using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARSessionController : ARTrackableManager<
    XRImageTrackingSubsystem,
    XRImageTrackingSubsystemDescriptor,
    XRImageTrackingSubsystem.Provider,
    XRTrackedImage,
    ARTrackedImage>
{
    [SerializeField] private GameObject prefabToSpawn;

    // Required abstract property
    protected override string gameObjectName => "ARTrackedImage";

    // Optional prefab spawning
    protected override GameObject GetPrefab()
    {
        return prefabToSpawn;
    }

    // ✅ Correct override for AR Foundation 6
    protected override void OnTrackablesChanged(
        List<ARTrackedImage> added,
        List<ARTrackedImage> updated,
        List<ARTrackedImage> removed)
    {
        // Added
        foreach (var trackedImage in added)
        {
            Debug.Log($"Image Added: {trackedImage.referenceImage.name}");
            if (prefabToSpawn != null)
            {
                var obj = Instantiate(prefabToSpawn, trackedImage.transform);
                obj.name = trackedImage.referenceImage.name + "_Instance";
            }
        }

        // Updated
        foreach (var trackedImage in updated)
        {
            Debug.Log($"Image Updated: {trackedImage.referenceImage.name}");
        }

        // Removed
        foreach (var trackedImage in removed)
        {
            Debug.Log($"Image Removed: {trackedImage.referenceImage.name}");
            Destroy(trackedImage.gameObject);
        }
    }
}