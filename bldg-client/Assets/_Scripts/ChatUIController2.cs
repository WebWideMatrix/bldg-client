
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Models;



public class ChatUIController2 : MonoBehaviour {

    public VerticalLayoutGroup listOfMessages;
    public TMP_InputField chatMessageInput;
    public GameObject chatMessageObj;


    private ResidentController rsdtController;
    private string residentAlias;


    List<SayAction> chatHistory;
    private ScrollRect scrollRect;



    void OnEnable()
    {
        if (scrollRect == null) {
            scrollRect = gameObject.GetComponent<ScrollRect>();
        }
        chatMessageInput.onSubmit.AddListener(HandleNewMessage);
    }

    void OnDisable()
    {
        
        chatMessageInput.onSubmit.RemoveListener(HandleNewMessage);
    }


    void HandleNewMessage(string text) {
        Debug.Log("@@@@@@ Sending new message");

        SayAction act = CreateChatMessage(text);
        AddToChatOutput(act); 
        SendChatMessage(act);
    }


    void AddToChatOutput(SayAction act)
    {
        // Clear Input Field
        chatMessageInput.text = string.Empty;
        List<SayAction> chatWithNewMessage = new List<SayAction>(chatHistory);
        chatWithNewMessage.Add(act);
        RedrawChat(chatWithNewMessage);
    }

    SayAction CreateChatMessage(string text) {
        // TODO extract recipient
        CurrentResidentController crc = CurrentResidentController.Instance;
        Debug.Log("Found a rsdt controller");
        SayAction act = new SayAction {
            resident_email = crc.resident.email,
            action_type = "SAY",
            say_speaker = crc.resident.alias,
            say_text = text,
            say_time = DateTime.Now.Ticks,
            say_flr = crc.resident.flr,
            say_location = crc.resident.location,
            say_mimetype = "text/plain",
            say_recipient = null
        };
        return act;
    }


    void SendChatMessage(SayAction act) {
        // TODO extract recipient
        CurrentResidentController crc = CurrentResidentController.Instance;
        Debug.Log("Found a rsdt controller");
        crc.SendSayAction(act);
    }
    

    public void SetResidentController(ResidentController controller) {
        rsdtController = controller;
        residentAlias = rsdtController.resident.alias;
    }

    public void AddMessageToHistory(string from, SayAction msg) {
        Debug.Log("^^^^^ Invoked to add message to history");
        chatHistory.Add(msg);
        chatHistory.Sort(delegate(SayAction m1, SayAction m2) {
            if (m1.say_time > m2.say_time) return 1;
            if (m1.say_time < m2.say_time) return -1;
            return 0;
        });

        RedrawChat(chatHistory);

        //GameObject newMessage = Instantiate(chatMessageObj, new Vector3 (0,0,0), Quaternion.identity) as GameObject;
        //newMessage.transform.parent = listOfMessages.transform;

        // TODO check if you have enough children - at least |chatHistory| - and create more if needed
        // TODO populte the content of the |chatHistory| items 
        // TODO SetActive to the populated children
    }


    void RedrawChat(List<SayAction> chat) {
        Debug.Log("@@@@ There are " + chat.Count + " messages in the chat to draw");
        Debug.Log("@@@@ There are " + listOfMessages.transform.childCount + " chat message children");

        // add more children if needed
        if (chat.Count > listOfMessages.transform.childCount) {
            for (int j = listOfMessages.transform.childCount; j < chat.Count; j++) {
                Debug.Log("@@@@ Adding chat message child");
                GameObject newMessage = Instantiate(chatMessageObj, listOfMessages.transform) as GameObject;
                newMessage.gameObject.SetActive(false);
                // newMessage.transform.parent = listOfMessages.transform;
            }
        }

        // loop through children
        int i = 0;

        foreach (Transform child in listOfMessages.transform)
        {
            if (i < chat.Count) {
                SayAction msg = chat[i];
                child.gameObject.SetActive(true);
                ChatMessageController msgController = child.gameObject.GetComponent<ChatMessageController>();
                if (msgController != null) {
                    msgController.Clear();
                    msgController.SetMessage(msg.say_speaker, msg.say_text);
                }
                Debug.Log("@@@@ Drawn message " + i);
            } else {
                child.gameObject.SetActive(false);
                Debug.Log("@@@@ chat message " + i + " isn't used, disabled");
            }
            i++;
        }
        scrollRect.verticalNormalizedPosition = 0;
    }


    public void ClearMessageHistory() {
        // TODO clear the chat messages from the UI
        chatHistory = new List<SayAction>();

        foreach (Transform child in listOfMessages.transform)
        {
            // TODO clear content
            child.gameObject.SetActive(false);
        }
    }

}