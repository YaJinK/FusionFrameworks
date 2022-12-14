using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Fusion.Frameworks.Assets
{
    /// <summary>
    /// 提供AssetBundle及Asset的加载
    /// </summary>
    [DisallowMultipleComponent]
    public class AssetsManager : MonoBehaviour
    {
        private static AssetsManager instance;
        private static AssetBundleManifest assetBundleManifest;
        private static Dictionary<string, AssetBundleCreateRequest> asyncCreateRequests = new Dictionary<string, AssetBundleCreateRequest>();

        public static string StreamingLoadPath {
            get
            {
                return $"{Application.streamingAssetsPath}/ManagedAssets";
            }
        }

        public static string PersistentLoadPath
        {
            get
            {
                return $"{Application.persistentDataPath}/ManagedAssets";
            }
        }

        public static AssetsManager Instance { 
            get 
            {
                if (instance == null)
                {
                    GameObject assetsManagerObject = new GameObject("AssetsManager");
                    instance = assetsManagerObject.AddComponent<AssetsManager>();
                    DontDestroyOnLoad(assetsManagerObject);
#if FUSION_ASSETBUNDLE || !UNITY_EDITOR
                    AssetBundle assetBundle = LoadFromFile("ManagedAssets");
                    assetBundleManifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                    assetBundle.Unload(false);
                    Debug.Log("AssetBundle Enabled");
#endif
                }
                return instance; 
            } 
        }

        private static AssetBundle LoadFromFile(string assetBundleName)
        {
            string persistentPath = Path.Combine(PersistentLoadPath, assetBundleName);
            string loadPath = File.Exists(persistentPath) ? persistentPath : Path.Combine(StreamingLoadPath, assetBundleName);

            AssetBundle assetBundle = AssetBundle.LoadFromFile(loadPath);
            return assetBundle;
        }

        private static AssetBundleCreateRequest LoadFromFileAsync(string assetBundleName)
        {
            string persistentPath = Path.Combine(PersistentLoadPath, assetBundleName);
            string loadPath = File.Exists(persistentPath) ? persistentPath : Path.Combine(StreamingLoadPath, assetBundleName);
            AssetBundleCreateRequest assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(loadPath);
            return assetBundleCreateRequest;
        }

        public static AssetBundleManifest AssetBundleManifest { get => assetBundleManifest; }

        public AssetBundle LoadAssetBundle(string path)
        {
            LoadAssetBundleDependencies(path);
            string assetBundleName = AssetsConfig.GetAssetBundleName(path);
            AssetBundle assetBundle = AssetReferences.Instance.Get(assetBundleName);
            if (assetBundle == null)
            {
                assetBundle = LoadFromFile(assetBundleName);
            }
            AssetReferences.Instance.Reference(assetBundle);
            return assetBundle;
        }
        public AssetBundle[] LoadAssetBundleDependencies(string path)
        {
            string assetBundleName = AssetsConfig.GetAssetBundleName(path);
            string[] assetBundleDependencies = assetBundleManifest.GetAllDependencies(assetBundleName);
            int nameLength = assetBundleDependencies.Length;
            AssetBundle[] assetBundles = new AssetBundle[nameLength];
            for (int i = 0; i < nameLength; i++)
            {
                assetBundles[i] = AssetReferences.Instance.Get(assetBundleDependencies[i]);
                if (assetBundles[i] == null)
                {
                    assetBundles[i] = LoadFromFile(assetBundleDependencies[i]);
                }
                AssetReferences.Instance.Reference(assetBundles[i]);
            }
            return assetBundles;
        }

        public IEnumerator LoadAssetBundleAsync(string path, Action<AssetBundle> finishCallback = null)
        {
            yield return LoadAssetBundleDependenciesAsync(path);
            string assetBundleName = AssetsConfig.GetAssetBundleName(path);
            AssetBundle assetBundle = AssetReferences.Instance.Get(assetBundleName);
            if (assetBundle == null)
            {
                AssetBundleCreateRequest assetBundleCreateRequest;
                if (!asyncCreateRequests.ContainsKey(assetBundleName))
                {
                    assetBundleCreateRequest = LoadFromFileAsync(assetBundleName);
                    asyncCreateRequests[assetBundleName] = assetBundleCreateRequest;
                    yield return assetBundleCreateRequest;
                } else
                {
                    assetBundleCreateRequest = asyncCreateRequests[assetBundleName];
                    while (!assetBundleCreateRequest.isDone)
                    {
                        yield return null;
                    }
                }
                    
                asyncCreateRequests.Remove(assetBundleName);
                assetBundle = assetBundleCreateRequest.assetBundle;
            }
            
            AssetReferences.Instance.Reference(assetBundle);
            if (finishCallback != null)
            {
                finishCallback(assetBundle);
            }
        }

        public IEnumerator LoadAssetBundleDependenciesAsync(string path, Action<AssetBundle[]> finishCallback = null)
        {
            string assetBundleName = AssetsConfig.GetAssetBundleName(path);
            string[] assetBundleDependencies = assetBundleManifest.GetAllDependencies(assetBundleName);
            int nameLength = assetBundleDependencies.Length;
            AssetBundle[] assetBundles = new AssetBundle[nameLength];
            for (int i = 0; i < nameLength; i++)
            {
                AssetBundle assetBundle = AssetReferences.Instance.Get(assetBundleDependencies[i]);
                if (assetBundle == null)
                {
                    AssetBundleCreateRequest assetBundleCreateRequest;
                    if (!asyncCreateRequests.ContainsKey(assetBundleDependencies[i]))
                    {
                        assetBundleCreateRequest = LoadFromFileAsync(assetBundleDependencies[i]);
                        asyncCreateRequests[assetBundleDependencies[i]] = assetBundleCreateRequest;
                    } else
                    {
                        assetBundleCreateRequest = asyncCreateRequests[assetBundleDependencies[i]];
                    }
                    yield return assetBundleCreateRequest;
                    assetBundle = assetBundleCreateRequest.assetBundle;
                    asyncCreateRequests.Remove(assetBundleDependencies[i]);
                }
                assetBundles[i] = assetBundle;
                AssetReferences.Instance.Reference(assetBundle);
            }

            if (finishCallback != null)
            {
                finishCallback(assetBundles);
            }
        }

        public T Load<T>(string path) where T : UnityEngine.Object
        {
#if FUSION_ASSETBUNDLE || !UNITY_EDITOR
            AssetBundle assetBundle = LoadAssetBundle(path);
            T asset = assetBundle.LoadAsset<T>(GetAssetNameByPath(path));
            return asset;
#else
            T asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(GetPathWithSuffix($"Assets/GameAssets/{path}"));
            return asset;
#endif
        }

#if !FUSION_ASSETBUNDLE && UNITY_EDITOR
        private string GetPathWithSuffix(string path)
        {
            string name = GetAssetNameByPath(path);
            string dir = path.Substring(0, path.LastIndexOf("/"));
            string[] assets = UnityEditor.AssetDatabase.FindAssets(name, new string[] { dir });
            List<string> resultPaths = new List<string>();
            for (int index = 0; index < assets.Length; index++)
            {
                string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(assets[index]);
                int suffixStart = assetPath.LastIndexOf(".");
                if (suffixStart != -1)
                {
                    string pathWithoutSuffix = assetPath.Substring(0, assetPath.LastIndexOf("."));
                    if (pathWithoutSuffix == path)
                    {
                        resultPaths.Add(assetPath);
                    }
                }
            }

            if (resultPaths.Count == 1)
            {
                return resultPaths[0];
            } else if (resultPaths.Count > 1)
            {
                Debug.LogError($"There are assets with same name in directory [{dir}], please check and fix them.");
                return resultPaths[0];
            } else
            {
                Debug.LogError($"Can not find asset with name [{name}] in directory [{dir}].");
                return path;
            }

        }
#endif

        private IEnumerator LoadAsyncCoroutine<T>(string path, Action<T> finishCallback) where T : UnityEngine.Object
        {
            AssetBundle assetBundle = null;
            yield return LoadAssetBundleAsync(path, delegate (AssetBundle assetBundleLoaded)
            {
                assetBundle = assetBundleLoaded;
            });

            AssetBundleRequest assetBundleRequest = assetBundle.LoadAssetAsync<T>(GetAssetNameByPath(path));
            yield return assetBundleRequest;
            if (finishCallback != null)
            {
                finishCallback((T)assetBundleRequest.asset);
            }
        }

        public void LoadAsync<T>(string path, Action<T> finishCallback = null) where T : UnityEngine.Object
        {
#if FUSION_ASSETBUNDLE || !UNITY_EDITOR
            StartCoroutine(LoadAsyncCoroutine(path, finishCallback));
#else
            T asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(GetPathWithSuffix($"Assets/GameAssets/{path}"));
            if (finishCallback != null)
            {
                finishCallback(asset);
            }
#endif
        }

        public string GetAssetNameByPath(string path)
        {
            return path.Substring(path.LastIndexOf('/') + 1);
        }
    }
}

