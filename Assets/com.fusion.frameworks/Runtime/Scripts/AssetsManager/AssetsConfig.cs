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

        private static Dictionary<string, string> spritePathAtlasNameMap = null;

        public static string GetAssetBundleName(string path)
        {
            if (spritePathAtlasNameMap == null)
            {
                LoadAssetBundlesNameMap(Application.streamingAssetsPath);
            }

            if (spritePathAtlasNameMap != null && spritePathAtlasNameMap.ContainsKey(path))
                return spritePathAtlasNameMap[path];
            else
                return (path + assetBundleSuffix).ToLower();
        }

        public static void LoadAssetBundlesNameMap(string path)
        {
            string configJsonFilePath = Path.Combine(path, "AssetBundleConfig.json");
            // Debug.Log(configJsonFilePath);
            System.Uri uri = new System.Uri(configJsonFilePath);
            // Debug.Log(uri);
            UnityWebRequest webRequest = UnityWebRequest.Get(uri);
            UnityWebRequestAsyncOperation requestAOp = webRequest.SendWebRequest();
            while (requestAOp.isDone == false)
            {
            }
            if (!(webRequest.result == UnityWebRequest.Result.ConnectionError) && !(webRequest.result == UnityWebRequest.Result.ProtocolError))
                spritePathAtlasNameMap = JsonUtility.FromJson<DictionarySerialization<string, string>>(webRequest.downloadHandler.text).ToDictionary();
        }
    }
}
