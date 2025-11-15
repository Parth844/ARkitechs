using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class ARPlacementHandler : MonoBehaviour
{
[Tooltip("Prefab used to anchor loaded runtime models")]
public GameObject anchorPrefab; // ARModelAnchor prefab

ARRaycastManager raycastManager;
List<ARRaycastHit> hits = new List<ARRaycastHit>();

GameObject spawnedAnchor;

void Awake()
{
raycastManager = GetComponent<ARRaycastManager>();
}

void Update()
{
// Single touch placement for mobile / tap
if (Input.touchCount == 0) return;

var touch = Input.GetTouch(0);
if (touch.phase != TouchPhase.Began) return;

if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
{
var hitPose = hits[0].pose;
PlaceAnchor(hitPose.position, hitPose.rotation);
}
}

/// <summary>
/// Instantiate or move the anchor to given pose.
/// Returns the anchor GameObject so caller can parent loaded models.
/// </summary>
public GameObject PlaceAnchor(Vector3 position, Quaternion rotation)
{
if (spawnedAnchor == null)
{
spawnedAnchor = Instantiate(anchorPrefab, position, rotation);
}
else
{
spawnedAnchor.transform.SetPositionAndRotation(position, rotation);
}

return spawnedAnchor;
}

/// <summary>
/// Remove the anchor (and its children) from scene.
/// </summary>
public void ClearAnchor()
{
if (spawnedAnchor != null)
{
Destroy(spawnedAnchor);
spawnedAnchor = null;
}
}
}