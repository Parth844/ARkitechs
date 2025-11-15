using System.Collections;
using UnityEngine;

public class MainFlowController : MonoBehaviour
{
    [Header("AR Components")]
    public ARCameraFeed cameraFeed;                 // Gets camera frame
    public ARPlacementHandler placementHandler;     // Places model anchor

    [Header("API Components")]
    public GeminiVisionAPI visionAPI;               // Identifies monument
    public SketchfabAPI sketchfabAPI;               // Searches model
    public ModelRuntimeLoader modelLoader;          // Loads GLB model

    [Header("Narration")]
    public GeminiNarrationAPI narrationAPI;         // Gets text narration
    public GoogleTTS googleTTS;                     // Plays TTS audio

    bool workflowRunning = false;

    public void StartProcess()
    {
        if (!workflowRunning)
            StartCoroutine(ProcessFlow());
    }

    IEnumerator ProcessFlow()
    {
        workflowRunning = true;
        Debug.Log("📸 Starting capture…");

        // ------------------------------------------------------
        // 1) CAPTURE FRAME
        // ------------------------------------------------------
        Texture2D frame = null;

        while (frame == null)
        {
            frame = cameraFeed.GetFrame();
            if (frame == null)
                yield return new WaitForSeconds(0.2f);
        }

        Debug.Log("🖼 Frame captured");

        // ------------------------------------------------------
        // 2) IDENTIFY MONUMENT (Gemini Vision)
        // ------------------------------------------------------
        string monumentName = null;
        yield return StartCoroutine(
            visionAPI.IdentifyMonument(frame, result => monumentName = result)
        );

        Destroy(frame);

        if (string.IsNullOrEmpty(monumentName))
        {
            Debug.LogWarning("⚠ No monument identified.");
            workflowRunning = false;
            yield break;
        }

        Debug.Log("🏛 Identified Monument: " + monumentName);

        // ------------------------------------------------------
        // 3) FETCH MODEL FROM SKETCHFAB
        // ------------------------------------------------------
        string glbUrl = null;
        yield return StartCoroutine(
            sketchfabAPI.SearchModel(monumentName, url => glbUrl = url)
        );

        if (string.IsNullOrEmpty(glbUrl))
        {
            Debug.LogWarning("⚠ No model found for " + monumentName);
            workflowRunning = false;
            yield break;
        }

        Debug.Log("🔗 GLB URL: " + glbUrl);

        // ------------------------------------------------------
        // 4) PLACE ANCHOR IN FRONT OF CAMERA
        // ------------------------------------------------------
        var cam = Camera.main.transform;
        Vector3 pos = cam.position + cam.forward * 2f;
        Quaternion rot = Quaternion.LookRotation(-cam.forward);

        GameObject anchor = placementHandler.PlaceAnchor(pos, rot);

        Debug.Log("📍 Anchor placed.");

        // ------------------------------------------------------
        // 5) LOAD THE MODEL
        // ------------------------------------------------------
        modelLoader.LoadGLB(glbUrl, anchor.transform);

        Debug.Log("📦 Loading model…");

        // ------------------------------------------------------
        // 6) GENERATE TEXT NARRATION
        // ------------------------------------------------------
        string narrationText = null;
        yield return StartCoroutine(
            narrationAPI.GenerateNarration(monumentName, text => narrationText = text)
        );

        if (string.IsNullOrEmpty(narrationText))
        {
            Debug.LogWarning("⚠ No narration.");
            workflowRunning = false;
            yield break;
        }

        Debug.Log("📝 Narration received.");

        // ------------------------------------------------------
        // 7) PLAY TTS AUDIO
        // ------------------------------------------------------
        yield return StartCoroutine(googleTTS.Speak(narrationText));

        Debug.Log("🔊 Narration played.");

        workflowRunning = false;
    }
}
