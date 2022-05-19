using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class InitApp : MonoBehaviour
{

    public LoginController loginController;


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("*********************   Init App   *********************");
        // check whether logged in already
        CurrentResidentController crc = CurrentResidentController.instance;
        if (!crc.isInitialized()) {
            Debug.Log("CRC not initialized");
            loginController.Show();
        }
        else {
            Debug.Log("CRC Initialized!!!!");
            // check whether we need to load the bldg_flr scene
            if (crc.resident.flr != "g") {
                Scene scene = SceneManager.GetActiveScene();
                if (scene.name != "bldg_flr") {
                    SceneManager.LoadScene("bldg_flr");
                }
            }
            loginController.completeLogin(crc.resident);
        }
    }

}
