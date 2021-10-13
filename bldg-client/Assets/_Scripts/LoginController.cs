using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Proyecto26;
using Models;

public class LoginController : MonoBehaviour
{
	public string bldgServer = "https://api.w2m.site";

    public ResidentController residentController;

    private string basePath = "/v1/residents";
	
    public Button signInButton;
    public TMP_InputField emailInputField;

    // Start is called before the first frame update
    void Start()
    {
        // TODO disable movement

        signInButton.onClick.AddListener(SignInHandler);

    }

    // Update is called once per frame
    void Update()
    {
        // TODO validate the email address, indicate in color & disable the login button until valid
    }


    void SignInHandler() {
        string email = emailInputField.text;
        Debug.Log("Signing in as " + email);

        // disable the button
        signInButton.enabled = false;
        signInButton.interactable = false;
        
        // TODO show spinner

        // call the login API
    	Debug.Log("Invoking resident Login API for resident " + email);
		string url = bldgServer + basePath + "/login";
		Debug.Log("url = " + url);
		// invoke login API
		RestClient.DefaultRequestHeaders["Authorization"] = "Bearer ...";
		RestClient.Post<LoginResponse>(url, new LoginRequest {
            email = email
        }).Then(loginResponse => {
			Debug.Log("Login done, received ");
            Debug.Log(loginResponse.data.alias);
            
            // once login result received, initialize player with resident details
            residentController.initialize(loginResponse.data);

            	
		}).Catch(err => {
            Debug.Log(err.Message);

            // TODO show login error
        });

    }
}
