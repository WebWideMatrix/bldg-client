using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitApp : MonoBehaviour
{

    public LoginController loginController;


    // Start is called before the first frame update
    void Start()
    {
        loginController.Show();
    }

}
