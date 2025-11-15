using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;

public class LoginPageAuthManager : MonoBehaviour
{
    [Header("UI (assign in Inspector)")]
    public InputField emailInput;
    public InputField passwordInput;
    public Button loginButton;
    public Text statusText;

    private FirebaseAuth auth;
    private bool isLoggingIn = false;

    IEnumerator Start()
    {
        // Wait until Firebase is ready
        while (FirebaseApp.DefaultInstance == null)
            yield return null;

        auth = FirebaseAuth.DefaultInstance;
        Debug.Log("✅ FirebaseAuth ready (Login)");

        if (loginButton != null)
            loginButton.onClick.AddListener(OnLoginButtonClicked);
    }

    public void OnLoginButtonClicked()
    {
        if (isLoggingIn) return;
        isLoggingIn = true;
        loginButton.interactable = false;

        string email = emailInput?.text?.Trim();
        string password = passwordInput?.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            statusText.text = "Fill all fields";
            isLoggingIn = false;
            loginButton.interactable = true;
            return;
        }

        statusText.text = "Logging in...";

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            isLoggingIn = false;
            loginButton.interactable = true;

            if (task.IsCanceled || task.IsFaulted)
            {
                string msg = task.Exception?.GetBaseException().Message ?? "Unknown error";
                Debug.LogError("❌ Login failed: " + msg);
                statusText.text = "Login failed: " + msg;
                return;
            }

            FirebaseUser user = task.Result.User;
            Debug.Log("✅ Login success: " + user.Email);
            statusText.text = "Login success!";
            SceneManager.LoadScene("HomePage");
        });
    }
}
