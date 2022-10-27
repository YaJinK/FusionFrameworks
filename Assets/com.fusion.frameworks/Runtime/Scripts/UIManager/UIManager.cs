using Fusion.Frameworks.Assets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

namespace Fusion.Frameworks.UI
{
    /// <summary>
    /// UI页面管理
    /// </summary>
    [DisallowMultipleComponent]
    public class UIManager : MonoBehaviour
    {
        private static UIManager instance;

        public static UIManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject uiManagerObject = new GameObject("UIManager");
                    instance = uiManagerObject.AddComponent<UIManager>();
                    GameObject canvasObject = GameObject.Find("Canvas");
                    if (canvasObject == null)
                    {
                        Debug.LogError("Can not find Canvas, please create it");
                    } else
                    {
                        instance.CanvasObject = canvasObject;
                    }
                    Transform rootTransform = canvasObject.transform.Find("Root");
                    if (rootTransform == null)
                    {
                        GameObject rootObject = new GameObject("Root");
                        RectTransform rectTransform = rootObject.AddComponent<RectTransform>();
                        rootObject.transform.SetParent(canvasObject.transform);
                        rectTransform.anchorMin = Vector2.zero;
                        rectTransform.anchorMax = Vector2.one;
                        rectTransform.anchoredPosition3D = Vector3.zero;
                        rectTransform.localScale = Vector3.one;
                        rectTransform.sizeDelta = Vector2.zero;
                        instance.rootObject = rootObject;
                    } else
                    {
                        instance.rootObject = rootTransform.gameObject;
                    }
                    DontDestroyOnLoad(uiManagerObject);
                }
                return instance;
            }
        }

        public GameObject CanvasObject { get => canvasObject; set => canvasObject = value; }

        private GameObject canvasObject = null;
        private GameObject rootObject = null;

        private List<UIObject> objectList = new List<UIObject>(4);
        private Dictionary<string, List<int>> indexListMap = new Dictionary<string, List<int>>();
        private Dictionary<string, UIObject> asyncHandling = new Dictionary<string, UIObject>();

        public UIObject Launch(string path, UIData data = null)
        {
            if (data == null)
            {
                data = new UIData();
            }
            data.Name = path;
            if (data.LaunchMode == UILaunchMode.Standard)
            {
                UIObject uiObject = CreateUIObject(path, data);
                HandleUILaunchData(uiObject);
                return uiObject;
            } else
            {
                int index = GetUIIndex(path);
                if (index >= 0)
                {
                    return LaunchSingleTop(index, data);
                } else
                {
                    UIObject uiObject = CreateUIObject(path, data);
                    HandleUILaunchData(uiObject);
                    return uiObject;
                }
            }
        }

        public UIObject CreateUIObject(string path, UIData data = null, GameObject rootObject = null)
        {
            if (rootObject == null)
            {
                rootObject = this.rootObject;
            }
            if (data == null)
            {
                data = new UIData();
            }
            data.Name = path;
            GameObject gameObject = AssetsUtility.CreateGameObject(path, rootObject);
            string className = data.Name.Substring(data.Name.LastIndexOf("/") + 1);
            Type classType = Type.GetType($"{className}, Assembly-CSharp");
            UIObject uiObject = classType != null ? (UIObject)Activator.CreateInstance(classType, data) : new UIObject(data);
            uiObject.GameObject = gameObject;
            uiObject.Init();
            uiObject.Update();

            return uiObject;
        }

        public UIObject LaunchAsync(string path, UIData data = null)
        {
            if (data == null)
            {
                data = new UIData();
            }
            data.Name = path;
            if (data.LaunchMode == UILaunchMode.Standard)
            {
                return CreateUIObjectAsyncHandling(path, data);
            }
            else
            {
                int index = GetUIIndex(path);
                if (index >= 0)
                {
                    return LaunchSingleTop(index, data);
                }
                else
                {
                    return CreateUIObjectAsyncHandling(path, data);
                }
            }
        }

        public UIObject CreateUIObjectAsyncHandling(string path, UIData data)
        {
            Dictionary<string, UIObject> asyncHandling = GetCurrentAsyncHandling();
            if (asyncHandling.ContainsKey(path))
            {
            Debug.LogError(asyncHandling[path]);
                return asyncHandling[path];
            }
            else
            {
                UIObject uiObject = CreateUIObjectAsync(path, data, null, delegate ()
                {
                    asyncHandling.Remove(path);
                });
                asyncHandling[path] = uiObject;
                HandleUILaunchData(uiObject);
                return uiObject;
            }
        }

        public UIObject CreateUIObjectAsync(string path, UIData data = null, GameObject rootObject = null, Action finishCallback = null)
        {
            if (rootObject == null)
            {
                rootObject = this.rootObject;
            }
            if (data == null)
            {
                data = new UIData();
            }
            data.Name = path;
            string className = data.Name.Substring(data.Name.LastIndexOf("/") + 1);
            Type classType = Type.GetType($"{className}, Assembly-CSharp");
            UIObject uiObject = classType != null ? (UIObject)Activator.CreateInstance(classType, data) : new UIObject(data);
            AssetsUtility.CreateGameObjectAsync(path, rootObject, false, delegate(GameObject gameObject)
            {
                uiObject.GameObject = gameObject;
                uiObject.Init();
                uiObject.Update();
                if (finishCallback != null)
                {
                    finishCallback();
                }
            });

            return uiObject;
        }

        public void Finish(UIObject uiObject, bool updateLast = false)
        {
            int index = GetUIIndex(uiObject);
            List<UIObject> objectList = GetCurrentUIObjectList();

            if (index == objectList.Count - 1 && index > 0)
            {
                UIObject lastUIObject = objectList[index - 1];
                if (updateLast)
                {
                    lastUIObject.Update();
                }
                lastUIObject.SetActive(true);
            }
            objectList.RemoveAt(index);
            Dictionary<string, List<int>> indexListMap = GetCurrentUIIndexListMap();
            List<int> uiIndexList = indexListMap[uiObject.Data.Name];
            if (uiIndexList.Count > 0)
            {
                uiIndexList.RemoveAt(uiIndexList.Count - 1);
            }
            AssetsUtility.Release(uiObject.GameObject);
        }

        private List<UIObject> GetCurrentUIObjectList()
        {
            return objectList;
        }
        private Dictionary<string, List<int>> GetCurrentUIIndexListMap()
        {
            return indexListMap;
        }
        private Dictionary<string, UIObject> GetCurrentAsyncHandling()
        {
            return asyncHandling;
        }

        private int GetLastWindowIndex()
        {
            List<UIObject> objectList = GetCurrentUIObjectList();
            int index = -1;
            for (int i=objectList.Count - 1; i >= 0; i--)
            {
                UITempleteData templeteData = objectList[i].GameObject.GetComponent<UITempleteData>();
                if (templeteData.Type == UIType.Window)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        private void HandleUILaunchData(UIObject uiObject)
        {
            UIData data = uiObject.Data;
            List<UIObject> objectList = GetCurrentUIObjectList();
            int lastWindowIndex = GetLastWindowIndex();
            if (lastWindowIndex >= 0)
            {
                for (int i= lastWindowIndex; i < objectList.Count; i++)
                {
                    objectList[i].SetActive(false);
                }
            }

            objectList.Add(uiObject);

            Dictionary<string, List<int>> indexListMap = GetCurrentUIIndexListMap();
            List<int> uiIndexList = null;
            if (indexListMap.ContainsKey(data.Name))
            {
                uiIndexList = indexListMap[data.Name];
            }
            else    
            {
                uiIndexList = new List<int>();
                indexListMap[data.Name] = uiIndexList;
            }

            uiIndexList.Add(objectList.Count - 1);

            uiObject.SetActive(true);
        }

        private int GetUIIndex(UIObject uiObject)
        {
            return GetUIIndex(uiObject.Data.Name);
        }
        private int GetUIIndex(string name)
        {
            Dictionary<string, List<int>> indexListMap = GetCurrentUIIndexListMap();
            int index = -1;
            if (indexListMap.ContainsKey(name))
            {
                List<int> uiIndexList = indexListMap[name];
                if (uiIndexList.Count > 0)
                {
                    index = uiIndexList[uiIndexList.Count - 1];
                }

            }
            return index;
        }

        private UIObject LaunchSingleTop(int index, UIData data)
        {
            List<UIObject> objectList = GetCurrentUIObjectList();
            for (int i = index + 1; i < objectList.Count; i++) 
            {
                AssetsUtility.Release(objectList[i].GameObject);
            }
            int removeIndex = index + 1;
            objectList.RemoveRange(removeIndex, objectList.Count - removeIndex);

            UIObject uiObject = objectList[index];
            uiObject.Data = data;
            uiObject.Update();
            uiObject.SetActive(true);
            return uiObject;
        }

        
    }
}


