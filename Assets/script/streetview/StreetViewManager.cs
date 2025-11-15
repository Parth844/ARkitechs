using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement; // Needed for scene loading

public class StreetViewManager : MonoBehaviour
{
    private WebViewObject webViewObject;

    void Start()
    {
        webViewObject = (new GameObject("WebViewObject")).AddComponent<WebViewObject>();
        webViewObject.Init(
            cb: OnWebMessage, // use a separate callback method
            transparent: false
        );

        webViewObject.SetMargins(0, 0, 0, 0);
        webViewObject.SetVisibility(true);

        string fileName = "streetview.html";
        string url = Path.Combine(Application.streamingAssetsPath, fileName);

#if UNITY_ANDROID
        // On Android, StreamingAssets are inside jar:file://
        string androidUrl = Path.Combine(Application.streamingAssetsPath, fileName);
        Debug.Log("Android URL: " + androidUrl);
        webViewObject.LoadURL(androidUrl.Replace(" ", "%20"));
#elif UNITY_IOS
        // On iOS, just use the path from StreamingAssets
        string iosUrl = "file://" + url;
        Debug.Log("iOS URL: " + iosUrl);
        webViewObject.LoadURL(iosUrl);
#else
        // In Editor / Standalone
        string editorUrl = "file://" + url;
        Debug.Log("Editor URL: " + editorUrl);
        webViewObject.LoadURL(editorUrl);
#endif
    }

    // Handle messages from JavaScript
    private void OnWebMessage(string msg)
    {
        Debug.Log("Message from JS: " + msg);

        if (msg == "ReturnToScene")
        {
            // Reload the same scene or go to another one
            SceneManager.LoadScene("HomePage"); 
            // replace "StreetViewScene" with your scene name
        }
    }
}
