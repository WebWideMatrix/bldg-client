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

    public GameObject baseResidentObject;

    public BldgController bldgController;

    public Button signInButton;
    public TMP_InputField emailInputField;
    public TMP_Text errorDisplay;
    public TMP_Text verifyDisplay;


    // TODO move to shared constants/configuration file
	public float floorStartX = -8f;
	public float floorStartZ = -6f;

    public CinemachineVirtualCamera flyCamera;
    public CinemachineVirtualCamera walkCamera;


    private bool isPollingForVerificationStatus = false;
    private int pollInterval = 2000;
    private int verificationExpirationTime = 6*60*1000; // 6 minutess
    private DateTime lastPollTime = DateTime.Now;
    private DateTime loginStartTime = DateTime.Now;

    private string currentResidentEmail = null;
    private string currentResidentSessionId = null;

    private Animator splashScreenAnimator;
    private Animator mainAnimator;
    private TimedEvent startTimedEvent;


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


    public void setAnimators(Animator sAnimator, Animator mAnimator, TimedEvent stEvent) {
        splashScreenAnimator = sAnimator;
        mainAnimator = mAnimator;
        startTimedEvent = stEvent;
    }

    public void completeLogin(Resident rsdt) {
        isPollingForVerificationStatus = false;
        Debug.Log("Login done, received " + rsdt.alias);
        splashScreenAnimator.Play("Login to Loading");
        startTimedEvent.StartIEnumerator();

        // once login result received, initialize crc & player with resident details
        CurrentResidentController crc = CurrentResidentController.Instance;
        if (!crc.isInitialized()) {
            crc.initialize(rsdt);
        }

        // check whether we need to load the bldg_flr scene
        if (crc.resident.flr != "g") {
            Scene scene = SceneManager.GetActiveScene();
            if (scene.name != "bldg_flr") {
                SceneManager.LoadScene("bldg_flr");
            }
        }

        float height = 0.5F;
        if (rsdt.flr != "g") {
            height = 2.5F;  // bldg is larger when inside a bldg, so floor is higher
        }
        Vector3 baseline = new Vector3(floorStartX, height, floorStartZ);	// WHY? if you set the correct Y, some images fail to display
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
        rsdtObject.initialize(rsdt, true);


        bldgController.SetCurrentResident(rsdt);
        bldgController.SetCurrentResidentController(rsdtObject);
        bldgController.SetAddress(rsdt.flr);
        

        // hide the login dialog
        this.gameObject.SetActive(false);

        EventManager.Instance.TriggerEvent("LoginSuccessful");
    }


    public void SignInHandler() {
        string email = emailInputField.text;
        Debug.Log("Signing in as " + email);
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
            // TODO find a better way to determine whether the login was done
            if (loginResponse.data.alias != null && loginResponse.data.alias != "") {
                // there was already a valid session, so just complete the login
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
