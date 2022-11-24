using LitJson;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Fusion.Frameworks.Assets
{
    /// <summary>
    /// 记录了资源与AssetBundle的名称Map以及相关资源的配置
    /// </summary>
    public static class AssetsConfig
    {
        public static readonly string assetBundleSuffix = ".bundle";

        private static Dictionary<string, string> assetBundleNameMap = null;

        public static string GetAssetBundleName(string path)
        {
            if (assetBundleNameMap == null)
            {
                LoadAssetBundlesNameMap();
            }

            if (assetBundleNameMap != null && assetBundleNameMap.ContainsKey(path))
                return assetBundleNameMap[path];
            else
                return (path + assetBundleSuffix).ToLower();
        }

        private static void LoadAssetBundlesNameMap()
        {
            LoadAssetBundlesNameMap(AssetsManager.PersistentLoadPath);
            if (assetBundleNameMap == null)
            {
                LoadAssetBundlesNameMap(AssetsManager.StreamingLoadPath);
            }
        }

        private static void LoadAssetBundlesNameMap(string path)
        {
            string configJsonFilePath = Path.Combine(path, "AssetBundleConfig.json");
            System.Uri uri = new System.Uri(configJsonFilePath);
            UnityWebRequest webRequest = UnityWebRequest.Get(uri);
            UnityWebRequestAsyncOperation requestAOp = webRequest.SendWebRequest();
            while (requestAOp.isDone == false)
            {
            }
            if (!(webRequest.result == UnityWebRequest.Result.ConnectionError) && !(webRequest.result == UnityWebRequest.Result.ProtocolError))
            {
                assetBundleNameMap = JsonMapper.ToObject<Dictionary<string, string>>(webRequest.downloadHandler.text);
            }
        }
    }
}
