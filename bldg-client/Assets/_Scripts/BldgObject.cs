using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Models;

public class BldgObject : MonoBehaviour, IPointerDownHandler
{

    public Bldg model;
    public BldgController bldgController;

    bool isRelocating = false;
    bool startedIndicatingRelocation = false;
    bool flashingIn = true; 
    public int redColor;
    public int greenColor;
    public int blueColor;

    public float ClickDuration = 1;
    bool clicking = false;
    float totalDownTime = 0;

    public string[] CHECKMARK_STATUSES = {"completed", "approved"};


    public void initialize(Bldg theModel, BldgController theController) {
        this.model = theModel;
        this.bldgController = theController;
        //Debug.Log("model " + this.model.name + " status is " + this.model.state);
        // if (!CHECKMARK_STATUSES.Contains(this.model.state)) {
        // 	Debug.Log("model " + this.model.name + " is NOT checked!");
        // 	string tag = "StatusCheck";
        // 	try {
        // 		// THIS doesn't work
        // 		List<Renderer> list = new List<Renderer>(this.gameObject.GetComponentsInChildren<Renderer>());
		//          for(int i = list.Count - 1; i >= 0; i--) 
		//          {
		//          	Debug.Log("Checking child " + i);
		//              if (list[i].CompareTag(tag) == true)
		//              {
		//                  list[i].gameObject.active = false;
        //          		 Debug.Log("Turned status off");
		//              }
		//          }
    	// 	} catch (Exception) {
    	// 		Debug.Log("Failed to find status check");
        // 	}
        // }
    }

    // Update is called once per frame
    void Update()
    {
        if (isRelocating) {
            this.gameObject.GetComponentInChildren<Renderer>().material.color = new Color32((byte)redColor, (byte)greenColor, (byte)blueColor, 255);
        }

        // Detect the first click
        if (Input.GetMouseButtonDown(0))
        {
            totalDownTime = 0;
            clicking = true;
        }

        // If a first click detected, and still clicking,
        // measure the total click time, and fire an event
        // if we exceed the duration specified
        if (clicking && Input.GetMouseButton(0))
        {
            totalDownTime += Time.deltaTime;

            if (totalDownTime >= ClickDuration)
            {
                Debug.Log("Long click detected");
                clicking = false;
                bldgController.handleLongClick (this, this.model, transform.position);
            }
        }

        // If a first click detected, and we release before the
        // duraction, do nothing, just cancel the click
        if (clicking && Input.GetMouseButtonUp(0))
        {
            clicking = false;
        }
    }

	void OnMouseDown() {
		Debug.Log ("Mouse down on bldg object!");
		bldgController.handleClick (this, this.model, transform.position);
	}

	public void OnPointerDown (PointerEventData eventData) {
		Debug.Log("OnPointerClick - bldg object");
         if (eventData.button == PointerEventData.InputButton.Right) {
             Debug.Log ("Right Mouse Button Clicked on bldg object");
			 bldgController.handleRightClick (this.model, transform.position);
         }
     }

	public void OnPointerUp(PointerEventData pointerEventData)
    {
        Debug.Log(name + "No longer being clicked");
    }

    public void indicateBeingRelocated() {
        Debug.Log("Hey! I'm relocating!!!");
        isRelocating = true;
        if (!startedIndicatingRelocation) {
            startedIndicatingRelocation = true;
            StartCoroutine(FlashObject());
        }
    }

    IEnumerator FlashObject() {
        while (isRelocating) {
            yield return new WaitForSeconds(0.05f);
            if (flashingIn) {
                if (blueColor <= 30) {
                    flashingIn = false;
                } else {
                    blueColor -= 25;
                    greenColor -= 10;
                }
            }
            if (!flashingIn) {
                if (blueColor >= 250) {
                    flashingIn = true;
                } else {
                    blueColor += 25;
                    greenColor += 10;
                }
            }
        }
    }


}
