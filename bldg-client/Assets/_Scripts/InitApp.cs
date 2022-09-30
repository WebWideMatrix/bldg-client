using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.Events;
using TMPro;
using Michsky.UI.Shift;
using Models;
using Utils;
using ImageUtils;


public class InitApp : MonoBehaviour
{

    [Header("Resources")]
    public GameObject baseResidentObject;
    public BldgController bldgController;

    public TMP_Text residentName;
    public TMP_Text residentName2;
    public TMP_Text currentAddress;

    public ModalWindowManager quickActionsDialog;

    public TimedEvent startTimedEvent;
    public Animator splashScreenAnimator;


    // TODO is there a better place for the cameras?
    public CinemachineVirtualCamera flyCamera;
    public CinemachineVirtualCamera walkCamera;
    

    private UnityAction onFlying;
    private UnityAction onWalking;
    private UnityAction onLogin;
    private UnityAction onQuickActions;
    private UnityAction onPromoteOrDemote;
    private UnityAction onEnterBldgDone;
    private UnityAction onExitBldgDone;
	
    // TODO move to shared constants/configuration file
	public float floorStartX = -8f;
	public float floorStartZ = -6f;

    private GameObject currentResidentObject;


    private void startLoadingAnimation() {
        startTimedEvent.StartIEnumerator();
    }

    private void initCurrentResidentUI(Resident rsdt) {
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
        currentResidentObject = (GameObject) Instantiate(baseResidentObject, baseline, qrt);
        float aliceFactor = AddressUtils.calcAliceFactor(rsdt.nesting_depth);
        if (aliceFactor != 1) {
            Vector3 currScale = currentResidentObject.transform.localScale ;
            currentResidentObject.transform.localScale = currScale * aliceFactor;
        }

        walkCamera.Follow = currentResidentObject.transform;
        walkCamera.LookAt = currentResidentObject.transform;
        flyCamera.Follow = currentResidentObject.transform;
        flyCamera.LookAt = currentResidentObject.transform;
        // TODO change cameras distance & screen size based on Alice Factor

        ResidentController rsdtObject = currentResidentObject.AddComponent<ResidentController>();
        rsdtObject.initialize(rsdt, true);

        // RETURN: replace all of these with event handling on bldg controller
        bldgController.SetCurrentResidentAlias(rsdt.alias);
        bldgController.SetCurrentResidentController(rsdtObject);
    }

    private void loadBldgs(Resident rsdt) {
        bldgController.SetAddress(rsdt.flr);
    }

    private void loadContainerContainerFlr(string address) {
        bldgController.reloadContainerContainerFlr(address);
    }

    private void setLabelsInUI(Resident rsdt) {
        residentName.text = rsdt.alias;
        residentName2.text = rsdt.alias;
        currentAddress.text = rsdt.flr_url;
    }

    private bool loadBldgSceneIfNeeded() {
        // check whether we need to load the bldg_flr scene
        CurrentResidentController crc = CurrentResidentController.Instance;
        if (crc.resident.flr != "g") {
            Scene scene = SceneManager.GetActiveScene();
            if (scene.name != "bldg_flr") {
                SceneManager.LoadScene("bldg_flr");
                return true;
            }
        }
        return false;
    }


    private void animateOutOfLogin() {
        Scene scene = SceneManager.GetActiveScene();
        try {
            splashScreenAnimator.Play("Login to Loading");
        } catch (Exception e) {
			Debug.Log("~~~~~~ Failed to animate loading: splashScreenAnimator is `" + splashScreenAnimator + "` " + e.ToString());
		}
    }

    void OnEnable() {
        onFlying = new UnityAction(OnFlying);
        onWalking = new UnityAction(OnWalking);
        onLogin = new UnityAction(OnLogin);
        onQuickActions = new UnityAction(OnQuickActions);
        onPromoteOrDemote = new UnityAction(OnPromoteOrDemote);
        onEnterBldgDone = new UnityAction(OnEnterBldgDone);
        onExitBldgDone = new UnityAction(OnExitBldgDone);
        EventManager.Instance.StartListening("SwitchToFlying", onFlying);
        EventManager.Instance.StartListening("SwitchToWalking", onWalking);
        EventManager.Instance.StartListening("LoginSuccessful", onLogin);
        EventManager.Instance.StartListening("OpenQuickActions", onQuickActions);
        EventManager.Instance.StartListening("PromoteOrDemote", onPromoteOrDemote);
        EventManager.Instance.StartListening("EnterBldgDone", onEnterBldgDone);
        EventManager.Instance.StartListening("ExitBldgDone", onExitBldgDone);
    }


    void Awake() {    
        CurrentResidentController crc = CurrentResidentController.Instance;
        if (crc.isInitialized()) {
            Debug.Log("~~~~~ *********************   Init App - Awake - Resident initializied  *********************");

            animateOutOfLogin();

            startLoadingAnimation();

            initCurrentResidentUI(crc.resident);

            loadBldgs(crc.resident);

            Debug.Log("~~~~~~~~~~ Awake - calling setLabelsInUI..");
            setLabelsInUI(crc.resident);
        } else {
            Debug.Log("~~~~~ *********************   Init App - Awake - Resident NOT YET INITIALIZED  *********************");
        }

    }




    private void OnLogin()
    {
        Debug.Log("~~~~~ *********************   Init App - On Login  *********************");

        startLoadingAnimation();
        
        CurrentResidentController crc = CurrentResidentController.Instance;
        if (!crc.isInitialized()) {
            Debug.LogError("This cannot happen - OnLogin called but current resident isn't initialized yet");
            return;
        }

        initCurrentResidentUI(crc.resident);

        Debug.Log("~~~~~~~~~~ loading bldgs");
        try {
            loadBldgs(crc.resident);
            loadContainerContainerFlr(crc.resident.flr);
        } catch (Exception e) {
            Debug.Log("Failed to load bldgs on Login:");
            Debug.LogError(e.ToString());
        }
        
        Debug.Log("~~~~~~~~~~ OnLogin - calling setLabelsInUI..");
        setLabelsInUI(crc.resident);
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

    private void OnQuickActions() {
        quickActionsDialog.ModalWindowIn();
    }

    private void OnPromoteOrDemote() {
        bldgController.reloadContainerBldg();
    }

    private void OnEnterBldgDone() {
        Debug.Log("~~~~~~~~ OnEnterBldgDone");
        CurrentResidentController crc = CurrentResidentController.Instance;

        // scale resident avatar by nesting depth & locate it based on new flr
        Destroy(currentResidentObject);
        initCurrentResidentUI(crc.resident);
        loadBldgs(crc.resident);
        setLabelsInUI(crc.resident);
    }

    private void OnExitBldgDone() {
        Debug.Log("~~~~~~~~ OnExitBldgDone");
        CurrentResidentController crc = CurrentResidentController.Instance;

        // scale resident avatar by nesting depth & locate it based on new flr
        Destroy(currentResidentObject);
        initCurrentResidentUI(crc.resident);
        loadBldgs(crc.resident);
        setLabelsInUI(crc.resident);
    }


    public static void startWalking()
    {
        EventManager.Instance.TriggerEvent("StartWalking");
    }

    public static void startFlyingLow()
    {
        EventManager.Instance.TriggerEvent("StartFlyingLow");
    }

    public static void startFlyingHigh()
    {
        EventManager.Instance.TriggerEvent("StartFlyingHigh");
    }

    public void setAnimators(Animator sAnimator, TimedEvent stEvent) {
        startTimedEvent = stEvent;
        splashScreenAnimator = sAnimator;
    }
}
