using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Proyecto26;
using Models;
using Utils;
using Cinemachine;


public class LoginController : MonoBehaviour
{
	public string bldgServer = "https://api.w2m.site";

    public GameObject baseResidentObject;

    public BldgController bldgController;


    // TODO is there a better place for the cameras?
    public CinemachineVirtualCamera flyCamera;
    public CinemachineVirtualCamera walkCamera;
    
	
    public Button signInButton;
    public TMP_InputField emailInputField;
    public TMP_Text errorDisplay;
    public TMP_Text verifyDisplay;


    private string basePath = "/v1/residents";

    // TODO move to shared constants/configuration file
	public float floorStartX = -8f;
	public float floorStartZ = -6f;

    private UnityAction onFlying;
    private UnityAction onWalking;

    private bool isPollingForVerificationStatus = false;
    private int pollInterval = 2000;
    private int verificationExpirationTime = 6*60*1000; // 6 minutess
    private DateTime lastPollTime = DateTime.Now;
    private DateTime loginStartTime = DateTime.Now;

    private string currentResidentEmail = null;
    private string currentResidentSessionId = null;


    // Start is called before the first frame update
    void Start()
    {
        // TODO disable movement

        signInButton.onClick.AddListener(SignInHandler);

    }

    void Update()
    {
        if (isPollingForVerificationStatus) {
            DateTime currentTime = DateTime.Now;
            double elapsedTime = currentTime.Subtract(lastPollTime).TotalMilliseconds;
            if (elapsedTime > pollInterval) {
                lastPollTime = DateTime.Now;
                pollForVerificationStatus();
            }
        }

    }

    private void Awake()
    {
        Debug.Log("LoginController Awake");
        onFlying = new UnityAction(OnFlying);
        onWalking = new UnityAction(OnWalking);
    }

    void OnEnable()
    {
        Debug.Log("LoginController On Enable");
        EventManager.StartListening("SwitchToFlying", onFlying);
        EventManager.StartListening("SwitchToWalking", onWalking);
    }

    void OnDisable()
    {
        Debug.Log("LoginController On Disable");
        //EventManager.StopListening("SwitchToFlying", onFlying);
        //EventManager.StopListening("SwitchToWalking", onWalking);
    }

    private void OnFlying()
    {
        Debug.Log("On Flying");
        flyCamera.gameObject.SetActive(true);
        walkCamera.gameObject.SetActive(false);
    }

    private void OnWalking()
    {
        Debug.Log("On Walking");
        walkCamera.gameObject.SetActive(true);
        flyCamera.gameObject.SetActive(false);
    }

    public void Show() {
        this.gameObject.SetActive(true);
    }


    void SignInHandler() {
        string email = emailInputField.text;
        Debug.Log("Signing in as " + email);
        errorDisplay.text = "";

        // disable the button
        toggleEnable(false);
        
        // TODO show spinner

        verifyDisplay.text = "Please click on the link in the email that was just sent to you";

        // call the login API
    	Debug.Log("Invoking resident Login API for resident " + email);
		string url = bldgServer + basePath + "/login";
		Debug.Log("url = " + url);
		// invoke login API
        RequestHelper req = RestUtils.createRequest("POST", url, new LoginRequest {email = email});
		RestClient.Post<LoginResponse>(req).Then(loginResponse => {

            currentResidentEmail = loginResponse.data.email;
            currentResidentSessionId = loginResponse.data.session_id;
            isPollingForVerificationStatus = true;
            lastPollTime = DateTime.Now;
            loginStartTime = DateTime.Now;
            	
		}).Catch(err => {
            Debug.Log(err.Message);

            errorDisplay.text = "Failed to sign in (" + err.Message + "), please check your email & try again";
            toggleEnable(true);
        });

        void toggleEnable(bool formEnabled) {
            if (!formEnabled) {
                signInButton.enabled = false;
                signInButton.interactable = false;
            } else {
                signInButton.enabled = true;
                signInButton.interactable = true;
            }
        }

    }


    void pollForVerificationStatus() {
        if (isPollingForVerificationStatus) {
            Debug.Log("Polling for verification status!");

            string url = bldgServer + basePath + "/verification_status?email=" + currentResidentEmail + "&session_id=" + currentResidentSessionId;
            Debug.Log("url = " + url);
            // invoke verification status API
            RequestHelper req = RestUtils.createRequest("GET", url);
            RestClient.Get<LoginResponse>(req).Then(loginResponse => {
                
                // If status is 200, meaning that the verification is successful:
                // - change the isPollingForVerificationStatus to false
                // - continue the login flow

                isPollingForVerificationStatus = false;
                Resident rsdt = loginResponse.data;
                Debug.Log("Login done, received " + rsdt.alias);

                Vector3 baseline = new Vector3(floorStartX, 0.5F, floorStartZ);	// WHY? if you set the correct Y, some images fail to display
                baseline.x += rsdt.x;
                baseline.z += rsdt.y;
                Debug.Log("Rendering current resident " + rsdt.alias + " at " + baseline.x + ", " + baseline.z);
                Quaternion qrt = Quaternion.identity;
                qrt.eulerAngles = new Vector3(0, rsdt.direction, 0);
                GameObject rsdtClone = (GameObject) Instantiate(baseResidentObject, baseline, qrt);
                walkCamera.Follow = rsdtClone.transform;
                walkCamera.LookAt = rsdtClone.transform;
                flyCamera.Follow = rsdtClone.transform;
                flyCamera.LookAt = rsdtClone.transform;
                ResidentController rsdtObject = rsdtClone.AddComponent<ResidentController>();
                rsdtObject.bldgServer = bldgServer;
                rsdtObject.initialize(rsdt, true);

                // once login result received, initialize player with resident details
                bldgController.bldgServer = bldgServer;
                bldgController.SetCurrentResident(rsdt);
                bldgController.SetCurrentResidentController(rsdtObject);
                bldgController.SetAddress("g");

                // hide the login dialog
                this.gameObject.SetActive(false);

                EventManager.TriggerEvent("LoginSuccessful");

            }).Catch(err => {
                Debug.Log("Emeil verification not yet done - " + err.Message);
                DateTime currentTime = DateTime.Now;
                double elapsedTime = currentTime.Subtract(loginStartTime).TotalMilliseconds;
                if (elapsedTime > verificationExpirationTime) {
                    verifyDisplay.text = "";
                    errorDisplay.text = "Login attempt expired. Please try again";
                    isPollingForVerificationStatus = false;
                }
            });

        }
    }
}
