using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;

public class SignupPageAuthManager : MonoBehaviour
{
    [Header("UI (assign in Inspector)")]
    public InputField nameInput;
    public InputField emailInput;
    public InputField passwordInput;
    public Text statusText;
    public Button signupButton;

    private FirebaseAuth auth;
    private bool isSigningUp = false;

    IEnumerator Start()
    {
        while (FirebaseApp.DefaultInstance == null)
            yield return null;

        auth = FirebaseAuth.DefaultInstance;
        Debug.Log("✅ FirebaseAuth ready (Signup)");

        signupButton.onClick.AddListener(OnSignupButtonClicked);
    }

    public void OnSignupButtonClicked()
    {
        if (isSigningUp) return;
        isSigningUp = true;
        signupButton.interactable = false;

        string name = nameInput.text.Trim();
        string email = emailInput.text.Trim();
        string pass = passwordInput.text;

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pass))
        {
            statusText.text = "Fill all fields";
            isSigningUp = false;
            signupButton.interactable = true;
            return;
        }

        statusText.text = "Creating user...";

        // ✅ Create Firebase user
        auth.CreateUserWithEmailAndPasswordAsync(email, pass).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                string msg = task.Exception?.GetBaseException()?.Message ?? "Unknown error";
                Debug.LogError("❌ Signup failed: " + msg);
                statusText.text = msg.ToLower().Contains("email") ? "Email already in use" : "Signup failed: " + msg;
                isSigningUp = false;
                signupButton.interactable = true;
                return;
            }

            // ✅ User created successfully
            FirebaseUser newUser = task.Result.User;
            Debug.Log("✅ User created: " + newUser.UserId + " | Email: " + newUser.Email);
            statusText.text = "User created! Saving profile...";

            // ✅ Save profile to database
            FirebaseDatabaseManager.Instance.SaveUserProfile(newUser.UserId, name, email, (ok, err) =>
            {
                if (ok)
                {
                    Debug.Log("✅ User profile saved in DB successfully");
                    statusText.text = "Signup complete!";
                    
                    // ✅ Redirect only after profile is saved
                    SceneManager.LoadScene("LoginUI");
                }
                else
                {
                    Debug.LogError("❌ Failed to save profile: " + err);
                    statusText.text = "Signup succeeded but DB save failed!";
                }

                isSigningUp = false;
                signupButton.interactable = true;
            });
        });
    }
}
