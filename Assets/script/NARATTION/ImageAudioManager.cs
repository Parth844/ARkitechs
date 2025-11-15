using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class ImageAudioManager : MonoBehaviour
{
    [Header("AR Components")]
    public ARTrackedImageManager trackedImageManager;

    [Header("Audio")]
    public AudioSource audioSource;
    public List<AudioClip> audioClips; // Assign in Inspector
    public List<string> imageNames;    // Match names of reference images

    [Header("UI")]
    public Button speakButton;

    private Dictionary<string, AudioClip> audioDict = new Dictionary<string, AudioClip>();
    private string currentImageName = null;

    void Awake()
    {
        // Map image name -> audio clip
        for (int i = 0; i < imageNames.Count; i++)
        {
            if (i < audioClips.Count)
                audioDict[imageNames[i]] = audioClips[i];
        }

        // Button listener
        speakButton.onClick.AddListener(ToggleAudio);
    }

    void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            SetCurrentImage(trackedImage.referenceImage.name);
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            if (trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
            {
                SetCurrentImage(trackedImage.referenceImage.name);
            }
        }
    }

    void SetCurrentImage(string imageName)
    {
        if (audioDict.ContainsKey(imageName))
        {
            currentImageName = imageName;
            audioSource.clip = audioDict[imageName];
            audioSource.Stop(); // Reset when new image is detected
        }
    }

    void ToggleAudio()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        else if (currentImageName != null && audioSource.clip != null)
        {
            audioSource.Play();
        }
    }
}
