using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Networking;


namespace ImageUtils
{
    public class ImageController : MonoBehaviour
    {
        public GameObject _imageDisplay;
        Texture2D _texture;
        public string imageName = "assignee1_picture";


        void Start() {
            imageName = transform.name;
        }

        public void SetImageURL(string imageURL) {
            StartCoroutine(DownloadImage(imageURL, _imageDisplay.GetComponent<Renderer>().material));
        }


        IEnumerator DownloadImage(string mediaUrl, Material targetMaterial)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(mediaUrl);
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
                Debug.Log(request.error);
            else
                _texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                targetMaterial.mainTexture = _texture;
        }


        void OnDestroy () => Dispose();
        public void Dispose () => Object.Destroy(_texture);// memory released, leak otherwise

    }
}