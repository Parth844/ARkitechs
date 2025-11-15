using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Auth;
using UnityEngine.UI;

public class HomePageManager : MonoBehaviour
{
    public Button logoutButton;
    public Text welcomeText;

    void Start()
    {
        logoutButton.onClick.AddListener(OnLogoutButtonClicked);

        var user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user != null)
            welcomeText.text = "Welcome, " + user.Email;
    }

    public void OnLogoutButtonClicked()
    {
        FirebaseAuth.DefaultInstance.SignOut();
        SceneManager.LoadScene("LoginUI");
    }
}
