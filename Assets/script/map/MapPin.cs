using UnityEngine;
using UnityEngine.UI;

public class MapPin : MonoBehaviour
{
    public string panoramaResourceName;

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            PanoramaViewer.Instance?.OpenPanorama(panoramaResourceName);
        });
    }
}
