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
    public TMP_InputField entityWebsiteInput;
    

    public void OnEnable() {
        // default action is Create
        showCreateForm();
    }
    
    public void ShowFormForSelectedAction() {
        string action = actionInput.options[actionInput.value].text;

        switch (action) {
            case "Create": 
                showCreateForm();
                break;
            case "Move":
                showMoveForm();
                break; 
            case "Connect":
                // TODO support Connect command
                showConnectForm();
                break; 
            case "Update":
                // TODO support update command
                showUpdateForm();
                break;
            default: 
                throw new Exception("Unknown command: " + action);
        }
    }

    
    public void SendCommand() {
        string action = actionInput.options[actionInput.value].text;
        string entity = entityInput.options[entityInput.value].text;
        string name = nameInput.text;
        string website = websiteInput.text;
        string summary = summaryInput.text;
        string picture = pictureInput.text;
        string entityWebsite = entityWebsiteInput.text;
        
        Dictionary<string, string> commandData = new Dictionary<string, string> {
            {"action", action},
            {"entity", entity},
            {"name", name},
            {"website", website},
            {"summary", summary},
            {"picture", picture},
            {"entityWebsite", entityWebsite}
        };

        string command = "";
        switch (action) {
            case "Create": 
                command = generateCreateCommand(commandData);
                break;
            case "Move":
                command = generateMoveCommand(commandData);
                break; 
            // TODO support Connect command

            // TODO support update command

            default: 
                throw new Exception("Unknown command: " + action);
        }
        if (command != "") {
            Debug.Log("Sending command from Quick Actions dialog: " + command);
            chatController.HandleNewMessage(command);
        }
    }
    
    //
    // GENERATE COMMANDS
    //

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
    

    string generateMoveCommand(Dictionary<string, string> data) {
        string command = "/move bldg";
        if (data["entityWebsite"] != "") {
            command += " " + data["entityWebsite"];
        } else {
            // TODO required field validation
        }
        command += " here";

        command = command.ToLower();
        return command;
    }


    
    //
    // GENERATE COMMANDS
    //
    
    void showCreateForm() {
        // TODO use arrays

        // show controls
        entityInput.transform.parent.gameObject.SetActive(true);
        nameInput.transform.parent.gameObject.SetActive(true);
        websiteInput.transform.parent.gameObject.SetActive(true);
        summaryInput.transform.parent.gameObject.SetActive(true);
        pictureInput.transform.parent.gameObject.SetActive(true);

        // hide rest
        entityWebsiteInput.transform.parent.gameObject.SetActive(false);
    }

    void showMoveForm() {
        // TODO use arrays

        // show controls
        entityWebsiteInput.transform.parent.gameObject.SetActive(true);

        // hide rest
        entityInput.transform.parent.gameObject.SetActive(false);
        nameInput.transform.parent.gameObject.SetActive(false);
        websiteInput.transform.parent.gameObject.SetActive(false);
        summaryInput.transform.parent.gameObject.SetActive(false);
        pictureInput.transform.parent.gameObject.SetActive(false);
    }

    void showConnectForm() {
        // TODO use arrays

        // show controls

        // hide rest
        entityInput.transform.parent.gameObject.SetActive(false);
        nameInput.transform.parent.gameObject.SetActive(false);
        websiteInput.transform.parent.gameObject.SetActive(false);
        summaryInput.transform.parent.gameObject.SetActive(false);
        pictureInput.transform.parent.gameObject.SetActive(false);
        entityWebsiteInput.transform.parent.gameObject.SetActive(false);
    }

    void showUpdateForm() {
        // TODO use arrays

        // show controls

        // hide rest
        entityInput.transform.parent.gameObject.SetActive(false);
        nameInput.transform.parent.gameObject.SetActive(false);
        websiteInput.transform.parent.gameObject.SetActive(false);
        summaryInput.transform.parent.gameObject.SetActive(false);
        pictureInput.transform.parent.gameObject.SetActive(false);
        entityWebsiteInput.transform.parent.gameObject.SetActive(false);
    }
}
