using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BldgDoorKnob : MonoBehaviour
{

    private string bldgName = "";
    private string bldgAddress = "";

    // Start is called before the first frame update
    void Start()
    {
        GameObject bldg = transform.parent.gameObject;
        BldgObject bldgObject = bldg.GetComponent<BldgObject>();
        bldgName = bldgObject.model.name;
        bldgAddress = bldgObject.model.address;
    }

    void OnMouseDown() {
        EditorUtility.DisplayDialog ("Entering " + bldgName, "You're about to enter the " + bldgName + " team HQ.", "Ok", "Cancel");
    }
}
