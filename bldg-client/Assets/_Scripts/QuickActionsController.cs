using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuickActionsController : MonoBehaviour
{
    [Header("Code Resources")]
    public ChatUIController2 chatController;

    [Header("UI Resources")]
    public TMP_Dropdown actionInput;
    public TMP_Dropdown entityInput;
    public TMP_InputField nameInput;
    public TMP_InputField websiteInput;
    public TMP_InputField summaryInput;
    public TMP_InputField pictureInput;
    
    
    public void SendCommand() {
        Debug.Log("#########################");
        string action = actionInput.options[actionInput.value].text;
        string entity = entityInput.options[entityInput.value].text;
        string name = nameInput.text;
        string website = websiteInput.text;
        string summary = summaryInput.text;
        string picture = pictureInput.text;
        Dictionary<string, string> commandData = new Dictionary<string, string> {
            {"action", action},
            {"entity", entity},
            {"name", name},
            {"website", website},
            {"summary", summary},
            {"picture", picture}
        };


        string command = action;
        switch (action) {
            case "Create": 
                command = generateCreateCommand(commandData);
                break;
            default: 
                throw new Exception("Unknown command: " + action);
        }

        Debug.Log("Sending command from Quick Actions dialog: " + command);
        chatController.HandleNewMessage(command);
    }
    

    string generateCreateCommand(Dictionary<string, string> data) {
        string command = "/create " + data["entity"] + " bldg";
        if (data["name"] != "") {
            command += " with name " + data["name"];
        }
        if (data["website"] != "") {
            command += " and website " + data["website"];
        }
        if (data["summary"] != "") {
            command += " and summary " + data["summary"];
        }
        if (data["picture"] != "") {
            command += " and picture " + data["picture"];
        }

        command = command.ToLower();
        return command;
    }
    
}
