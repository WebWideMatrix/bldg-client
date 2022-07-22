
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Models;



public class ChatUIController2 : MonoBehaviour {

    public VerticalLayoutGroup listOfMessages;
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

        RedrawChatHistory();

        //GameObject newMessage = Instantiate(chatMessageObj, new Vector3 (0,0,0), Quaternion.identity) as GameObject;
        //newMessage.transform.parent = listOfMessages.transform;

        // TODO check if you have enough children - at least |chatHistory| - and create more if needed
        // TODO populte the content of the |chatHistory| items 
        // TODO SetActive to the populated children
    }


    void RedrawChatHistory() {
        Debug.Log("@@@@ There are " + chatHistory.Count + " messages in the chat to draw");
        Debug.Log("@@@@ There are " + listOfMessages.transform.childCount + " chat message children");

        // add more children if needed
        if (chatHistory.Count > listOfMessages.transform.childCount) {
            for (int j = listOfMessages.transform.childCount; j < chatHistory.Count; j++) {
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
            if (i < chatHistory.Count) {
                SayAction msg = chatHistory[i];
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