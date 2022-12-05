using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Fusion.Frameworks.Net
{
    public class HttpManager : MonoBehaviour
    {
        private static HttpManager instance = null;

        public static HttpManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject managerObject = new GameObject("HttpManager");
                    instance = managerObject.AddComponent<HttpManager>();
                    DontDestroyOnLoad(managerObject);
                }
                return instance;
            }
        }

        public void Post(string url, string content, Action<string> callback)
        {
            StartCoroutine(PostCoroutine(url, content, callback));
        }

        private IEnumerator PostCoroutine(string url, string content, Action<string> callback)
        {
            UnityWebRequest request = new UnityWebRequest(url, "POST");
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(content));
            request.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
            request.downloadHandler = new DownloadHandlerBuffer();
            UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = request.SendWebRequest();
            yield return unityWebRequestAsyncOperation;
            if (!(request.result == UnityWebRequest.Result.ConnectionError) && !(request.result == UnityWebRequest.Result.ProtocolError))
            {
                if (callback != null)
                {
                    callback(request.downloadHandler.text);
                }
            } else
            {
                Debug.LogError("Post Error");
            }
        }

        public void Get(string url, Action<string> callback)
        {
            StartCoroutine(GetCoroutine(url, callback));
        }

        private IEnumerator GetCoroutine(string url, Action<string> callback)
        {
            UnityWebRequest request = new UnityWebRequest(url, "GET");
            request.downloadHandler = new DownloadHandlerBuffer();
            UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = request.SendWebRequest();
            yield return unityWebRequestAsyncOperation;
            if (!(request.result == UnityWebRequest.Result.ConnectionError) && !(request.result == UnityWebRequest.Result.ProtocolError))
            {
                if (callback != null)
                {
                    callback(request.downloadHandler.text);
                }
            }
            else
            {
                Debug.LogError("Get Error");
            }
        }
    }
}

