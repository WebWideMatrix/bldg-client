using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitApp : MonoBehaviour
{

    public LoginController loginController;


    // Start is called before the first frame update
    void Start()
    {
        // check whether logged in already
        CurrentResidentController crc = CurrentResidentController.instance;
        if (!crc.isInitialized()) {
            Debug.Log("CRC not initialized????");
            loginController.Show();
        }
        else {
            Debug.Log("CRC Initialized!!!!");
            
            loginController.completeLogin(crc.resident);
        }
    }

}
