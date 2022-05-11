using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;


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
        if (EditorUtility.DisplayDialog ("Entering " + bldgName, "You're about to enter the " + bldgName + " team HQ.", "Ok", "Cancel")) {
            EventManager.TriggerEvent("EnteringBldg");
            bldgController.EnterBuildingByAddress(bldgAddress);
        }
    }
}
