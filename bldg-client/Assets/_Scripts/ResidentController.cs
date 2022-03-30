using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;
using TMPro;
using Proyecto26;
using Utils;


public class ResidentController : MonoBehaviour
{
    public string bldgServer = "https://api.w2m.site";
    private string baseResidentsPath = "/v1/residents";

    public Resident resident;
    private TMP_Text alias;
    private bool initialized = false;

    private bool inFlyingMode = false;
    private bool flyingHigh = false; 


    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float rotateSpeed = 100f;
    [SerializeField] float ACTION_SEND_INTERVAL = 200;  // Milliseconds

    [SerializeField] int FLY_HEIGHT = 80;

    float prevX = 0;
    float prevZ = 0;

    int exactDirection = -1;
    int previousDirection = -1;

    int directionInterval = 20;

    bool isCurrentUser = false;
    DateTime lastActionTime;


    // TODO move to shared constants/configuration file
	public float floorStartX = -8f;
	public float floorStartZ = -6f;


    public void initialize(Resident model) {
        initialize(model, false);
        lastActionTime = DateTime.Now;
    }

    public void initialize(Resident model, bool isCurrent) {
        resident = model;
        Debug.Log("Initializing resident " + resident.alias + " at " + resident.location);
        initialized = true;
        isCurrentUser = isCurrent;
    }



    // Start is called before the first frame update
    void Start()
    {
        alias = this.gameObject.GetComponentInChildren<TMP_Text>();
        inFlyingMode = false;
        flyingHigh = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (initialized && alias.text != resident.alias) {
            alias.text = resident.alias;
        }

        if (isCurrentUser) {

            if (Input.GetKey("f") && !inFlyingMode) {
                // clicking f while on the land: start flying
                inFlyingMode = true;
                flyingHigh = false;
                Debug.Log("Fly mode");
                transform.position = new Vector3(transform.position.x, FLY_HEIGHT, transform.position.z);
                //transform.Rotate(90, 0, 0);
                EventManager.TriggerEvent("SwitchToFlying");
            }
            else if (Input.GetKey("f") && flyingHigh) {
                // clicking f while flying high: fly lower
                flyingHigh = false;
                transform.position = new Vector3(transform.position.x, FLY_HEIGHT, transform.position.z);
            }

            if (Input.GetKey("h") && !inFlyingMode) {
                // clicking h while on the land: fly high
                inFlyingMode = true;
                Debug.Log("High fly mode");
                transform.position = new Vector3(transform.position.x, FLY_HEIGHT * 3.5F, transform.position.z);
                //transform.Rotate(90, 0, 0);
                EventManager.TriggerEvent("SwitchToFlying");
            }
            else if (Input.GetKey("h") && !flyingHigh) {
                // clicking h while flying low: fly higher
                flyingHigh = true;
                transform.position = new Vector3(transform.position.x, FLY_HEIGHT * 3.5F, transform.position.z);
            }
            
            if (Input.GetKey("l") && inFlyingMode) {
                // clicking l while flying: land on the ground
                inFlyingMode = false;
                flyingHigh = false;
                Debug.Log("Walking mode");
                transform.position = new Vector3(transform.position.x, 0.5F, transform.position.z);
                //transform.Rotate(-90, 0, 0);
                EventManager.TriggerEvent("SwitchToWalking");
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
                    //Debug.Log("Sending turn action for " + resident.alias + " ^^^^^^^^^^^^^^^^^^^^^^^ " + newDirection);
                    SendTurnAction(new TurnAction {
                        resident_email = resident.email,
                        action_type = "TURN",
                        turn_direction = newDirection
                    });
                    resident.direction = newDirection;
                }
            }

            // check whether residemt moved 
            if (isMovementChange) {
                //Debug.Log("Moved " + xValue + ", " + zValue);
                // Send action to bldg server
                // TODO calculate new location
                int moveX = (int)(transform.position.x - floorStartX);
                int moveY = (int)(transform.position.z - floorStartZ);
                string moveLocation = AddressUtils.updateLocation(resident.location, moveX, moveY);
                SendMoveAction(new MoveAction {
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
    }


    void SendTurnAction(TurnAction action) {
        // call the act API
        Debug.Log("Invoking resident turn action for resident " + resident.email);
        string url = bldgServer + baseResidentsPath + "/act";
        Debug.Log("url = " + url);
        // invoke act API
        RequestHelper req = RestUtils.createRequest("POST", url, action);
        RestClient.Post<ActionResponse>(req).Then(actionResponse => {
            Debug.Log("Action sent, received new location");
            Debug.Log(actionResponse.data.location);
        });
    }



    void SendMoveAction(MoveAction action) {
        DateTime currentTime = DateTime.Now;
        float timeSinceLastActionSend = currentTime.Subtract(lastActionTime).Milliseconds;  
        // don't send action in frequency smaller than the configured interval
        if (timeSinceLastActionSend < ACTION_SEND_INTERVAL) {
            // Skipping action sending due to short interval
            // TODO make sure to invoke the same update after the minimal interval has passed
        }
        else {
            // call the act API
            Debug.Log("Invoking resident move action for resident " + resident.email);
            lastActionTime = DateTime.Now;
            string url = bldgServer + baseResidentsPath + "/act";
            Debug.Log("url = " + url);
            // invoke act API
            RequestHelper req = RestUtils.createRequest("POST", url, action);
            RestClient.Post<ActionResponse>(req).Then(actionResponse => {
                Debug.Log("Action sent, received new location");
                Debug.Log(actionResponse.data.location);
            });
        }
    }


    public void SendSayAction(SayAction action) {
        Debug.Log("Sending say action from " + action.resident_email);
        string url = bldgServer + baseResidentsPath + "/act";
        Debug.Log("url = " + url);
        // invoke act API
        RequestHelper req = RestUtils.createRequest("POST", url, action);
        RestClient.Post<ActionResponse>(req).Then(actionResponse => {
            Debug.Log("Action sent");
        });
    }
}
