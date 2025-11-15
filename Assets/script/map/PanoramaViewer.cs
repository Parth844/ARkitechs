using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PanoramaViewer : MonoBehaviour
{
    public static PanoramaViewer Instance;
    public CanvasGroup uiCanvas;
    public Button closeButton;

    private Material runtimeSkybox;

    void Awake()
    {
        Instance = this;
        if (closeButton != null) closeButton.onClick.AddListener(Close);
        if (uiCanvas != null) { uiCanvas.alpha = 0; uiCanvas.interactable = false; uiCanvas.blocksRaycasts = false; }
    }

    public void OpenPanorama(string resourceName)
    {
        StartCoroutine(OpenRoutine(resourceName));
    }

    IEnumerator OpenRoutine(string resourceName)
    {
        Texture2D tex = Resources.Load<Texture2D>("Panoramas/" + resourceName);
        if (tex == null)
        {
            Debug.LogWarning("Panorama not found: " + resourceName);
            yield break;
        }

        runtimeSkybox = new Material(Shader.Find("Skybox/Panoramic"));
        runtimeSkybox.SetTexture("_MainTex", tex);
        RenderSettings.skybox = runtimeSkybox;
        DynamicGI.UpdateEnvironment();

        if (uiCanvas != null)
        {
            uiCanvas.blocksRaycasts = true; uiCanvas.interactable = true; uiCanvas.alpha = 1f;
        }
    }

    public void Close()
    {
        if (uiCanvas != null) { uiCanvas.alpha = 0; uiCanvas.blocksRaycasts = false; uiCanvas.interactable = false; }
        RenderSettings.skybox = null;
        if (runtimeSkybox != null) Destroy(runtimeSkybox);
    }
}

