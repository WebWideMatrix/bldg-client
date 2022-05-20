using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using Models;
using Utils;


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
            string containerFlr = AddressUtils.getContainerFlr(bldgAddress);
            bldgController.SetAddress(containerFlr);
            Debug.Log("Invoking exit bldg action");
            CurrentResidentController crc = CurrentResidentController.instance;
            Debug.Log("Sending exit bldg action for resident " +  crc.resident.email);
            crc.SendExitBldgAction(new ExitBldgAction() {
                resident_email = crc.resident.email,
                action_type = "EXIT_BLDG",
                bldg_address = bldgAddress
            });
        }
    }
}