using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "EventManager", menuName = "EventManager", order = 0)]
public class EventManager : ScriptableObjectSingleton<EventManager> {

    private Dictionary <string, UnityEvent> eventDictionary;

    void Init ()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, UnityEvent>();
        }
    }

    public void StartListening (string eventName, UnityAction listener)
    {
        Debug.Log(">>>>>>>>>>>>> Start listening to: " + eventName + " (" + listener + ")");

        Init();
        UnityEvent thisEvent = null;
        if (eventDictionary.TryGetValue (eventName, out thisEvent))
        {
            thisEvent.AddListener (listener);
        } 
        else
        {
            thisEvent = new UnityEvent ();
            thisEvent.AddListener (listener);
            eventDictionary.Add (eventName, thisEvent);
        }
    }

    public void StopListening (string eventName, UnityAction listener)
    {
        Debug.Log(">>>>>>>>>>>>> Stop listening to: " + eventName + " (" + listener + ")");

        Init();
        UnityEvent thisEvent = null;
        if (eventDictionary.TryGetValue (eventName, out thisEvent))
        {
            thisEvent.RemoveListener (listener);
        }
    }

    public void TriggerEvent (string eventName)
    {
        Debug.Log(">>>>>>>>>>>>> Trigger event: " + eventName);

        Init();
        UnityEvent thisEvent = null;
        if (eventDictionary.TryGetValue (eventName, out thisEvent))
        {
            thisEvent.Invoke ();
        }
    }
}