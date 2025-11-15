// Assets/Scripts/Firebase/FirebaseAppInitializer.cs
using UnityEngine;
using Firebase;
using System.Threading.Tasks;

public class FirebaseAppInitializer : MonoBehaviour
{
    public static bool IsFirebaseReady { get; private set; } = false;
    public static DependencyStatus DependencyStatus { get; private set; }

    private async void Awake()
    {
        DontDestroyOnLoad(gameObject);
        DependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (DependencyStatus == DependencyStatus.Available)
        {
            IsFirebaseReady = true;
            Debug.Log("✅ Firebase is ready!");
        }
        else
        {
            IsFirebaseReady = false;
            Debug.LogError("❌ Firebase dependency error: " + DependencyStatus);
        }
    }
}
