using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.Events;
using TMPro;
using Michsky.UI.Shift;
using Models;

public class InitApp : MonoBehaviour
{

    [Header("Resources")]
    public PressKeyEvent quickActionsHotkey;
    public GameObject baseResidentObject;
    public BldgController bldgController;

    public TMP_Text residentName;
    public TMP_Text residentName2;
    public TMP_Text currentAddress;


    // TODO is there a better place for the cameras?
    public CinemachineVirtualCamera flyCamera;
    public CinemachineVirtualCamera walkCamera;
    

    private UnityAction onFlying;
    private UnityAction onWalking;
    private UnityAction onLogin;

    private TimedEvent startTimedEvent;
	
    // TODO move to shared constants/configuration file
	public float floorStartX = -8f;
	public float floorStartZ = -6f;


    private void startLoadingAnimation() {
        // ROLE 9   ////////////////////
        startTimedEvent.StartIEnumerator();
        ////////////////////////////////
    }

    private void initCurrentResidentUI(Resident rsdt) {
        // ROLE 6   ///////////////////////
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

        // RETURN: replace all of these with event handling on bldg controller
        bldgController.SetCurrentResident(rsdt);
        bldgController.SetCurrentResidentController(rsdtObject);
        /////////////////////////////////////
    }

    private void loadBldgs(Resident rsdt) {
        // ROLE 7   /////////////////////////
        bldgController.SetAddress(rsdt.flr);        
        /////////////////////////////////////
    }

    private void setLabelsInUI(Resident rsdt) {
        // ROLE 8  /////////////////////
        residentName.text = rsdt.alias;
        residentName2.text = rsdt.alias;
        currentAddress.text = rsdt.flr;
        ////////////////////////////////
    }

    private bool loadBldgSceneIfNeeded() {
        // ROLE 4   //////////////////////
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
        ///////////////////////////////////
    }


    void OnEnable() {
        Debug.Log("*********************   Init App - On Enable  *********************");

        // check whether logged in already
        // CurrentResidentController crc = CurrentResidentController.Instance;
        // if (!crc.isInitialized()) {
        //     Debug.Log("CRC not initializede");
        //     loginController.Show();
        // }
        // else {
        //     loginController.completeLogin(crc.resident);
        // }

        onFlying = new UnityAction(OnFlying);
        onWalking = new UnityAction(OnWalking);
        onLogin = new UnityAction(OnLogin);
        EventManager.Instance.StartListening("SwitchToFlying", onFlying);
        EventManager.Instance.StartListening("SwitchToWalking", onWalking);
        EventManager.Instance.StartListening("LoginSuccessful", onLogin);
    
        CurrentResidentController crc = CurrentResidentController.Instance;
        if (crc.isInitialized()) {
            startLoadingAnimation();

            initCurrentResidentUI(crc.resident);

            loadBldgs(crc.resident);

            setLabelsInUI(crc.resident);
        }
    }




    private void OnLogin()
    {
        Debug.Log("~~~~~~ On Login");

        startLoadingAnimation();
        
        CurrentResidentController crc = CurrentResidentController.Instance;
        if (!crc.isInitialized()) {
            Debug.LogError("This cannot happen - OnLogin called but current resident isn't initialized yet");
            return;
        }

        if (loadBldgSceneIfNeeded()) return;

        initCurrentResidentUI(crc.resident);

        loadBldgs(crc.resident);

        setLabelsInUI(crc.resident);

        Debug.Log("~~~~~~ [before] QuickActions Key is active? " + quickActionsHotkey.gameObject.active);
        quickActionsHotkey.gameObject.SetActive(true);
        Debug.Log("~~~~~~ [after] QuickActions Key is active? " + quickActionsHotkey.gameObject.active);
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

    public void setAnimators(TimedEvent stEvent) {
        startTimedEvent = stEvent;
        Debug.Log("~~~~~~~~~ got TimesEvent!!!");
    }
}
