using System;
using UnityEngine;
using UnityEditor;


[CreateAssetMenu(fileName = "EntityMapping", menuName = "Entity prefab mapping", order = 0)]
public class EntityPrefabMapping : ScriptableSingleton<EntityPrefabMapping>
{   

    // SHAPES
    // TODO: change to array
	[SerializeField] public GameObject whiteboardBldg;
	[SerializeField] public GameObject presentationStandBldg;
	[SerializeField] public GameObject trafficSignBldg;
	[SerializeField] public GameObject streetSignBldg;
	
	[SerializeField] public GameObject chairBldg;
	[SerializeField] public GameObject laptopBldg;
	[SerializeField] public GameObject briefcaseBldg;
	[SerializeField] public GameObject tabletBldg;
	[SerializeField] public GameObject filingCabinetBldg;
	[SerializeField] public GameObject buildingWithStorefront;

	[SerializeField] public GameObject greenLotObject;
	[SerializeField] public GameObject blueLotObject;
	[SerializeField] public GameObject yellowLotObject;



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