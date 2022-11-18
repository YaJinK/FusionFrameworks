using Fusion.Frameworks.Assets;
using System.IO;
using UnityEngine;

namespace Fusion.Frameworks.UI
{
    /// <summary>
    /// UI运行模式
    /// </summary>
    public enum UILaunchMode
    {
        Standard,   //无论是否有相同的UI，直接创建新的
        SingleTop   //如果有相同的UI，则展示已存在的UI页面，并关闭其上层所有界面。否则创建新的。
    }

    /// <summary>
    /// UI类型
    /// </summary>
    public enum UIType
    {
        Window,     //展示Window UI时，会把上一个Window类型的UI及其上层Pop全部隐藏。
        Pop         //展示Pop UI时，不会隐藏任何UI
    }

    /// <summary>
    /// UI页面数据，可以继承UIData，扩展需要传输的数据
    /// </summary>
    public class UIData
    {
        private UILaunchMode launchMode = UILaunchMode.Standard;

        public UIData()
        {
        }

        public UIData(UILaunchMode launchMode)
        {
            this.launchMode = launchMode;
        }

        public string Name { get; set; }    // UI Prefab的路径
        public UILaunchMode LaunchMode { get => launchMode; set => launchMode = value; }
    }

    /// <summary>
    /// UI页面对象
    /// 所有UI必须继承UIObject，并且必须实现UIObject中的构造方法
    /// 可以在子类的构造方法中设置UIType和sortringOrder
    /// 在Init方法中，进行空间查找及一次性的静态空间属性设置
    /// 在Update方法中，设置动态的空间属性
    /// Release方法参数传true时，会自动调用上一个页面的Update方法进行页面更新
    /// </summary>
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
        public bool Active { get => active; }

        public UIObject()
        {
        }

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

        /// <summary>
        /// 同步创建子页面
        /// </summary>
        /// <param name="path">UI Prefab的路径</param>
        /// <param name="data">UI页面数据</param>
        /// <param name="rootObject">页面父节点</param>
        /// <returns></returns>
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
        /// <summary>
        /// 异步创建子页面
        /// </summary>
        /// <param name="path">UI Prefab的路径</param>
        /// <param name="data">UI页面数据</param>
        /// <param name="rootObject">页面父节点</param>
        /// <returns></returns>
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

