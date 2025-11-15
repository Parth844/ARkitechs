using System;
using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;

public class FirebaseDatabaseManager : MonoBehaviour
{
    public static FirebaseDatabaseManager Instance { get; private set; }

    private DatabaseReference dbRef;

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize Firebase Database reference
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        Debug.Log("✅ Firebase DatabaseManager ready");
    }

    /// <summary>
    /// Saves user profile info in Firebase Database.
    /// </summary>
    /// <param name="userId">Firebase User ID</param>
    /// <param name="name">User Name</param>
    /// <param name="email">User Email</param>
    /// <param name="callback">Action<bool, string>: success, error message</param>
    public void SaveUserProfile(string userId, string name, string email, Action<bool, string> callback)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email))
        {
            callback?.Invoke(false, "Invalid input");
            return;
        }

        // Create a user object
        UserProfileData user = new UserProfileData
        {
            Name = name,
            Email = email
        };

        string json = JsonUtility.ToJson(user);

        // Save to /users/userId/
        dbRef.Child("users").Child(userId).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
            {
                Debug.Log("✅ User profile saved: " + userId);
                callback?.Invoke(true, null);
            }
            else
            {
                string err = task.Exception?.GetBaseException().Message ?? "Unknown error";
                Debug.LogError("❌ SaveUserProfile failed: " + err);
                callback?.Invoke(false, err);
            }
        });
    }
}

/// <summary>
/// Helper class to store user data
/// </summary>
[Serializable]
public class UserProfileData
{
    public string Name;
    public string Email;
}
