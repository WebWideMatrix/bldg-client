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


    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float rotateSpeed = 100f;

    float prevX = 0;
    float prevZ = 0;

    bool isCurrentUser = false;


    public void initialize(Resident model) {
        initialize(model, false);
    }

    public void initialize(Resident model, bool isCurrent) {
        resident = model;
        Debug.Log("Initializing resident " + resident.alias + " at " + resident.location);
        initialized = true;
        isCurrentUser = isCurrent;
    }



    // Start is called before the first frame update
    void Start()
    {
        alias = this.gameObject.GetComponentInChildren<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (initialized && alias.text != resident.alias) {
            alias.text = resident.alias;
        }

        if (isCurrentUser) {
            // control movement
            float xValue =  Input.GetAxis("Horizontal") * Time.deltaTime * rotateSpeed;
            float zValue =  Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
            transform.Translate(0, 0, zValue);
            transform.Rotate(0, xValue, 0);
            if (xValue != prevX || zValue != prevZ) {
                //Debug.Log("Moved " + xValue + ", " + zValue);
                prevX = xValue;
                prevZ = zValue;
            }
        }
    }
}
