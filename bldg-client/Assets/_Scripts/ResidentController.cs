using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;
using TMPro;

public class ResidentController : MonoBehaviour
{

    private Resident resident;
    private TMP_Text alias;
    private bool initialized = false;




    public void initialize(Resident model) {
        resident = model;
        Debug.Log("Initializing resident " + resident.alias + " at " + resident.location);
        initialized = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        alias = this.gameObject.GetComponentInChildren<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (initialized) {
            alias.text = resident.alias;
        }
    }
}
