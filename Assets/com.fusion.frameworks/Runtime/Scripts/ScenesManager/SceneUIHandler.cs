using Fusion.Frameworks.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion.Frameworks.Scenes
{
    /// <summary>
    /// 在LoadAsyncTask中添加SceneUIHandler，可以保存当前场景的UI，下次加载这个场景时，会自动恢复UI
    /// storeUIChecker 这个值为null或者回调返回为true的UI才会被保存
    /// </summary>
    public class SceneUIHandler : SceneDataHandler
    {
        
        private List<UIObject> objectList = new List<UIObject>(4);

        public Func<UIObject, bool> storeUIChecker = null;

        public void Load()
        {
            UIManager.Instance.Restore(objectList);
            objectList.Clear();
        }

        public void Save()
        {
            UIManager.Instance.ForEach(delegate (UIObject uiObject)
            {
                if (storeUIChecker == null || storeUIChecker(uiObject))
                {
                    objectList.Add(uiObject);
                }
            });
        }
    }
}


