using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class GeminiNarrationAPI : MonoBehaviour
{
    public string apiKey;

    [Serializable]
    public class GeminiResponse
    {
        public Candidate[] candidates;
    }

    [Serializable]
    public class Candidate
    {
        public Content content;
    }

    [Serializable]
    public class Content
    {
        public Part[] parts;
    }

    [Serializable]
    public class Part
    {
        public string text;
    }

    /// <summary>
    /// Returns a narrated description of the identified monument.
    /// </summary>
    public IEnumerator GenerateNarration(string monumentName, Action<string> callback)
    {
        string prompt = $"Give a short historical narration (80–120 words) about the monument: {monumentName}.";

        string json = "{\"contents\": [{\"parts\": [{\"text\": \"" + prompt + "\"}]}]}";

        string url =
            $"https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent?key={apiKey}";

        UnityWebRequest req = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Gemini Narration API Error: " + req.error);
            callback("");
            yield break;
        }

        string responseText = req.downloadHandler.text;
        Debug.Log("Gemini Narration Raw Response: " + responseText);

        GeminiResponse res = JsonUtility.FromJson<GeminiResponse>(responseText);

        if (res?.candidates == null ||
            res.candidates.Length == 0 ||
            res.candidates[0].content.parts.Length == 0)
        {
            Debug.LogWarning("⚠ Gemini responded but no narration found.");
            callback("");
            yield break;
        }

        string narration = res.candidates[0].content.parts[0].text;
        callback(narration);
    }
}
