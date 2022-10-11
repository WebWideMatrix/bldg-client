using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;

public class BldgChatController : MonoBehaviour
{    

    // public ChatUIController chatUIController;
    public ChatUIController2 chatUIController;

    public void SetResidentController(ResidentController controller) {
        chatUIController.SetResidentController(controller);
    }

    public void AddHistoricMessages(string[] messages) {
        foreach (string msg in messages) {
            SayAction msgObject = JsonUtility.FromJson<SayAction>(msg);
            Debug.Log("~~~~~~~~ Adding historic message: " + msgObject.say_text);
            chatUIController.AddMessageToHistory(msgObject.say_speaker, msgObject);
        }
        // Debug.Log("Done adding them to chat history");
    }

    public void ClearMessageHistory() {
        chatUIController.ClearMessageHistory();
    }
}
