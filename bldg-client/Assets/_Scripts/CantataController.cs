using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Networking;


public class CantataController : MonoBehaviour
{

    public string _imageUrl;
    public string _linkURL;

    public GameObject _imageDisplay;
    Texture2D _texture;

    // Start is called before the first frame update
    void Start()
    {
        //_texture = await GetRemoteTexture(_imageUrl);
        //_material.mainTexture = _texture;
        StartCoroutine(DownloadImage(_imageUrl, _imageDisplay.GetComponent<Renderer>().material));
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    IEnumerator DownloadImage(string mediaUrl, Material targetMaterial)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(mediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
            //this.gameObject.GetComponent<Renderer>().material.mainTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            targetMaterial.mainTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
    }


    //void OnDestroy () => Dispose();
    //public void Dispose () => Object.Destroy(_texture);// memory released, leak otherwise

}
