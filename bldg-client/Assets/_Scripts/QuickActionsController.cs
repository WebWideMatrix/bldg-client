using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Michsky.UI.Shift;


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
    public TMP_Dropdown entityWebsiteDropdown1;
    public TMP_Dropdown entityWebsiteDropdown2;
    public TMP_InputField ownerInput;

    public TMP_Text informationText;
    public TMP_Text errorText;

    private UnityAction onEntitiesChange;

    private ModalWindowManager modalManager;


    public void OnEnable() {
        // default action is Create
        showEmptyForm();
        entityInput.ClearOptions();
        CurrentMetadata cm = CurrentMetadata.Instance;
        entityInput.AddOptions(cm.getEntityTypes());
        if (onEntitiesChange == null) {
            onEntitiesChange = new UnityAction(OnEntitiesChange);
        }
        clearForm();
        EventManager.Instance.StartListening("EntitiesChanged", onEntitiesChange);
        modalManager = gameObject.GetComponent<ModalWindowManager>();
    }

    private void OnEntitiesChange()
    {
        entityWebsiteDropdown1.ClearOptions();
        entityWebsiteDropdown2.ClearOptions();
        CurrentMetadata cm = CurrentMetadata.Instance;
        List<string> entities = cm.getAllEntities();
        entityWebsiteDropdown1.AddOptions(entities);
        entityWebsiteDropdown2.AddOptions(entities);
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
            case "Add Owner":
                // TODO support update command
                showAddOwnerForm();
                break;
            case "Remove Owner":
                // TODO support update command
                showRemoveOwnerForm();
                break;

            default: 
                showEmptyForm();
                break;
        }
    }

    
    public void SendCommand() {
        string action = actionInput.options[actionInput.value].text;
        string entity = entityInput.options[entityInput.value].text;
        string name = nameInput.text;
        string website = websiteInput.text;
        string summary = summaryInput.text;
        string picture = pictureInput.text;
        
        string entityWebsite = "";
        if (entityWebsiteDropdown1.options.Count > 0) entityWebsite = entityWebsiteDropdown1.options[entityWebsiteDropdown1.value].text;
        
        string targetEntityWebsite = "";
        if (entityWebsiteDropdown2.options.Count > 0) targetEntityWebsite = entityWebsiteDropdown2.options[entityWebsiteDropdown2.value].text;
        
        string owner = ownerInput.text;
        
        Dictionary<string, string> commandData = new Dictionary<string, string> {
            {"action", action},
            {"entity", entity},
            {"name", name},
            {"website", website},
            {"summary", summary},
            {"picture", picture},
            {"entityWebsite", entityWebsite},
            {"targetEntityWebsite", targetEntityWebsite},
            {"owner", owner}
        };

        string command = "";
        string error = "";
        switch (action) {
            case "Create":
                error = validateCreateForm(commandData);
                if (error == "") {
                    command = generateCreateCommand(commandData);
                } 
                break;
            case "Move":
                error = validateMoveForm(commandData);
                if (error == "") {
                    command = generateMoveCommand(commandData);
                }
                break;             
            case "Connect":
                error = validateConnectForm(commandData);
                if (error == "") {
                    command = generateConnectCommand(commandData);
                }
                break; 

            // TODO support update command

            case "Add Owner":
                error = validateAddOwnerForm(commandData);
                if (error == "") {
                    command = generateAddOwnerCommand(commandData);
                }
                break; 
            case "Remove Owner":
                error = validateRemoveOwnerForm(commandData);
                if (error == "") {
                    command = generateRemoveOwnerCommand(commandData);
                }
                break;

            default: 
                throw new Exception("Unknown command: " + action);
        }
        if (command != "") {
            Debug.Log("Sending command from Quick Actions dialog: " + command);
            chatController.HandleNewMessage(command);
            modalManager.ModalWindowOut();
        } else {
            Debug.Log("Validation error: " + error);
            errorText.text = error;
        }
    }


    //
    // VALIDATE FORM DATA
    //



    string validateCreateForm(Dictionary<string, string> commandData) {
        string error = "";
        if (commandData["entity"] == "" ) {
            error = "Please select an entity type";
        } else if (commandData["name"] == "") {
            error = "Name is required";
        } else if (commandData["name"].IndexOf(' ') > -1) {
            error = "Name cannot contain spaces";
        } else if (commandData["website"] == "") {
            error = "Website is required";
        } else if (commandData["website"].IndexOf(' ') > -1) {
            error = "Website cannot contain spaces";
        } else if (commandData["summary"] == "") {
            error = "Summary is required";
        }
        return error;
    }

    string validateMoveForm(Dictionary<string, string> commandData) {
        string error = "";
        if (commandData["entityWebsite"] == "" ) {
            error = "Please select an entity";
        }
        return error;
    }

    string validateConnectForm(Dictionary<string, string> commandData) {
        string error = "";
        if (commandData["entityWebsite"] == "" ) {
            error = "Please select an entity";
        } else if (commandData["targetEntityWebsite"] == "" ) {
            error = "Please select a target entity";
        }
        return error;
    }

    string validateAddOwnerForm(Dictionary<string, string> commandData) {
        string error = "";
        if (commandData["entityWebsite"] == "" ) {
            error = "Please select an entity";
        } else if (commandData["owner"] == "" ) {
            error = "Please enter an owner email";
        } else if (commandData["owner"].IndexOf(' ') > -1) {
            error = "Owner email cannot contain spaces";
        } else if (commandData["owner"].IndexOf('@') < 1) {
            error = "Owner email needs to be a valid email address";
        }
        return error;
    }

    string validateRemoveOwnerForm(Dictionary<string, string> commandData) {
        string error = "";
        if (commandData["entityWebsite"] == "" ) {
            error = "Please select an entity";
        } else if (commandData["owner"] == "" ) {
            error = "Please enter an owner email";
        } else if (commandData["owner"].IndexOf(' ') > -1) {
            error = "Owner email cannot contain spaces";
        } else if (commandData["owner"].IndexOf('@') < 1) {
            error = "Owner email needs to be a valid email address";
        }
        return error;
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

    string generateConnectCommand(Dictionary<string, string> data) {
        string command = "/connect between";
        if (data["entityWebsite"] != "") {
            command += " " + data["entityWebsite"];
        } else {
            // TODO required field validation
        }
        if (data["targetEntityWebsite"] != "") {
            command += " and " + data["targetEntityWebsite"];
        } else {
            // TODO required field validation
        }

        command = command.ToLower();
        return command;
    }

    string generateAddOwnerCommand(Dictionary<string, string> data) {
        string command = "/add owner";
        if (data["owner"] != "") {
            command += " " + data["owner"];
        } else {
            // TODO required field validation
        }

        if (data["entityWebsite"] != "") {
            command += " to bldg " + data["entityWebsite"];
        } else {
            // TODO required field validation
        }

        command = command.ToLower();
        return command;
    }

    string generateRemoveOwnerCommand(Dictionary<string, string> data) {
        string command = "/remove owner";
        if (data["owner"] != "") {
            command += " " + data["owner"];
        } else {
            // TODO required field validation
        }

        if (data["entityWebsite"] != "") {
            command += " from bldg " + data["entityWebsite"];
        } else {
            // TODO required field validation
        }

        command = command.ToLower();
        return command;
    }




    //
    // SHOW FORMS
    //
    
    void showCreateForm() {
        // TODO use arrays

        // show controls
        entityInput.transform.parent.gameObject.SetActive(true);
        nameInput.transform.parent.gameObject.SetActive(true);
        websiteInput.transform.parent.gameObject.SetActive(true);
        summaryInput.transform.parent.gameObject.SetActive(true);
        pictureInput.transform.parent.gameObject.SetActive(true);

        informationText.text = "Create a new entity";

        // hide rest
        entityWebsiteDropdown1.transform.parent.gameObject.SetActive(false);
        entityWebsiteDropdown2.transform.parent.gameObject.SetActive(false);
        ownerInput.transform.parent.gameObject.SetActive(false);
    }

    void showMoveForm() {
        // TODO use arrays

        // show controls
        entityWebsiteDropdown1.transform.parent.gameObject.SetActive(true);

        informationText.text = "Choose an entity to move to where you are.";

        // hide rest
        entityInput.transform.parent.gameObject.SetActive(false);
        nameInput.transform.parent.gameObject.SetActive(false);
        websiteInput.transform.parent.gameObject.SetActive(false);
        summaryInput.transform.parent.gameObject.SetActive(false);
        pictureInput.transform.parent.gameObject.SetActive(false);
        entityWebsiteDropdown2.transform.parent.gameObject.SetActive(false);
        ownerInput.transform.parent.gameObject.SetActive(false);
    }

    void showConnectForm() {
        // TODO use arrays

        // show controls
        entityWebsiteDropdown1.transform.parent.gameObject.SetActive(true);
        entityWebsiteDropdown2.transform.parent.gameObject.SetActive(true);

        informationText.text = "Choose two entities to connect.";

        // hide rest
        entityInput.transform.parent.gameObject.SetActive(false);
        nameInput.transform.parent.gameObject.SetActive(false);
        websiteInput.transform.parent.gameObject.SetActive(false);
        summaryInput.transform.parent.gameObject.SetActive(false);
        pictureInput.transform.parent.gameObject.SetActive(false);
        ownerInput.transform.parent.gameObject.SetActive(false);
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
        entityWebsiteDropdown1.transform.parent.gameObject.SetActive(false);
        entityWebsiteDropdown2.transform.parent.gameObject.SetActive(false);
        ownerInput.transform.parent.gameObject.SetActive(false);
    }

    void showAddOwnerForm() {
        // TODO use arrays

        // show controls
        entityWebsiteDropdown1.transform.parent.gameObject.SetActive(true);
        ownerInput.transform.parent.gameObject.SetActive(true);

        // hide rest
        entityInput.transform.parent.gameObject.SetActive(false);
        nameInput.transform.parent.gameObject.SetActive(false);
        websiteInput.transform.parent.gameObject.SetActive(false);
        summaryInput.transform.parent.gameObject.SetActive(false);
        pictureInput.transform.parent.gameObject.SetActive(false);
        entityWebsiteDropdown2.transform.parent.gameObject.SetActive(false);
    }

    void showRemoveOwnerForm() {
        // TODO use arrays

        // show controls
        entityWebsiteDropdown1.transform.parent.gameObject.SetActive(true);
        ownerInput.transform.parent.gameObject.SetActive(true);

        // hide rest
        entityInput.transform.parent.gameObject.SetActive(false);
        nameInput.transform.parent.gameObject.SetActive(false);
        websiteInput.transform.parent.gameObject.SetActive(false);
        summaryInput.transform.parent.gameObject.SetActive(false);
        pictureInput.transform.parent.gameObject.SetActive(false);
        entityWebsiteDropdown2.transform.parent.gameObject.SetActive(false);
    }

    void showEmptyForm() {
        // TODO use arrays

        // show controls

        // hide rest
        entityInput.transform.parent.gameObject.SetActive(false);
        nameInput.transform.parent.gameObject.SetActive(false);
        websiteInput.transform.parent.gameObject.SetActive(false);
        summaryInput.transform.parent.gameObject.SetActive(false);
        pictureInput.transform.parent.gameObject.SetActive(false);
        entityWebsiteDropdown1.transform.parent.gameObject.SetActive(false);
        entityWebsiteDropdown2.transform.parent.gameObject.SetActive(false);
        ownerInput.transform.parent.gameObject.SetActive(false);

        informationText.text = "";
        errorText.text = "";
    }

    void clearForm() {
        // clear form
        actionInput.value = 0;
        nameInput.text = "";
        websiteInput.text = "";
        summaryInput.text = "";
        pictureInput.text = "";
        entityWebsiteDropdown1.value = 0;
        entityWebsiteDropdown2.value = 0;
        ownerInput.text = "";
        informationText.text = "";
        errorText.text = "";
    }

    public void validateName(string name) {
        if (name != "" && name.IndexOf(' ') > -1) {
            errorText.text = "Name cannot contain spaces.";
            // return false;
        } else {
            errorText.text = "";
            // return true;
        }
    }
}
