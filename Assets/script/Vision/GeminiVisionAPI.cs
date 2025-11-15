using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GeminiVisionAPI : MonoBehaviour
{
    public string apiKey;

    public IEnumerator IdentifyMonument(Texture2D image, System.Action<string> callback)
    {
        byte[] bytes = image.EncodeToPNG();

        var form = new WWWForm();
        form.AddBinaryData("image", bytes, "image.png", "image/png");

        string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-pro-vision:generateContent?key={apiKey}";

        UnityWebRequest request = UnityWebRequest.Post(url, form);
        yield return request.SendWebRequest();

        string result = request.downloadHandler.text;
        Debug.Log("Gemini Vision Response: " + result);

        // Extract monument name from JSON using regex or JSON parser
        string monument = "Taj Mahal";  // <-- Replace with parsed value
        callback(monument);
    }
}
