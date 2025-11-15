using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SketchfabAPI : MonoBehaviour
{
    public string apiToken;

    public IEnumerator SearchModel(string query, System.Action<string> callback)
    {
        string url = $"https://api.sketchfab.com/v3/search?q={query}&type=models";

        UnityWebRequest req = UnityWebRequest.Get(url);
        req.SetRequestHeader("Authorization", "Token " + apiToken);

        yield return req.SendWebRequest();

        string data = req.downloadHandler.text;

        // TODO: Parse JSON correctly (model URL)
        string glbURL = ExtractDownloadURL(data);

        callback(glbURL);
    }

    private string ExtractDownloadURL(string json)
    {
        // placeholder parser
        return "https://example.com/model.glb";
    }
}
