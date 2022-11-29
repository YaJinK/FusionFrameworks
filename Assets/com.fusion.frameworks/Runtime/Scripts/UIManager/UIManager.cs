using Fusion.Frameworks.Assets;
using Fusion.Frameworks.DynamicDLL;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
                    GameObject canvasObject = GameObject.Find("UI/Canvas");
                    if (canvasObject == null)
                    {
                        Debug.LogError("Can not find UI/Canvas in Hierarchy, please drag it from Prefab/UI/UI");
                    } else
                    {
                        GameObject uiObject = GameObject.Find("UI");
                        instance.CanvasObject = canvasObject;
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
                        }
                        else
                        {
                            instance.rootObject = rootTransform.gameObject;
                        }
                        DontDestroyOnLoad(uiManagerObject);
                    }
                    
                }
                return instance;
            }
        }

        public GameObject CanvasObject { get => canvasObject; set => canvasObject = value; }

        private GameObject canvasObject = null;
        private GameObject rootObject = null;

        private List<UIObject> objectList = new List<UIObject>(4);
        private Dictionary<string, AsyncHandlingData> asyncHandling = new Dictionary<string, AsyncHandlingData>();

        private class AsyncHandlingData {
            public UIObject uiObject = null;
            public int objectIndex = 0;
        }

        private float zDistance = -5.0f;

        /// <summary>
        /// 同步运行UI页面，页面会被UIManager管理
        /// </summary>
        /// <param name="path">UI Prefab的路径</param>
        /// <param name="data">UI页面数据</param>
        /// <returns></returns>
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
                int objectIndex = HandleUILaunchData(uiObject);
                SetUIOffsetPosition(uiObject, objectIndex);
                return uiObject;
            } else
            {
                int index = GetTopUIIndex(path);
                if (index >= 0)
                {
                    return LaunchSingleTop(index, data);
                } else
                {
                    UIObject uiObject = CreateUIObject(path, data);
                    int objectIndex = HandleUILaunchData(uiObject);
                    SetUIOffsetPosition(uiObject, objectIndex);
                    return uiObject;
                }
            }
        }

        /// <summary>
        /// 同步创建UI自由页面，不会被UIManager管理
        /// </summary>
        /// <param name="path">UI Prefab的路径</param>
        /// <param name="data">UI页面数据</param>
        /// <param name="rootObject">页面父节点</param>
        /// <returns></returns>
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
            string className = data.Name.Replace("/", ".");

            UIObject uiObject = DLLManager.Instance.Instantiate<UIObject>(className, data);
            uiObject.GameObject = gameObject;
            Canvas canvas = uiObject.GameObject.GetOrAddComponent<Canvas>();
            uiObject.GameObject.GetOrAddComponent<GraphicRaycaster>();
            canvas.SetOverrideSorting(true);
            canvas.sortingOrder = uiObject.SortingOrder;
            bool oldActive = uiObject.Active;
            uiObject.SetActive(true);
            uiObject.Init();
            uiObject.Update();
            uiObject.SetActive(oldActive);

            return uiObject;
        }

        /// <summary>
        /// 异步运行UI页面，页面会被UIManager管理，同一个Prefab只能同时运行一个
        /// </summary>
        /// <param name="path">UI Prefab的路径</param>
        /// <param name="data">UI页面数据</param>
        /// <returns></returns>
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
                int index = GetTopUIIndex(path);
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

        /// <summary>
        /// 异步创建UI自由页面，不会被UIManager管理
        /// </summary>
        /// <param name="path">UI Prefab的路径</param>
        /// <param name="data">UI页面数据</param>
        /// <param name="rootObject">页面父节点</param>
        /// <param name="finishCallback">创建完成后回调</param>
        /// <returns></returns>
        public UIObject CreateUIObjectAsync(string path, UIData data = null, GameObject rootObject = null, Action<UIObject> finishCallback = null)
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
            string className = data.Name.Replace("/", ".");
            UIObject uiObject = DLLManager.Instance.Instantiate<UIObject>(className, data);
            AssetsUtility.CreateGameObjectAsync(path, rootObject, false, delegate(GameObject gameObject)
            {
                uiObject.GameObject = gameObject;
                Canvas canvas = uiObject.GameObject.GetOrAddComponent<Canvas>();
                uiObject.GameObject.GetOrAddComponent<GraphicRaycaster>();
                canvas.SetOverrideSorting(true);
                canvas.sortingOrder = uiObject.SortingOrder;
                bool oldActive = uiObject.Active;
                uiObject.SetActive(true);
                uiObject.Init();
                uiObject.Update();
                uiObject.SetActive(oldActive);
                if (finishCallback != null)
                {
                    finishCallback(uiObject);
                }
            });

            return uiObject;
        }

        /// <summary>
        /// 关闭UI页面，正在异步加载中的页面不能被关闭
        /// </summary>
        /// <param name="uiObject">UI对象</param>
        /// <param name="updateLast">是否更新上一个页面</param>
        public void Finish(UIObject uiObject, bool updateLast = false)
        {
            if (!uiObject.CheckValid())
            {
                return;
            }

            int index = GetTopUIIndex(uiObject);
            List<UIObject> objectList = GetCurrentUIObjectList();
            objectList.RemoveAt(index);

            if (index == objectList.Count && index > 0)
            {
                int lastWindowIndex = GetLastWindowIndex();
                for (int i = lastWindowIndex; i < objectList.Count; i++)
                {
                    UIObject windowUIObject = objectList[i];
                    windowUIObject.SetActive(true);
                }
                UIObject lastUIObject = objectList[index - 1];

                if (updateLast)
                {
                    lastUIObject.Update();
                }
            }
            AssetsUtility.Release(uiObject.GameObject);
            uiObject.GameObject = null;
        }

        public void Clear()
        {
            for (int index = 0; index < objectList.Count; index++)
            {
                UIObject uiObject = objectList[index];
                AssetsUtility.ReleaseImmediate(uiObject.GameObject);
                uiObject.GameObject = null;
            }
            objectList.Clear();
            asyncHandling.Clear();
        }

        public void Restore(List<UIObject> restoreList)
        {
            Clear();
            for (int index = 0; index < restoreList.Count; index++)
            {
                UIObject uiObject = restoreList[index];
                if (uiObject.GameObject == null)
                {
                    GameObject gameObject = AssetsUtility.CreateGameObject(uiObject.Data.Name, rootObject);
                    uiObject.GameObject = gameObject;
                }
                objectList.Add(uiObject);
                bool oldActive = uiObject.Active;
                uiObject.SetActive(true);
                uiObject.Init();
                uiObject.Update();
                uiObject.SetActive(oldActive);
            }
        }

        public void ForEach(Action<UIObject> foreachCallback)
        {
            for (int index = 0; index < objectList.Count; index++)
            {
                foreachCallback(objectList[index]);
            }
        }

        private void SetUIOffsetPosition(UIObject uiObject, int index)
        {
            if (index > 0)
            {
                List<UIObject> objectList = GetCurrentUIObjectList();
                uiObject.GameObject.transform.localPosition = new Vector3(0, 0, zDistance + objectList[index - 1].GameObject.transform.localPosition.z);
            } else
            {
                uiObject.GameObject.transform.localPosition = Vector3.zero;
            }

        }
        private UIObject CreateUIObjectAsyncHandling(string path, UIData data)
        {
            if (asyncHandling.ContainsKey(path))
            {
                return asyncHandling[path].uiObject;
            }
            else
            {
                UIObject uiObject = CreateUIObjectAsync(path, data, null, delegate (UIObject uiObject)
                {
                    if (asyncHandling.ContainsKey(path) && objectList.Contains(uiObject))
                    {
                        SetUIOffsetPosition(uiObject, asyncHandling[path].objectIndex);
                        asyncHandling.Remove(path);
                    } else
                    {
                        AssetsUtility.Release(uiObject.GameObject);
                    }
                });
                asyncHandling[path] = new AsyncHandlingData();
                asyncHandling[path].uiObject = uiObject;
                asyncHandling[path].objectIndex = HandleUILaunchData(uiObject);
                return uiObject;
            }
        }

        private List<UIObject> GetCurrentUIObjectList()
        {
            return objectList;
        }
     
        private int GetLastWindowIndex()
        {
            List<UIObject> objectList = GetCurrentUIObjectList();
            int index = -1;
            for (int i=objectList.Count - 1; i >= 0; i--)
            {
                if (objectList[i].Type == UIType.Window)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        private int HandleUILaunchData(UIObject uiObject)
        {
            UIData data = uiObject.Data;
            List<UIObject> objectList = GetCurrentUIObjectList();
            int lastWindowIndex = GetLastWindowIndex();
            int insertIndex = 0;
            int objectListMaxIndex = objectList.Count - 1;
            for (int index = objectListMaxIndex; index >= 0; index--)
            {
                UIObject obj = objectList[index];
                if (obj.SortingOrder <= uiObject.SortingOrder)
                {
                    insertIndex = index + 1;
                    break;
                }
            }
            objectList.Insert(insertIndex, uiObject);
            if (uiObject.Type == UIType.Window && lastWindowIndex >= 0)
            {
                for (int i = lastWindowIndex; i < insertIndex; i++)
                {
                    objectList[i].SetActive(false);
                }
            }
            uiObject.SetActive(insertIndex > lastWindowIndex);

            return insertIndex;
        }

        private int GetTopUIIndex(UIObject uiObject)
        {
            return GetTopUIIndex(uiObject.Data.Name);
        }
        private int GetTopUIIndex(string name)
        {
            List<UIObject> objectList = GetCurrentUIObjectList();

            int index = -1;

            for (int i=objectList.Count - 1; i >= 0; i--)
            {
                UIObject uiObject = objectList[i];
                if (uiObject.Data.Name == name)
                {
                    index = i;
                    break;
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
            uiObject.SetActive(true);
            uiObject.Update();
            return uiObject;
        }
    }
}


