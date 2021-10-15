using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;

public class BldgChatController : MonoBehaviour
{

    public ResidentController rsdtController;
    public Resident currentResident;

    public ChatUIController chatUIController;

    public void SetResidentController(ResidentController controller) {
        chatUIController.SetResidentController(controller);
    }

    public void AddHistoricMessages(string from, string[] messages) {
        foreach (string msg in messages) {
            chatUIController.AddMessageToHistory(from, msg);
        }
    }

    public void ClearMessageHistory() {
        chatUIController.ClearMessageHistory();
    }
}
