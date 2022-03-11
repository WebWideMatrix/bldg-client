using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Models;

public class RoadObject : MonoBehaviour
{

    public Road model;


    public void initialize(Road theModel) {
        this.model = theModel;
    }

}
