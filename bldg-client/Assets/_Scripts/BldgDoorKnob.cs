using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using Models;


public class BldgDoorKnob : MonoBehaviour
{

    private string bldgName = "";
    private string bldgAddress = "";
    private BldgController bldgController;

    // Start is called before the first frame update
    void Start()
    {
        GameObject bldg = transform.parent.gameObject;
        BldgObject bldgObject = bldg.GetComponent<BldgObject>();
        bldgName = bldgObject.model.name;
        bldgAddress = bldgObject.model.address;
        bldgController = bldgObject.bldgController;
    }

    void OnMouseDown() {
        if (EditorUtility.DisplayDialog ("Entering " + bldgName, "You're about to enter the " + bldgName + " team HQ. Please note that due to the Alice effect, everything is 10x smaller inside buildings.", "Ok", "Cancel")) {
            EventManager.TriggerEvent("EnteringBldg");
            bldgController.SetAddress(bldgAddress);
            CurrentResidentController crc = CurrentResidentController.instance;
            crc.SendEnterBldgAction(new EnterBldgAction() {
                resident_email = crc.resident.email,
                action_type = "ENTER_BLDG",
                bldg_address = bldgAddress
            });
        }
    }
}