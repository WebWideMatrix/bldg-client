using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;

    float prevX = 0;
    float prevZ = 0;


    void Update()
    {
        float xValue =  Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        float zValue =  Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
        transform.Translate(xValue, 0, zValue);
        if (xValue != prevX || zValue != prevZ) {
            Debug.Log("Moved " + xValue + ", " + zValue);
            prevX = xValue;
            prevZ = zValue;
        }
    }
}
