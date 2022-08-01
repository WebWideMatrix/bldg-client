using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Models;
using Proyecto26;
using Utils;
using UnityEngine.SceneManagement;


[CreateAssetMenu(fileName = "Metadata", menuName = "Current Metadata", order = 0)]
public class CurrentMetadata : ScriptableObjectSingleton<CurrentMetadata>
{   
    public string groundLevelName;

    private List<string> entityTypes = new List<string>() {
        "purpose",
        "cantata",
        "neighborhood",
        "street",
        "member",
        "milestone",
        "task",
        "web_page",
        "team",
        "lot",
        "green-lot",
        "blue-lot",
        "yellow-lot"
    };
    
    private Dictionary<string, int> entityCountPerType = new Dictionary<string, int>();

    private Dictionary<string, List<string>> entityWebsitesPerType = new Dictionary<string, List<string>>();


    public List<string> getEntityTypes() {
        return entityTypes;
    }

    public void clearEntities() {
        entityCountPerType.Clear();
        entityWebsitesPerType.Clear();
    }

    public void addEntity(string type, string website) {
        if (!entityCountPerType.ContainsKey(type)) {
            entityCountPerType[type] = 0;
        }
        entityCountPerType[type]++;
        if (!entityWebsitesPerType.ContainsKey(type)) {
            entityWebsitesPerType[type] = new List<string>();
        }
        if (!entityWebsitesPerType[type].Contains(website)) {
            entityWebsitesPerType[type].Add(website);
        }
    }

    public List<string> getEntitiesPerType(string type) {
        if (!entityWebsitesPerType.ContainsKey(type)) {
            return new List<string>();
        }
        return entityWebsitesPerType[type];
    }
}