using UnityEngine;
using GLTFast;

public class ModelRuntimeLoader : MonoBehaviour
{
    public async void LoadGLB(string url, Transform parent)
    {
        var gltf = new GltfImport();
        bool success = await gltf.Load(new System.Uri(url));

        if (success)
        {
            GameObject root = new GameObject("LoadedModel");
            gltf.InstantiateMainScene(root.transform);
            root.transform.SetParent(parent);
        }
    }
}
