using System;
using UnityEngine;
using UnityEditor;


[CreateAssetMenu(fileName = "EntityMapping", menuName = "Entity prefab mapping", order = 0)]
public class EntityPrefabMapping : ScriptableObjectSingleton<EntityPrefabMapping>
{   

    // SHAPES
    // TODO: change to array
	public GameObject whiteboardBldg;
	public GameObject presentationStandBldg;
	public GameObject trafficSignBldg;
	public GameObject streetSignBldg;
	
	public GameObject chairBldg;
	public GameObject laptopBldg;
	public GameObject briefcaseBldg;
	public GameObject tabletBldg;
	public GameObject filingCabinetBldg;
	public GameObject buildingWithStorefront;

	public GameObject greenLotObject;
	public GameObject blueLotObject;
	public GameObject yellowLotObject;


    private void OnEnable()
    {
        hideFlags = HideFlags.DontUnloadUnusedAsset;
    }

	public GameObject getPrefabByEntityClass(string entity_type) {
		Debug.Log("Getting prefab for entity class " + entity_type);
        
        switch (entity_type) {
		case "purpose":
			return whiteboardBldg;
		case "cantata":
			return presentationStandBldg;
		case "neighborhood":
			return trafficSignBldg;
		case "street":
			return streetSignBldg;
		case "member":
			return laptopBldg;
		case "milestone":
			return briefcaseBldg;
		case "web_page":
			return tabletBldg;
		case "team":
			return buildingWithStorefront;
		case "lot":
			return greenLotObject;
		case "green-lot":
			return greenLotObject;
		case "blue-lot":
			return blueLotObject;
		case "yellow-lot":
			return yellowLotObject;
		default:
			return chairBldg;
		}
	}



}