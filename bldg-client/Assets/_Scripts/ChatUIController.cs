using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Models;


public class ChatUIController : MonoBehaviour {


    public TMP_InputField ChatInputField;

    public TMP_Text ChatDisplayOutput;

    public Scrollbar ChatScrollbar;

    private ResidentController rsdtController;


    public void SetResidentController(ResidentController controller) {
        rsdtController = controller;
    }


    void OnEnable()
    {
        ChatInputField.onSubmit.AddListener(HandleNewMessage);
    }

    void OnDisable()
    {
        ChatInputField.onSubmit.RemoveListener(HandleNewMessage);
    }


    void HandleNewMessage(string text) {
        AddToChatOutput(text);
        SendChatMessage(text);
    }

    void AddToChatOutput(string newText)
    {
        // Clear Input Field
        ChatInputField.text = string.Empty;

        var timeNow = System.DateTime.Now;

        string formattedInput = "[<#FFFF80>" + timeNow.Hour.ToString("d2") + ":" + timeNow.Minute.ToString("d2") + ":" + timeNow.Second.ToString("d2") + "</color>] " + newText;

        if (ChatDisplayOutput != null)
        {
            // No special formatting for first entry
            // Add line feed before each subsequent entries
            if (ChatDisplayOutput.text == string.Empty)
                ChatDisplayOutput.text = formattedInput;
            else
                ChatDisplayOutput.text += "\n" + formattedInput;
        }

        // Keep Chat input field active
        ChatInputField.ActivateInputField();

        // Set the scrollbar to the bottom when next text is submitted.
        ChatScrollbar.value = 0;
    }

    void SendChatMessage(string text) {
        Debug.Log("Got new text " + text);
        if (rsdtController) {
            Debug.Log("Found a rsdt controller");
            rsdtController.SendSayAction(new SayAction {
                resident_email = rsdtController.resident.email,
                action_type = "SAY",
                say_text = text
            });
        }
    }

}
