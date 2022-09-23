using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEditor;
using UnityEngine.Events;
using Models;


public class BidiBldgDoorKnob : MonoBehaviour
{
    public string flr = "l0";   // jusf the flr name itself, not full flr path

    private string bldgName = "";
    private string bldgAddress = "";
    private string bldgURL = "";

    private Color initialColor;


    void getContainerBldgDetails()
    {
        initialColor = GetComponent<Renderer>().material.color;
        GameObject bldg = transform.parent.gameObject;
        BldgObject bldgObject = bldg.GetComponent<BldgObject>();
        if (bldgObject != null) {
            bldgName = bldgObject.model.name;
            bldgAddress = bldgObject.model.address;
            bldgURL = bldgObject.model.bldg_url;
        }
    }

    void OnMouseOver()
    {
        GetComponent<Renderer>().material.color = Color.cyan;
    }

    void OnMouseExit()
    {
        GetComponent<Renderer>().material.color = initialColor;
    }

    void OnMouseDown() {
        if (bldgName == "") {
            getContainerBldgDetails();
        }

        bool entering = true;
        // determine whether we're entering or exiting the bldg
        CurrentResidentController crc = CurrentResidentController.Instance;
        Debug.Log("~~~~~~~~~~ determining enter/exit: resident flr_url is: " + crc.resident.flr_url + " and bldg_url is " + bldgURL);
        if (crc.resident.flr_url == bldgURL + "/" + flr) {
            // we're already inside the parent bldg -> need to exit
            entering = false;
        }

        if (entering) {
            EventManager.Instance.TriggerEvent("EnteringBldg");
            Debug.Log("Sending enter bldg action for resident " +  crc.resident.email);
            crc.SendEnterBldgAction(new EnterBldgAction() {
                resident_email = crc.resident.email,
                action_type = "ENTER_BLDG",
                bldg_address = bldgAddress,
                bldg_url = bldgURL,
                flr = flr
            });
        } else {
            EventManager.Instance.TriggerEvent("ExitingBldg");
            Debug.Log("Sending exit bldg action for resident " +  crc.resident.email);
            crc.SendExitBldgAction(new ExitBldgAction() {
                resident_email = crc.resident.email,
                action_type = "EXIT_BLDG",
                bldg_address = bldgAddress,
                bldg_url = bldgURL
            });
        }
    }
}