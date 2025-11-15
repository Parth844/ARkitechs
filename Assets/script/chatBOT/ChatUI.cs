using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem; // <-- for new Input System

public class ChatUI : MonoBehaviour
{
    public InputField inputField;   // Where user types their message
    public Text chatHistory;        // Where the conversation is displayed
    public ScrollRect scrollRect;   // To scroll the chat automatically
    public GeminiHandler chatGemini;  // Reference to the script that calls Gemini API

    private bool isWaitingResponse = false; // prevent multiple messages at once

    public void OnSendButton()
    {
        string userMessage = inputField.text;  
        if (string.IsNullOrWhiteSpace(userMessage)) return;  
        if (isWaitingResponse) return;

        // Display user message in the chat window
        chatHistory.text += "\nYou: " + userMessage;
        inputField.text = "";  
        AutoScroll();          

        // Call Gemini API
        StartCoroutine(chatGemini.SendToGemini(userMessage, (response) =>
        {
            // Display bot reply
            chatHistory.text += "\nBot: " + response;
            AutoScroll();  
            isWaitingResponse = false;
        }));

        isWaitingResponse = true;
    }

    private void AutoScroll()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }

    void Update()
    {
        // Using new Input System
        if (Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
        {
            OnSendButton();
        }
    }
}
