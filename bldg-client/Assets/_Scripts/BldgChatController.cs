using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;

public class BldgChatController : MonoBehaviour
{    

    public ChatUIController chatUIController;

    public void SetResidentController(ResidentController controller) {
        chatUIController.SetResidentController(controller);
    }

    public void AddHistoricMessages(string from, string[] messages) {
        foreach (string msg in messages) {
            SayAction msgObject = JsonUtility.FromJson<SayAction>(msg);
            chatUIController.AddMessageToHistory(from, msgObject);
        }
    }

    public void ClearMessageHistory() {
        chatUIController.ClearMessageHistory();
    }
}
