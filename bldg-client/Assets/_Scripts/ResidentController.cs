using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;

public class ResidentController : MonoBehaviour
{

    private Resident resident;




    public void initialize(Resident model) {
        resident = model;
        Debug.Log("Initializing resident " + resident.alias + " at " + resident.location);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
