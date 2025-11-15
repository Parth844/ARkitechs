using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class ARImageHandler : MonoBehaviour
{
    private ARTrackedImageManager trackedImageManager;

    [Tooltip("Assign prefabs you want to spawn for each reference image. Use matching names or fallback to index 0.")]
    public GameObject[] imagePrefabs;

    // Map referenceImage name -> spawned instance
    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();

    void Awake()
    {
        trackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        // Added
        foreach (var trackedImage in eventArgs.added)
        {
            if (trackedImage == null || trackedImage.transform == null) continue;
            HandleTrackedImage(trackedImage);
        }

        // Updated
        foreach (var trackedImage in eventArgs.updated)
        {
            if (trackedImage == null || trackedImage.transform == null) continue;
            HandleTrackedImage(trackedImage);
        }

        // Removed
        foreach (var trackedImage in eventArgs.removed)
        {
            if (trackedImage == null) continue;
            var name = trackedImage.referenceImage.name;
            if (spawnedPrefabs.TryGetValue(name, out GameObject go))
            {
                if (go != null) Destroy(go);
                spawnedPrefabs.Remove(name);
            }
        }
    }

    private void HandleTrackedImage(ARTrackedImage trackedImage)
    {
        var name = trackedImage.referenceImage.name;

        // Already spawned? update transform + active state
        if (spawnedPrefabs.TryGetValue(name, out GameObject existing))
        {
            if (existing != null)
            {
                existing.transform.position = trackedImage.transform.position;
                existing.transform.rotation = trackedImage.transform.rotation;
                existing.SetActive(trackedImage.trackingState == TrackingState.Tracking);
            }
            return;
        }

        // Choose prefab (try match by name first)
        GameObject prefab = null;
        if (imagePrefabs != null && imagePrefabs.Length > 0)
        {
            foreach (var p in imagePrefabs)
            {
                if (p != null && p.name == name)
                {
                    prefab = p;
                    break;
                }
            }
            if (prefab == null) prefab = imagePrefabs[0];
        }

        if (prefab == null) return;

        GameObject spawned = Instantiate(prefab, trackedImage.transform.position, trackedImage.transform.rotation);
        spawned.transform.SetParent(trackedImage.transform, worldPositionStays: true);
        spawned.name = prefab.name + "_" + name;
        spawnedPrefabs[name] = spawned;
    }
}