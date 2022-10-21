using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion.Frameworks.Assets
{
    /// <summary>
    /// 异步设置资源时，记录资源加载完成后的回调
    /// 当回调执行之后，会把回调函数删除
    /// </summary>
    [DisallowMultipleComponent]
    public class AssetAsyncHandler : MonoBehaviour
    {
        Dictionary<string, Action<UnityEngine.Object>> asyncCallbacks = new Dictionary<string, Action<UnityEngine.Object>>();

        public void RegisterCallback(string asyncCallbackType, Action<UnityEngine.Object> callback)
        {
            asyncCallbacks[asyncCallbackType] = callback;
        }

        public Action<UnityEngine.Object> GetCallback(string asyncCallbackType)
        {
            return asyncCallbacks.ContainsKey(asyncCallbackType) ? asyncCallbacks[asyncCallbackType] : null;
        }

        public void RemoveCallback(string asyncCallbackType)
        {
            asyncCallbacks.Remove(asyncCallbackType);
        }
    }
}
