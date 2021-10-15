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
}
