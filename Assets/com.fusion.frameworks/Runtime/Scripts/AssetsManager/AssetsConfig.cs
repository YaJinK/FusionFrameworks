using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Fusion.Frameworks.Assets
{
    /// <summary>
    /// ��¼����Դ��AssetBundle������Map�Լ������Դ������
    /// </summary>
    public static class AssetsConfig
    {
        public static string packagePath = Application.streamingAssetsPath;

        public static readonly string assetBundleSuffix = "";

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
                return path + assetBundleSuffix;
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