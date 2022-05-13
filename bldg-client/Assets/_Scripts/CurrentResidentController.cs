using System;
using UnityEngine;
using UnityEditor;
using Models;
using Proyecto26;
using Utils;


[CreateAssetMenu(fileName = "Resident", menuName = "Current Resident", order = 0)]
public class CurrentResidentController : ScriptableSingleton<CurrentResidentController>
{    
    public string bldgServer = "https://api.w2m.site";
    private string baseResidentsPath = "/v1/residents";

    public Resident resident;

    private bool initialized = false;

    [SerializeField] float ACTION_SEND_INTERVAL = 200;  // Milliseconds

    public bool inFlyingMode = false;
    public bool flyingHigh = false; 

    DateTime lastActionTime;


    public void initialize(Resident model) {
        resident = model;
        Debug.Log("Initializing resident " + resident.alias + " at " + resident.location);
        initialized = true;
        lastActionTime = DateTime.Now;
        inFlyingMode = false;
        flyingHigh = false;
    }

    public void OnAwake() {

    }
    


    public void SendTurnAction(TurnAction action) {
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



    public void SendMoveAction(MoveAction action) {
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

    public void SendEnterBldgAction(EnterBldgAction action) {
        // call the act API
        Debug.Log("Invoking resident enter bldg action for resident " + resident.email);
        string url = bldgServer + baseResidentsPath + "/act";
        Debug.Log("url = " + url);
        // invoke act API
        RequestHelper req = RestUtils.createRequest("POST", url, action);
        RestClient.Post<ActionResponse>(req).Then(actionResponse => {
            Debug.Log("Action sent, received new location");
            Debug.Log(actionResponse.data.location);
        });
    }

    public void SendExitBldgAction(EnterBldgAction action) {
        // call the act API
        Debug.Log("Invoking resident exit bldg action for resident " + resident.email);
        string url = bldgServer + baseResidentsPath + "/act";
        Debug.Log("url = " + url);
        // invoke act API
        RequestHelper req = RestUtils.createRequest("POST", url, action);
        RestClient.Post<ActionResponse>(req).Then(actionResponse => {
            Debug.Log("Action sent, received new location");
            Debug.Log(actionResponse.data.location);
        });
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