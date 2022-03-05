using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Utils;
using ImageUtils;
using BrowserUtils;
using Models;
using Proyecto26;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Networking;


public class BldgController : MonoBehaviour
{

	public string bldgServer = "https://api.w2m.site";
	public string bldgsBasePath = "/v1/bldgs";
	public string residentsBasePath = "/v1/residents";
	public string roadsBasePath = "/v1/roads";

	public string DEFAULT_BLDG = "fromteal.app";


	public float floorStartX = -8f;
	public float floorStartZ = -6f;

	// SHAPES
    // TODO: change to array
	public GameObject whiteboardBldg;
	public GameObject presentationStandBldg;
	public GameObject trafficSignBldg;
	
	public GameObject chairBldg;
	public GameObject laptopBldg;
	public GameObject briefcaseBldg;
	public GameObject tabletBldg;
	public GameObject filingCabinetBldg;
	public GameObject buildingWithStorefront;

	public GameObject roadObject;

	public GameObject baseResidentObject;

	public GameObject contextMenu;

	public Texture2D relocateCursorTexture;

	public string targetURL;

	public BldgObject clickedObject;
    public Bldg clickedModel; 

	

	bool isRelocating = false;
	BldgObject relocatedObject; 
	bool isShowingContextMenu = false;
	bool isReloadingInLoop = false;

    // public ChaseCamera camera;

    string currentAddress;
	string currentFlr;

	Resident currentRsdt;
	BldgChatController bldgChatController;


    // Start is called before the first frame update
    void Start()
    {
    	// #if !UNITY_EDITOR && UNITY_WEBGL
		// 	WebGLInput.captureAllKeyboardInput = false;
		// #endif
        Debug.Log("Started");

		bldgChatController = gameObject.GetComponent<BldgChatController>();

        // if (currentAddress == null) {
			// TODO change to user's home bldg (received from login?)
			// SetAddress("g");

//			string lastAddress = PlayerPrefs.GetString ("currentAddress");
//			if (lastAddress != null && lastAddress != "") {
//				SetAddress(lastAddress);
//			} else {
        		// Debug.Log("Going to default bldg: " + DEFAULT_BLDG);
				// EnterBuilding(DEFAULT_BLDG);
//			}
		// }

    }

	public void SetCurrentResidentController(ResidentController rsdtController) {
		bldgChatController.SetResidentController(rsdtController);
	}

	public void SetCurrentResident(Resident rsdt) {
		currentRsdt = rsdt;
	}

	void showContextMenu() {
		isShowingContextMenu = true;
		contextMenu.gameObject.SetActive(true);
	}

	void hideContextMenu() {
		isShowingContextMenu = false;
		contextMenu.gameObject.SetActive(false);
	}

	public void handleFloorClick(Vector3 point) {
		Debug.Log("Floor click at " + point);
		if (isRelocating) {
			completeRelocatingBldg(point);
		} else if (!isShowingContextMenu) {
			// camera.moveToStart();
			contextMenu.gameObject.SetActive(false);
			clickedModel = null;
			clickedObject = null;
		}
	}


    public void handleClick(BldgObject bldgObject, Bldg bldgModel, Vector3 position) {
        //camera.moveToTarget(position);
		if (clickedModel != bldgModel) {
			Debug.Log("Clicked on different object: " + clickedModel.name);
			clickedObject = bldgObject;
			clickedModel = bldgModel;
			hideContextMenu();
			targetURL = null;
		}
    }

    public void handleLongClick(BldgObject bldgObject, Bldg bldgModel, Vector3 position) {
        Debug.Log("long click on: " + bldgModel.name);
		//showContextMenu();
    }

    public void handleRightClick(Bldg model, Vector3 position) {
        Debug.Log("right click on: " + model.name);
    }

	public void browse() {
		if (targetURL != null) {
			if (!(targetURL.StartsWith("http://") || targetURL.StartsWith("https://"))) {
				targetURL = "https://" + targetURL;
			}
			Debug.Log("Browsing to: " + targetURL);
			Application.OpenURL (targetURL);
			targetURL = null;
		} else if (clickedModel != null && clickedModel.web_url != null) {
			targetURL = clickedModel.web_url;
		}
	}

	public void startRelocating() {
		Debug.Log("Starting to relocate");
		isRelocating = true;
		relocatedObject = clickedObject;
		relocatedObject.indicateBeingRelocated();
		hideContextMenu();
		// camera.moveToStart();
		Cursor.SetCursor(relocateCursorTexture, new Vector2(30, 60), CursorMode.Auto);
	}


	public void closeMenu() {
		hideContextMenu();
	}

	string generateNewAddress(string oldAddress, int newX, int newY) {
		// generate new address based on new coordinates
		Debug.Log("Old address is: " + oldAddress);
		string[] addressParts = oldAddress.Split(AddressUtils.DELIM_CHAR);
		string[] newAddressParts = new string[addressParts.Length];
		for (int i = 0; i < addressParts.Length - 1; i++) {
			newAddressParts[i] = addressParts[i];
		}
		newAddressParts[newAddressParts.Length - 1] = "b(" + newX + "," + newY + ")";
		return string.Join(AddressUtils.DELIM, newAddressParts);
	}

	public void completeRelocatingBldg(Vector3 point) {
		// figure out new coordinates
		int newX = (int)(point.x - floorStartX);
		int newY = (int)(point.z - floorStartZ);
		Debug.Log("Invoking relocate API to move bldg " + clickedModel.name + ", from (" + clickedModel.x + ", " + clickedModel.y + ") to (" + newX + ", " + newY + ")");
		string newAddress = generateNewAddress(clickedModel.address, newX, newY);
		string url = bldgServer + bldgsBasePath + "/" + clickedModel.address + "/relocate_to/" + newAddress;
		Debug.Log("url = " + url);
		// invoke relocate API
		RestClient.DefaultRequestHeaders["Authorization"] = "Bearer ...";
		RestClient.Post(url, null).Then(res => {
			Debug.Log("Relocation done");
			isRelocating = false;
			relocatedObject = null;
			clickedModel = null;
			clickedObject = null;
			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
			// fetch all bldgs again
			this.AddressChanged();	// TODO remove, once receiving updates from server
		});
	}

	// public void GoToBldg(string address) {
	// 	currentAddress = address;
	// 	if (AddressUtils.isBldg(address)) {
	// 		currentAddress = AddressUtils.generateInsideAddress (address);
	// 	}
	// 	AddressChanged ();
	// }

	// public void GoIn() {
	// 	InputField input = GameObject.FindObjectOfType<InputField> ();
	// 	currentAddress = input.text;
	// 	AddressChanged ();
	// }


	// public void GoOut() {
	// 	InputField input = GameObject.FindObjectOfType<InputField> ();

	// 	currentAddress = AddressUtils.getContainerFlr(input.text);

	// 	AddressChanged ();
	// }


	// public void GoUp() {
	// 	InputField input = GameObject.FindObjectOfType<InputField> ();
	// 	currentAddress = input.text;
	// 	int level = AddressUtils.getFlrLevel(currentAddress);
	// 	currentAddress = AddressUtils.replaceFlrLevel (currentAddress, level + 1);
	// 	AddressChanged ();
	// }


	// public void GoDown() {
	// 	InputField input = GameObject.FindObjectOfType<InputField> ();
	// 	currentAddress = input.text;
	// 	int level = AddressUtils.getFlrLevel(currentAddress);
	// 	if (level > 0) {
	// 		currentAddress = AddressUtils.replaceFlrLevel (currentAddress, level - 1);
	// 	}
	// 	AddressChanged ();
	// }


	public void SetAddress(string address) {
		Debug.Log("SetAddress -> " + address);
		currentAddress = address;
		AddressChanged();
	}

	public void EnterBuilding(string web_url) {
		Debug.Log("EnterBuilding -> " + web_url);
		// lookup the address for that web_url
		// We can add default request headers for all requests
		RestClient.DefaultRequestHeaders["Authorization"] = "Bearer ...";
 
        Debug.Log("Resolvin bldg for web_url: " + web_url);
		string address = null;
		string url = bldgServer + bldgsBasePath + "/resolve_address?web_url=" + UnityWebRequest.EscapeURL(web_url);
		Debug.Log(url);
		RestClient.Get(url).Then(res =>
			{
				Debug.Log(res);
				address = res.Text;
				Debug.Log("Resolve address was successful: " + address);
				address = address + AddressUtils.DELIM + "l0";	// TODO add floor only if really needed
				SetAddress(address);

			}).Catch(err => {
				Debug.Log(err.Message);
				Debug.Log("Failed to resolve address for: " + web_url);
			});
	}


	public void AddressChanged() {
		Debug.Log ("Address changed to: " + currentAddress);
		// InputField input = GameObject.FindObjectOfType<InputField> ();
		// if (input.text != currentAddress) {
		// 	input.text = currentAddress;
		// }

		PlayerPrefs.SetString ("currentAddress", currentAddress);

		// TODO validate address

		currentFlr = AddressUtils.extractFlr(currentAddress);

		// TODO check whether it changed

		// TODO DECIDE WHETHER WE NEED DIFFERENT SCENES FOR G & FLR
		
		// check whether we need to switch scene
		// if (currentAddress.ToLower () == "g" && SceneManager.GetActiveScene().name == "Floor") {
		// 	SceneManager.LoadScene ("Ground");
		// 	return;
		// }
		// if (currentAddress.ToLower () != "g" && SceneManager.GetActiveScene().name == "Ground") {
		// 	SceneManager.LoadScene ("Floor");
		// 	return;
		// }

		// load the new address
		Reload();
	}


	public void Reload() {
		Debug.Log("Reload invoked");
		// TODO compare new content with existing to decide whether a reload is really needed
		switchAddress (currentAddress);
		if (!isReloadingInLoop) {
			Debug.Log("Starting reload loop");
			isReloadingInLoop = true;
		}
		Invoke("Reload", 5.0f);
	}

	void switchAddress(string address) {


		if (address.ToLower() != "g") {
			updateFloorSign ();
		}

		reloadBuildings(address);
		reloadResidents(address);
		reloadRoads(address);
	}


	void reloadBuildings(string address) {
		GameObject[] currentFlrBuildings = GameObject.FindGameObjectsWithTag("Building");
		foreach (GameObject bldg in currentFlrBuildings) {
			GameObject.Destroy (bldg);
		}

		// We can add default request headers for all requests
		RestClient.DefaultRequestHeaders["Authorization"] = "Bearer ...";
        string url = bldgServer + bldgsBasePath + "/look/" + address;
		// Debug.Log("Loading buildings from: " + url);
		RestClient.GetArray<Bldg>(url).Then(res =>
			{
				int count = 0;
				foreach (Bldg b in res) {
					count += 1;
					// Debug.Log("processing bldg " + count);
					
					// // The area is 16x12, going from (8,6) - (-8,-6)

					Vector3 baseline = new Vector3(floorStartX, 0F, floorStartZ);	// WHY? if you set the correct Y, some images fail to display
					baseline.x += b.x;
					baseline.z += b.y;
					GameObject prefab = getPrefabByEntityClass(b.entity_type);
					GameObject bldgClone = (GameObject) Instantiate(prefab, baseline, Quaternion.identity);
					bldgClone.tag = "Building";
                    BldgObject bldgObject = bldgClone.AddComponent<BldgObject>();
					bldgObject.initialize(b, this);
					Debug.Log(b.summary);
					TMP_Text[] labels = bldgClone.GetComponentsInChildren<TMP_Text>();
					foreach (TMP_Text label in labels) {
						if (label.name == "summary")
							label.text = b.summary;
						else if (label.name == "summary_top") {
							label.text = b.summary;
						}
						else if (label.name == "entity_type")
							label.text = b.entity_type;
						else if (label.name == "name")
							label.text = b.name;
						else if (label.name == "name2")
							label.text = b.name;		
						else if (label.name == "name_top")
							label.text = b.name;
						else if (label.name == "state")
							label.text = b.state;
					}
					ImageController[] imageDisplays = bldgClone.GetComponentsInChildren<ImageController>();
					foreach (ImageController imgDisplay in imageDisplays) {
						imgDisplay.SetImageURL(b.picture_url);
					}
					LinkController[] linkObjects = bldgClone.GetComponentsInChildren<LinkController>();
					foreach (LinkController linkObj in linkObjects) {
						linkObj.SetLinkURL(b.web_url);
					}
					//Debug.Log("About to call renderAuthorPicture on bldg " + count);
                    // TODO create picture element
					// controller.renderMainPicture();
				}
				// Debug.Log("Rendered " + count + " bldgs");
			});
	}

	void reloadResidents(string address) {
		GameObject[] currentFlrResidents = GameObject.FindGameObjectsWithTag("Resident");
		foreach (GameObject rsdnt in currentFlrResidents) {
			GameObject.Destroy (rsdnt);
		}

		// We can add default request headers for all requests
		RestClient.DefaultRequestHeaders["Authorization"] = "Bearer ...";
        string url = bldgServer + residentsBasePath + "/look/" + address;
		// Debug.Log("Loading residents from: " + url);
		bool clearedChatHistory = false;
		RestClient.GetArray<Resident>(url).Then(result =>
			{
				int count = 0;
				foreach (Resident r in result) {
					count += 1;
					// Debug.Log("processing resident " + count);

					if (!clearedChatHistory) {
						bldgChatController.ClearMessageHistory();
						clearedChatHistory = true;
					}

					if (r.previous_messages.Length > 0) {
						bldgChatController.AddHistoricMessages(r.alias, r.previous_messages);
					}

					// if it's the current user, skip
					if (r.alias == currentRsdt.alias) continue;

					// // The area is 16x12, going from (8,6) - (-8,-6)

					Vector3 baseline = new Vector3(floorStartX, 0.5F, floorStartZ);	// WHY? if you set the correct Y, some images fail to display
					baseline.x += r.x;
					baseline.z += r.y;
					// Debug.Log("Rendering resident " + r.alias + " at " + baseline.x + ", " + baseline.z);
					Debug.Log("Changing direction of " + r.alias + " to " + r.direction);
					Quaternion qrt = Quaternion.identity;
					qrt.eulerAngles = new Vector3(0, r.direction, 0);
					GameObject rsdtClone = (GameObject) Instantiate(baseResidentObject, baseline, qrt);
					rsdtClone.tag = "Resident";
                    ResidentController rsdtObject = rsdtClone.AddComponent<ResidentController>();
					rsdtObject.initialize(r);
					// Debug.Log(r.alias);
					//Debug.Log("About to call renderAuthorPicture on bldg " + count);
                    // TODO create picture element
					// controller.renderMainPicture();
				};
				// Debug.Log("Rendered " + count + " bldgs");
			});
	}

	void reloadRoads(string address) {
		GameObject[] currentFlrRoads = GameObject.FindGameObjectsWithTag("Road");
		foreach (GameObject road in currentFlrRoads) {
			GameObject.Destroy (road);
		}

		// We can add default request headers for all requests
		RestClient.DefaultRequestHeaders["Authorization"] = "Bearer ...";
        string url = bldgServer + roadsBasePath + "/look/" + address;
		Debug.Log("Loading roads from: " + url);
		RestClient.GetArray<Road>(url).Then(res =>
			{
				Debug.Log("Got response for look roads");
				int count = 0;
				foreach (Road r in res) {
					count += 1;
					renderRoad(r.from_x, r.from_y, r.to_x, r.to_y);
				}
				Debug.Log("Rendered " + count + " roads");
			});
	}


	void renderRoad(int from_x, int from_y, int to_x, int to_y)
	{	
		int d_x = 0;
		if (to_x != from_x) {
			d_x = to_x - from_x;
		}
		int d_y = 0;
		if (to_y != from_y) {
			d_y = to_y - from_y;
		}
		
		// if straight line, draw 1 segment
		if (d_x == 0 || d_y == 0) {
			renderRoadSegment(from_x, from_y, d_x, d_y);
		}
		// else break to 2 segments
		else {
			int mid_x = from_x + d_x;
			int mid_y = from_y;

			if (from_x > to_x) {
				from_x = mid_x;
				d_x = -1 * d_x;
			}

			if (from_y > to_y) {
				mid_y = mid_y + d_y;
				d_y = -1 * d_y;
			}
			renderRoadSegment(from_x, from_y, d_x, 0);
			renderRoadSegment(mid_x, mid_y, 0, d_y);
		}
	}

	void renderRoadSegment(int from_x, int from_y, int d_x, int d_y) 
	{
		Vector3 baseline = new Vector3(floorStartX, 0.01F, floorStartZ);
		float default_road_scale = 10.01F;
		baseline.x += from_x;
		baseline.z += from_y;
		GameObject roadClone = (GameObject) Instantiate(roadObject, baseline, Quaternion.identity);
		roadClone.transform.Translate((d_x / 2), 0, (d_y / 2));
		roadClone.transform.localScale += new Vector3(d_x / default_road_scale, 0, d_y / default_road_scale);
		roadClone.tag = "Road";
	}

	GameObject getPrefabByEntityClass(string entity_type) {
		switch (entity_type) {
		case "purpose":
			return whiteboardBldg;
		case "cantata":
			return presentationStandBldg;
		case "neighborhood":
			return trafficSignBldg;
		case "member":
			return laptopBldg;
		case "milestone":
			return briefcaseBldg;
		case "web_page":
			return tabletBldg;
		case "team":
			return buildingWithStorefront;
		default:
			return chairBldg;
		}
	}

	void updateFloorSign() {
		TextMesh flrSign = GameObject.FindGameObjectWithTag ("FloorSign").GetComponent<TextMesh>();
		flrSign.text = currentFlr.ToUpper ();
	}

}
