using Fusion.Frameworks.Assets;
using System.IO;
using UnityEngine;

namespace Fusion.Frameworks.UI
{
    public enum UILaunchMode
    {
        Standard,
        SingleTop
    }
    public enum UIType
    {
        Window,
        Pop
    }

    public class UIData
    {
        private UILaunchMode launchMode = UILaunchMode.Standard;

        public string Name { get; set; }
        public UILaunchMode LaunchMode { get => launchMode; set => launchMode = value; }
    }

    public class UIObject
    {
        protected GameObject gameObject = null;
        protected UIData data = null;

        protected UIType type = UIType.Window;
        protected int sortingOrder = 0;

        private bool isChild = false;
        protected UIObject parent = null;

        private bool active = false;

        public UIData Data { get => data; set => data = value; }
        public GameObject GameObject { 
            get => gameObject; 
            set 
            { 
                gameObject = value;
                SetActive(active);
            } 
        }

        public UIType Type { get => type; }
        public int SortingOrder { get => sortingOrder; }

        public UIObject(UIData data)
        {
            this.data = data;
        }

        public void SetActive(bool value)
        {
            active = value;
            if (gameObject != null)
            {
                gameObject.SetActive(value);
            }
        }

        public void Finish(bool updateLast = false)
        {
            if (!isChild)
            {
                UIManager.Instance.Finish(this, updateLast);
            } else
            {
                AssetsUtility.Release(gameObject);
                if (updateLast)
                {
                    parent.Update();
                }
            }
        }

        public bool CheckValid()
        {
            return gameObject != null;
        }

        public virtual void Init()
        {

        }

        public virtual void Update()
        {

        }

        protected UIObject LaunchChild(string path, UIData data = null, GameObject rootObject = null)
        {
            if (rootObject == null)
            {
                rootObject = gameObject;
            }

            if (data == null)
            {
                data = new UIData();
            }
            data.Name = path;

            UIObject uiObject = UIManager.Instance.CreateUIObject(path, data, rootObject);
            uiObject.isChild = true;
            uiObject.parent = this;
            uiObject.SetActive(true);
            return uiObject;
        }

        protected UIObject LaunchChildAsync(string path, UIData data = null, GameObject rootObject = null)
        {
            if (rootObject == null)
            {
                rootObject = gameObject;
            }

            if (data == null)
            {
                data = new UIData();
            }
            data.Name = path;

            UIObject uiObject = UIManager.Instance.CreateUIObjectAsync(path, data, rootObject);
            uiObject.isChild = true;
            uiObject.parent = this;
            uiObject.SetActive(true);
            return uiObject;
        }
    }
}

