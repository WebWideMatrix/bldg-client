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


    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float rotateSpeed = 100f;
    [SerializeField] float ACTION_SEND_INTERVAL = 200;  // Milliseconds

    float prevX = 0;
    float prevZ = 0;

    int exactDirection = -1;
    int previousDirection = -1;

    int directionInterval = 45;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (initialized && alias.text != resident.alias) {
            alias.text = resident.alias;
        }

        if (isCurrentUser) {
            if (exactDirection == -1) {
                // initialize the values
                exactDirection = resident.direction;
                previousDirection = resident.direction;
            }
            
            // control movement
            float xValue =  Input.GetAxis("Horizontal") * Time.deltaTime * rotateSpeed;
            float zValue =  Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
            transform.Translate(0, 0, zValue);
            transform.Rotate(0, xValue, 0);
            bool isRotationChange = xValue != prevX;
            bool isMovementChange = zValue != prevZ;
            // check whether resident turned to the sides
            if (isRotationChange) {
                prevX = xValue;
                Debug.Log("======================================");
                int directionDelta = (int)(xValue * 10);
                Debug.Log("directionDelta = " + directionDelta);
                exactDirection += (int)directionDelta + 360;
                exactDirection = exactDirection % 360;                
                Debug.Log("exactDirection = " + exactDirection);

                
                Debug.Log("exactDirection after modulus = " + exactDirection);

                int newDirection = exactDirection - (exactDirection % directionInterval);
                Debug.Log("newDirection = " + newDirection);

                if (newDirection != previousDirection) {
                    Debug.Log("Sending turn action");
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
                prevX = xValue;
                prevZ = zValue;
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
        RestClient.DefaultRequestHeaders["Authorization"] = "Bearer ...";
        // TODO change to ActionResponse
        RestClient.Post<LoginResponse>(url, action).Then(loginResponse => {
            Debug.Log("Action sent, received new location");
            Debug.Log(loginResponse.data.location);
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
            RestClient.DefaultRequestHeaders["Authorization"] = "Bearer ...";
            // TODO change to ActionResponse
            RestClient.Post<LoginResponse>(url, action).Then(loginResponse => {
                Debug.Log("Action sent, received new location");
                Debug.Log(loginResponse.data.location);
            });
        }
    }


    public void SendSayAction(SayAction action) {
        Debug.Log("Sending say action from " + action.resident_email);
        string url = bldgServer + baseResidentsPath + "/act";
        Debug.Log("url = " + url);
        // invoke act API
        RestClient.DefaultRequestHeaders["Authorization"] = "Bearer ...";
        // TODO change to ActionResponse
        RestClient.Post<LoginResponse>(url, action).Then(loginResponse => {
            Debug.Log("Action sent");
        });
    }
}
