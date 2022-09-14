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
using UnityEngine.SceneManagement;
using Michsky.UI.Shift;


public class LoginController : MonoBehaviour
{

    [Header("Resources")]
    public GameObject baseResidentObject;

    public Button signInButton;
    public TMP_InputField emailInputField;
    public TMP_Text errorDisplay;
    public TMP_Text verifyDisplay;    

    private bool isPollingForVerificationStatus = false;
    private bool isSigningOnStarted = false;
    private int pollInterval = 2000;
    private int verificationExpirationTime = 6*60*1000; // 6 minutess
    private DateTime lastPollTime = DateTime.Now;
    private DateTime loginStartTime = DateTime.Now;

    private string currentResidentEmail = null;
    private string currentResidentSessionId = null;

    private Animator splashScreenAnimator;

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

    }

    void OnEnable()
    {
        Debug.Log("LoginController On Enable");
    }

    void OnDisable()
    {
        Debug.Log("LoginController On Disable");
        //EventManager.StopListening("SwitchToFlying", onFlying);
        //EventManager.StopListening("SwitchToWalking", onWalking);
    }

    public void Show() {
        this.gameObject.SetActive(true);
    }


    public void setAnimators(Animator sAnimator) {
        splashScreenAnimator = sAnimator;
    }


    private void animateOutOfLogin() {
        // ROLE 5   ///////////////////
        splashScreenAnimator.Play("Login to Loading");
        ////////////////////////////////
    }

    private void initCurrentResidentController(Resident rsdt) {
        // ROLE 1   //////////////////////
        // once login result received, initialize crc & player with resident details
        CurrentResidentController crc = CurrentResidentController.Instance;
        if (!crc.isInitialized()) {
            crc.initialize(rsdt);
        }
        //////////////////////////////////
    }

    private void fireLoginSuccessfulEvent() {
        // ROLE 2   //////////////////////////
        // Debug.Log("~~~~~ triggering LoginSuccessful");
        EventManager.Instance.TriggerEvent("LoginSuccessful");
        /////////////////////////////////////
    }

    public void completeLogin(Resident rsdt) {
        isPollingForVerificationStatus = false;
        // Debug.Log("~~~~~ Login done, received " + rsdt.alias);

        animateOutOfLogin();

        Debug.Log("~~~~~~ Received rsdt: " + rsdt);
        Debug.Log("~~~~~~ rsdt flr_url: " + rsdt.flr_url);
        

        initCurrentResidentController(rsdt);
    
        // hide the login dialog - TODO IS IT STILL NEEDED?
        this.gameObject.SetActive(false);

        fireLoginSuccessfulEvent();
    }

    public void SignInHandler() {
        if (isSigningOnStarted) return;
        isSigningOnStarted = true;
        string email = emailInputField.text;
        Debug.Log("Signing in as " + email + " " + DateTime.UtcNow);
        errorDisplay.text = "";

        // disable the button
        toggleEnable(false);
        
        // TODO show spinner

        // call the login API
    	Debug.Log("Invoking resident Login API for resident " + email);
        GlobalConfig conf = GlobalConfig.Instance;
		string url = conf.bldgServer + conf.residentsBasePath + "/login";
		Debug.Log("url = " + url);
		// invoke login API
        RequestHelper req = RestUtils.createRequest("POST", url, new LoginRequest {email = email});
		RestClient.Post<LoginResponse>(req).Then(loginResponse => {
            isSigningOnStarted = false;
            // TODO find a better way to determine whether the login was done
            if (loginResponse.data.alias != null && loginResponse.data.alias != "") {
                // there was already a valid session, so just complete the login
                // Debug.Log("~~~~ Email verification done recently, completing login");
                completeLogin(loginResponse.data);
            } else {
                // no valid session found, notify the user that they need to verify their email
                verifyDisplay.text = "Please click on the link in the email that was just sent to you";

                currentResidentEmail = loginResponse.data.email;
                currentResidentSessionId = loginResponse.data.session_id;
                isPollingForVerificationStatus = true;
                lastPollTime = DateTime.Now;
                loginStartTime = DateTime.Now;
            }	
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
            GlobalConfig conf = GlobalConfig.Instance;
            string url = conf.bldgServer + conf.residentsBasePath + "/verification_status?email=" + currentResidentEmail + "&session_id=" + currentResidentSessionId;
            Debug.Log("url = " + url);
            // invoke verification status API
            RequestHelper req = RestUtils.createRequest("GET", url);
            RestClient.Get<LoginResponse>(req).Then(loginResponse => {
                // If status is 200, meaning that the verification is successful:
                // - change the isPollingForVerificationStatus to false
                // - continue the login flow
                // Debug.Log("~~~~ Email verification done! completing login");
                completeLogin(loginResponse.data);
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
