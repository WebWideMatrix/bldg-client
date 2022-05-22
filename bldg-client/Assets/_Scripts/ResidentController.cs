using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Utils;
using Models;


public class ResidentController : MonoBehaviour
{

    public Resident resident;

    private TMP_Text alias;
    private bool initialized = false;

    bool isCurrentUser = false;

    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float rotateSpeed = 100f;

    [SerializeField] float FLY_HEIGHT = 80F;
    [SerializeField] float INDOOR_FLY_HEIGHT = 10F;
    
    [SerializeField] float GROUND_HEIGHT = 0.5F;
    [SerializeField] float INDOOR_GROUND_HEIGHT = 2F;

    float prevX = 0;
    float prevZ = 0;

    int exactDirection = -1;
    int previousDirection = -1;

    int directionInterval = 20;

    // TODO move to shared constants/configuration file
	public float floorStartX = -8f;
	public float floorStartZ = -6f;


    public void initialize(Resident model) {
        initialize(model, false);
    }

    public void initialize(Resident model, bool isCurrent) {
        resident = model;
        Debug.Log("Initializing resident " + resident.alias + " at " + resident.location);
        initialized = true;
        isCurrentUser = isCurrent;
    }

    float getFlyHeight(CurrentResidentController crc) {
        if (crc.resident.flr == "g")
            return FLY_HEIGHT;
        else
            return INDOOR_FLY_HEIGHT;
    }

    float getGroundHeight(CurrentResidentController crc) {
        if (crc.resident.flr == "g")
            return GROUND_HEIGHT;
        else
            return INDOOR_GROUND_HEIGHT;
    }

    void handleMovement() {
        CurrentResidentController currentResident = CurrentResidentController.Instance; // TODO only get it when you need it
        if (Input.GetKey("f") && !currentResident.inFlyingMode) {
            // clicking f while on the land: start flying
            currentResident.inFlyingMode = true;
            currentResident.flyingHigh = false;
            Debug.Log("Fly mode");
            float h = getFlyHeight(currentResident);
            transform.position = new Vector3(transform.position.x, h, transform.position.z);
            //transform.Rotate(90, 0, 0);
            EventManager.Instance.TriggerEvent("SwitchToFlying");
        }
        else if (Input.GetKey("f") && currentResident.flyingHigh) {
            // clicking f while flying high: fly lower
            currentResident.flyingHigh = false;
            float h = getFlyHeight(currentResident);
            transform.position = new Vector3(transform.position.x, h, transform.position.z);
        }

        if (Input.GetKey("h") && !currentResident.inFlyingMode) {
            // clicking h while on the land: fly high
            currentResident.inFlyingMode = true;
            Debug.Log("High fly mode");
            float h = getFlyHeight(currentResident);
            transform.position = new Vector3(transform.position.x, h * 3.5F, transform.position.z);
            //transform.Rotate(90, 0, 0);
            EventManager.Instance.TriggerEvent("SwitchToFlying");
        }
        else if (Input.GetKey("h") && !currentResident.flyingHigh) {
            // clicking h while flying low: fly higher
            currentResident.flyingHigh = true;
            float h = getFlyHeight(currentResident);
            transform.position = new Vector3(transform.position.x, h * 3.5F, transform.position.z);
        }
        
        if (Input.GetKey("l") && currentResident.inFlyingMode) {
            // clicking l while flying: land on the ground
            currentResident.inFlyingMode = false;
            currentResident.flyingHigh = false;
            Debug.Log("Walking mode");
            float h = getGroundHeight(currentResident);
            transform.position = new Vector3(transform.position.x, h, transform.position.z);
            //transform.Rotate(-90, 0, 0);
            EventManager.Instance.TriggerEvent("SwitchToWalking");
        }

        // control movement
        float xValue =  Input.GetAxis("Horizontal") * Time.deltaTime * rotateSpeed;
        float zValue =  Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
        bool isRotationChange = xValue != prevX;
        bool isMovementChange = zValue != prevZ;

        if (!(isRotationChange || isMovementChange)) {
            return;
        }
        prevX = xValue;
        prevZ = zValue;

        transform.Translate(0, 0, zValue);
        transform.Rotate(0, xValue, 0);

        //Debug.Log("Current rotation: " + transform.eulerAngles.y);
        exactDirection = (int)transform.eulerAngles.y;


        // check whether resident turned to the sides
        if (isRotationChange) {
            
            //Debug.Log("======================================");
            //Debug.Log("exactDirection = " + exactDirection);

            int newDirection = exactDirection - (exactDirection % directionInterval);
            //Debug.Log("newDirection = " + newDirection);

            if (newDirection != previousDirection) {
                previousDirection = newDirection;
                //Debug.Log("Sending turn action for " + resident.alias + " ^^^^^^^^^^^^^^^^^^^^^^^ " + newDirection);
                currentResident.SendTurnAction(new TurnAction {
                    resident_email = resident.email,
                    action_type = "TURN",
                    turn_direction = newDirection
                });
                resident.direction = newDirection;
            }
        }

        // check whether resident moved 
        if (isMovementChange) {
            //Debug.Log("Moved " + xValue + ", " + zValue);
            // Send action to bldg server
            // TODO calculate new location
            int moveX = (int)(transform.position.x - floorStartX);
            int moveY = (int)(transform.position.z - floorStartZ);
            string moveLocation = AddressUtils.updateLocation(resident.location, moveX, moveY);
            currentResident.SendMoveAction(new MoveAction {
                resident_email = resident.email,
                action_type = "MOVE",
                move_location = moveLocation,
                move_x = moveX,
                move_y = moveY
            });
            resident.location = moveLocation;
            resident.x = moveX;
            resident.y = moveY;
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        alias = this.gameObject.GetComponentInChildren<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (initialized && alias.text != resident.alias) {
            alias.text = resident.alias;
        }

        if (isCurrentUser) {
            handleMovement();
        }
    }

}
