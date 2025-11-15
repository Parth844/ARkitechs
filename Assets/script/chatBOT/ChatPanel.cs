using UnityEngine;
using UnityEngine.UI;

public class ChatUIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject chatPanel;        // The entire chat panel (ScrollView + InputField + Button)
    public InputField inputField;       // InputField for typing messages
    public ScrollRect scrollRect;       // ScrollView to show messages
    public Text chatHistory;            // Text component that shows conversation
    public GeminiHandler chatGemini;    // Reference to Gemini API handler

    private bool isWaitingResponse = false; // prevent sending multiple messages simultaneously

    // Toggle the chat panel visibility
    public void ToggleChat()
    {
        chatPanel.SetActive(!chatPanel.activeSelf);
    }

    // Called by the Send Button
    public void OnSendButton()
    {
        string userMessage = inputField.text;

        if (string.IsNullOrWhiteSpace(userMessage)) return; // ignore empty messages
        if (isWaitingResponse) return;                       // wait if bot is responding

        // Show user's message
        chatHistory.text += "\nYou: " + userMessage;
        inputField.text = "";      // clear InputField
        AutoScroll();

        // Send message to Gemini API
        StartCoroutine(chatGemini.SendToGemini(userMessage, (response) =>
        {
            chatHistory.text += "\nBot: " + response;
            AutoScroll();
            isWaitingResponse = false;
        }));

        isWaitingResponse = true;
    }

    // Automatically scroll to the bottom of the chat
    private void AutoScroll()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }
}
