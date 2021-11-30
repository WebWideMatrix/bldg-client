using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace BrowserUtils
{
    public class LinkController : MonoBehaviour
    {


        public GameObject linkObject;

        string _linkURL;

        // Update is called once per frame
        void Update()
        {
            // Check for mouse input
            if (Input.GetMouseButton(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                // Casts the ray and get the first game object hit
                Physics.Raycast(ray, out hit);
                Debug.Log("This hit at " + hit.point );
                if (hit.collider.gameObject == linkObject) {
                    Debug.Log("Opening link");
                    OpenLink();
                }
            }
        }

        public void SetLinkURL(string url) {
            _linkURL = url;
            if (!(_linkURL.StartsWith("http://") || _linkURL.StartsWith("https://"))) {
				_linkURL = "https://" + _linkURL;
			}
        }


        void OpenLink() {
            if (_linkURL != null && _linkURL != "") {
                Debug.Log("Browsing to: " + _linkURL);
                Application.OpenURL (_linkURL);
            }
            else {
                Debug.Log("Warning: clicked on link object but not given a link URL to open");
            }
        }
    }
}