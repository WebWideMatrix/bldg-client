using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using Models;
using Proyecto26;
using Utils;
using UnityEngine.SceneManagement;


[CreateAssetMenu(fileName = "Resident", menuName = "Current Resident", order = 0)]
public class CurrentResidentController : ScriptableObjectSingleton<CurrentResidentController>
{    
    public Resident resident;
    public string containerEntityType;

    public bool initialized = false;
    public DateTime lastLoginTime = DateTime.Today.AddYears(-1);

    public float ACTION_SEND_INTERVAL = 200;  // Milliseconds

    public bool inFlyingMode = false;
    public bool flyingHigh = false; 

    DateTime lastActionTime;


    public void initialize(Resident model, string _containerEntityType) {
        Debug.Log("CRC initialized with " + model.alias);
        resident = model;
        containerEntityType = _containerEntityType;
        Debug.Log("Initializing resident " + resident.alias + " at " + resident.location);
        initialized = true;
        lastLoginTime = DateTime.Now;
        lastActionTime = DateTime.Now;
        inFlyingMode = false;
        flyingHigh = false;
    }

    public bool isInitialized() {
        return initialized;
    }

    public void logout() {
        initialized = false;
    }

    public void Awake() {
        Debug.Log("CRC awoken");
    }

    private void OnEnable()
    {
        hideFlags = HideFlags.DontUnloadUnusedAsset;
    }
    
    public void setContainerEntityType(string entityType) {
        containerEntityType = entityType;
    }


    public void SendTurnAction(TurnAction action) {
        // call the act API
        Debug.Log("Invoking resident turn action for resident " + resident.email);
        GlobalConfig conf = GlobalConfig.Instance;
        string url = conf.bldgServer + conf.residentsBasePath + "/act";
        Debug.Log("url = " + url);
        // invoke act API
        RequestHelper req = RestUtils.createRequest("POST", url, action);
        RestClient.Post<ActionResponse>(req).Then(actionResponse => {
            Debug.Log("Action sent, received new direction");
            Debug.Log(actionResponse.data.direction);
            resident.direction = actionResponse.data.direction;
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
            GlobalConfig conf = GlobalConfig.Instance;
            string url = conf.bldgServer + conf.residentsBasePath + "/act";
            Debug.Log("url = " + url);
            // invoke act API
            RequestHelper req = RestUtils.createRequest("POST", url, action);
            RestClient.Post<ActionResponse>(req).Then(actionResponse => {
                Debug.Log("Action sent, received new location");
                Debug.Log(actionResponse.data.location);
                resident.location = actionResponse.data.location;
                resident.x = actionResponse.data.x;
                resident.y = actionResponse.data.y;
                resident.flr = actionResponse.data.flr;
                resident.flr_url = actionResponse.data.flr_url;
            });
        }
    }

    public void SendEnterBldgAction(EnterBldgAction action) {
        // call the act API
        GlobalConfig conf = GlobalConfig.Instance;
        string url = conf.bldgServer + conf.residentsBasePath + "/act";
        Debug.Log("url = " + url);
        // invoke act API
        RequestHelper req = RestUtils.createRequest("POST", url, action);
        RestClient.Post<ActionResponse>(req).Then(actionResponse => {
            Debug.Log("Action sent, received new location");
            Debug.Log(actionResponse.data.location);
            Debug.Log("Received resident location as " + actionResponse.data.x + ", " + actionResponse.data.y);
            resident.location = actionResponse.data.location;
            resident.x = actionResponse.data.x;
            resident.y = actionResponse.data.y;
            resident.flr = actionResponse.data.flr;
            resident.flr_url = actionResponse.data.flr_url;
            resident.nesting_depth = actionResponse.data.nesting_depth;
            
            EventManager.Instance.TriggerEvent("EnterBldgDone");

            // TODO cleanup ~~~
            // if (resident.flr != "g") {
            //     SceneManager.LoadScene("bldg_flr");
            // }

        }).Catch(err => {
            Debug.Log("Enter bldg action failed - " + err.Message);        
        });
    }

    public void SendExitBldgAction(ExitBldgAction action) {
        // call the act API
        GlobalConfig conf = GlobalConfig.Instance;
        string url = conf.bldgServer + conf.residentsBasePath + "/act";
        Debug.Log("url = " + url);
        // invoke act API
        RequestHelper req = RestUtils.createRequest("POST", url, action);
        RestClient.Post<ActionResponse>(req).Then(actionResponse => {
            Debug.Log("Action sent, received new location");
            Debug.Log(actionResponse.data.location);
            Debug.Log("Received resident location as " + actionResponse.data.x + ", " + actionResponse.data.y);
            resident.location = actionResponse.data.location;
            resident.x = actionResponse.data.x;
            resident.y = actionResponse.data.y;
            resident.flr = actionResponse.data.flr;
            resident.flr_url = actionResponse.data.flr_url;
            resident.nesting_depth = actionResponse.data.nesting_depth;
            EventManager.Instance.TriggerEvent("ExitBldgDone");

            // TODO cleanup ~~~~
            // Scene scene = SceneManager.GetActiveScene();
            // if (resident.flr == "g" && scene.name != "g") {
            //     SceneManager.LoadScene("g");
            // } else {
            //     SceneManager.LoadScene("bldg_flr");
            // }
        }).Catch(err => {
            Debug.Log("Exit bldg action failed - " + err.Message);        
        });
    }

    public void SendSayAction(SayAction action) {
        // Debug.Log("~~~~~~~~~~~~~~ Sending say action from " + action.resident_email + " and text: " + action.say_text);
        GlobalConfig conf = GlobalConfig.Instance;
        string url = conf.bldgServer + conf.residentsBasePath + "/act";
        Debug.Log("url = " + url);
        // invoke act API
        RequestHelper req = RestUtils.createRequest("POST", url, action);
        RestClient.Post<ActionResponse>(req).Then(actionResponse => {
            // Debug.Log("~~~~~~~ Say Action sent & response received");
            if (action.say_text.StartsWith("/promote") || action.say_text.StartsWith("/demote")) {
                // Debug.Log("~~~~~~~~~~~~~~~ this was a promote/demote command - sending event to reload container bldg");
                // need to reload the container bldg
                EventManager.Instance.TriggerEvent("PromoteOrDemote");
            }
        });
    }

}