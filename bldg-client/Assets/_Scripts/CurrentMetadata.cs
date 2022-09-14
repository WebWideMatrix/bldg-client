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

    private Dictionary<string, List<string>> entityNamesPerType = new Dictionary<string, List<string>>();


    public List<string> getEntityTypes() {
        return entityTypes;
    }

    public void clearEntities() {
        entityCountPerType.Clear();
        entityNamesPerType.Clear();
    }


    //
    // IMPORTANT: this must be cleared when switching floors - names are only unique within floors!
    //
    public void addEntity(string type, string name) {
        if (!entityCountPerType.ContainsKey(type)) {
            entityCountPerType[type] = 0;
        }
        entityCountPerType[type]++;
        if (!entityNamesPerType.ContainsKey(type)) {
            entityNamesPerType[type] = new List<string>();
        }
        if (!entityNamesPerType[type].Contains(name)) {
            entityNamesPerType[type].Add(name);
        }
    }

    public List<string> getEntitiesPerType(string type) {
        if (!entityNamesPerType.ContainsKey(type)) {
            return new List<string>();
        }
        return entityNamesPerType[type];
    }

    public int getEntitiesCount() {
        int count = 0;
        foreach (string type in entityCountPerType.Keys) {
            count += entityCountPerType[type];
        }
        return count;
    }

    public List<string> getAllEntities() {
        List<string> entities = new List<string>();
        foreach (string type in entityNamesPerType.Keys) {
            foreach (string name in entityNamesPerType[type]) {
                entities.Add("[" + type + "] " + name);
            }
        }
        return entities;
    }
}