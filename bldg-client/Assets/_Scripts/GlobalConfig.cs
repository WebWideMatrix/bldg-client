using System;
using UnityEngine;
using UnityEditor;
using Models;
using Proyecto26;
using Utils;


[CreateAssetMenu(fileName = "Config", menuName = "General Config", order = 1)]
public class GlobalConfig : ScriptableSingleton<GlobalConfig>
{   

    [SerializeField] public string bldgServer = "https://api.w2m.site";
    [SerializeField] public string residentsBasePath = "/v1/residents";
	[SerializeField] public string bldgsBasePath = "/v1/bldgs";
	[SerializeField] public string roadsBasePath = "/v1/roads";

}