using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion.Frameworks.Assets
{
    /// <summary>
    /// �첽������Դʱ����¼��Դ������ɺ�Ļص�
    /// ���ص�ִ��֮�󣬻�ѻص�����ɾ��
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
