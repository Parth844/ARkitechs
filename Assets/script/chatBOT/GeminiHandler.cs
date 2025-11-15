using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public class GeminiHandler : MonoBehaviour
{
    [Header("API Settings")]
    // Replace with your Gemini API Key
    public string apiKey = "AIzaSyBERaJaP-JgA1E4vNJ8nSXgGM3Tmw50xqM";

    // Correct Gemini endpoint
    private string apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";

    // Main request function
    public IEnumerator SendToGemini(string userMessage, System.Action<string> callback)
    {
        // ✅ Correct JSON body format (matches dashboard example)
        string jsonBody = "{ \"contents\": [ { \"parts\": [ { \"text\": \"" + userMessage + "\" } ] } ] }";
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            // ✅ Correct headers (no Bearer, must use X-goog-api-key)
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("X-goog-api-key", apiKey);

            // Optional: bypass SSL if Unity certificates are broken
            request.certificateHandler = new BypassCertificate();

            // Send request
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Gemini API Error: " + request.error + "\nResponse: " + request.downloadHandler.text);
                callback?.Invoke("Error: " + request.error);
            }
            else
            {
                Debug.Log("Gemini Raw Response: " + request.downloadHandler.text);

                // ✅ Parse JSON response safely
                var json = JSON.Parse(request.downloadHandler.text);
                string reply = "No response";

                var candidates = json["candidates"].AsArray;
                if (candidates != null && candidates.Count > 0)
                {
                    var parts = candidates[0]["content"]["parts"].AsArray;
                    if (parts != null && parts.Count > 0)
                    {
                        reply = parts[0]["text"];
                    }
                }

                callback?.Invoke(reply);
            }
        }
    }
}

// ⚠️ Certificate bypass handler (for Unity SSL issues).
// REMOVE this in production!
public class BypassCertificate : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData) { return true; }
}