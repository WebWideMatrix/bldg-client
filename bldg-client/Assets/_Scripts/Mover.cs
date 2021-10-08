using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float rotateSpeed = 100f;

    float prevX = 0;
    float prevZ = 0;


    void Update()
    {
        float xValue =  Input.GetAxis("Horizontal") * Time.deltaTime * rotateSpeed;
        float zValue =  Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
        transform.Translate(0, 0, zValue);
        transform.Rotate(0, xValue, 0);
        if (xValue != prevX || zValue != prevZ) {
            Debug.Log("Moved " + xValue + ", " + zValue);
            prevX = xValue;
            prevZ = zValue;
        }
    }
}
