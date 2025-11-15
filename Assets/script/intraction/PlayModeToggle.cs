using UnityEngine;
using UnityEngine.UI;

public class PlayModeToggle : MonoBehaviour
{
    public Button toggleButton;
    public Text buttonText;
    public Text statusText;   // ✅ UI Text for notification
    public ModelInteraction modelInteraction;

    private bool isPlayMode = false;

    void Start()
    {
        if (toggleButton != null)
            toggleButton.onClick.AddListener(ToggleMode);

        // Show ready notification at start
        if (statusText != null)
            statusText.text = "✅ Model is Ready – Currently in Edit Mode";

        UpdateButtonText();
    }

    void ToggleMode()
    {
        isPlayMode = !isPlayMode;
        modelInteraction.EnableInteraction(isPlayMode);
        UpdateButtonText();

        // Update notification when switching modes
        if (statusText != null)
        {
            if (isPlayMode)
                statusText.text = "🎮 Play Mode Active – You can interact with the model!";
            else
                statusText.text = "✏️ Edit Mode Active – Model interaction disabled.";
        }
    }

    void UpdateButtonText()
    {
        if (buttonText != null)
            buttonText.text = isPlayMode ? "Switch to Edit Mode" : "Switch to Play Mode";
    }
}
