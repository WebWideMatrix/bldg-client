using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Models;


public class ChatUIController : MonoBehaviour {

    public int CHAT_HISTORY_SIZE = 10;

    public TMP_InputField ChatInputField;

    public TMP_Text ChatDisplayOutput;
    public GameObject ChatHistoryDisplay;

    public Scrollbar ChatScrollbar;

    private ResidentController rsdtController;
    private string residentAlias;

    List<SayAction> chatHistory;

    private UnityAction onLogin;

    

    void OnEnable()
    {
        ChatInputField.onSubmit.AddListener(HandleNewMessage);
        EventManager.StartListening("LoginSuccessful", onLogin);
    }

    void OnDisable()
    {
        ChatInputField.onSubmit.RemoveListener(HandleNewMessage);
        EventManager.StopListening("LoginSuccessful", onLogin);
    }


    private void Awake()
    {
        onLogin = new UnityAction(OnLogin);
    }

    private void OnLogin()
    {
        ChatInputField.gameObject.SetActive(true);
        // ChatInputField.ActivateInputField();
        ChatHistoryDisplay.gameObject.SetActive(true);
    }



    public void SetResidentController(ResidentController controller) {
        rsdtController = controller;
        residentAlias = rsdtController.resident.alias;
    }

    public void AddMessageToHistory(string from, SayAction msg) {
        chatHistory.Add(msg);
        chatHistory.Sort(delegate(SayAction m1, SayAction m2) {
            if (m1.say_time > m2.say_time) return 1;
            if (m1.say_time < m2.say_time) return -1;
            return 0;
        });

        int toSkip = 0;
        if (chatHistory.Count > CHAT_HISTORY_SIZE) {
            toSkip = chatHistory.Count - CHAT_HISTORY_SIZE;
        }
        ChatDisplayOutput.text = string.Empty;
        foreach (SayAction m in chatHistory) {
            if (toSkip > 0) {
                toSkip--;
                continue;
            }
            Debug.Log("Rendering message");
            string formattedInput = formatMessage(m.say_speaker, m.say_text);
            if (ChatDisplayOutput.text == string.Empty)
                ChatDisplayOutput.text = formattedInput;
            else
                ChatDisplayOutput.text += "\n" + formattedInput;
        }

        // Set the scrollbar to the bottom when next text is submitted.
        ChatScrollbar.value = 0;
    }

    string formatMessage(string from, string text) {
        return "[<#FFFF80>" + from + "</color>] " + text;
    }

    public void ClearMessageHistory() {
        ChatDisplayOutput.text = string.Empty;
        chatHistory = new List<SayAction>();
    }

    void HandleNewMessage(string text) {
        AddToChatOutput(text);
        SendChatMessage(text);
    }

    void AddToChatOutput(string newText)
    {
        // Clear Input Field
        ChatInputField.text = string.Empty;

        // var timeNow = System.DateTime.Now;

        // string formattedInput = "[<#FFFF80>" + timeNow.Hour.ToString("d2") + ":" + timeNow.Minute.ToString("d2") + ":" + timeNow.Second.ToString("d2") + "</color>] " + newText;
        string formattedInput = formatMessage(residentAlias, newText);


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
        // TODO extract recipient
        if (rsdtController) {
            Debug.Log("Found a rsdt controller");
            rsdtController.SendSayAction(new SayAction {
                resident_email = rsdtController.resident.email,
                action_type = "SAY",
                say_speaker = rsdtController.resident.alias,
                say_text = text,
                say_time = DateTime.Now.Ticks,
                say_flr = rsdtController.resident.flr,
                say_location = rsdtController.resident.location,
                say_mimetype = "text/plain",
                say_recipient = null
            });
        }
    }

}
