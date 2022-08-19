using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.Events;
using Michsky.UI.Shift;

public class InitApp : MonoBehaviour
{

    [Header("Resources")]
    public PressKeyEvent quickActionsHotkey;


    // TODO is there a better place for the cameras?
    public CinemachineVirtualCamera flyCamera;
    public CinemachineVirtualCamera walkCamera;
    

    private UnityAction onFlying;
    private UnityAction onWalking;
    private UnityAction onLogin;
	

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

    private void OnLogin()
    {
        Debug.Log("~~~~~~ On Login");
        Debug.Log("~~~~~~ [before] QuickActions Key is active? " + quickActionsHotkey.gameObject.active);
        quickActionsHotkey.gameObject.SetActive(true);
        Debug.Log("~~~~~~ [after] QuickActions Key is active? " + quickActionsHotkey.gameObject.active);
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

}
