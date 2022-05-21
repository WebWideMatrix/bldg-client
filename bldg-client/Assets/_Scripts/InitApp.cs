using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.Events;


public class InitApp : MonoBehaviour
{

    public LoginController loginController;

    // TODO is there a better place for the cameras?
    public CinemachineVirtualCamera flyCamera;
    public CinemachineVirtualCamera walkCamera;
    

    private UnityAction onFlying;
    private UnityAction onWalking;
	


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("*********************   Init App   *********************");
        // check whether logged in already
        CurrentResidentController crc = CurrentResidentController.Instance;
        if (!crc.isInitialized()) {
            Debug.Log("CRC not initialized");
            loginController.Show();
        }
        else {
            Debug.Log("CRC Initialized!!!!");
            loginController.completeLogin(crc.resident);
        }
        onFlying = new UnityAction(OnFlying);
        onWalking = new UnityAction(OnWalking);
        EventManager.Instance.StartListening("SwitchToFlying", onFlying);
        EventManager.Instance.StartListening("SwitchToWalking", onWalking);
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

}
