using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using Models;


public class InternalBldgDoorKnob : MonoBehaviour
{

    private string bldgName = "";
    private string bldgAddress = "";
    private BldgController bldgController;

    void getContainerBldgDetails()
    {
        GameObject bldg = transform.parent.gameObject;
        BldgObject bldgObject = bldg.GetComponent<BldgObject>();
        if (bldgObject != null) {
            bldgName = bldgObject.model.name;
            bldgAddress = bldgObject.model.address;
            bldgController = bldgObject.bldgController;
        }
    }

    void OnMouseDown() {
        if (bldgName == "") {
            getContainerBldgDetails();
        }
        
        if (EditorUtility.DisplayDialog ("Exiting " + bldgName, "You're about to exit the " + bldgName + " team HQ.", "Ok", "Cancel")) {
            EventManager.TriggerEvent("ExitingBldg");
            Debug.Log("Setting address");
            // TODO extract the parent bldg address
            bldgController.SetAddress(bldgAddress + "/l0");
            Debug.Log("Invoking enter bldg action");
            CurrentResidentController crc = CurrentResidentController.instance;
            Debug.Log("Sending enter bldg action for resident " +  crc.resident.email);
            crc.SendEnterBldgAction(new EnterBldgAction() {
                resident_email = crc.resident.email,
                action_type = "ENTER_BLDG",
                bldg_address = bldgAddress
            });
        }
    }
}