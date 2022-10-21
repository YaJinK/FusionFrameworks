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

        public static AssetsManager Instance { 
            get 
            {
                if (instance == null)
                {
                    GameObject assetsManagerObject = new GameObject("AssetsManager");
                    instance = assetsManagerObject.AddComponent<AssetsManager>();
                    DontDestroyOnLoad(assetsManagerObject);
                   
                    AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(AssetsConfig.packagePath, AssetsConfig.packagePath.Substring(AssetsConfig.packagePath.LastIndexOf("/") + 1)));
                    assetBundleManifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                }
                return instance; 
            } 
        }

        public static AssetBundleManifest AssetBundleManifest { get => assetBundleManifest; }

        public AssetBundle LoadAssetBundle(string path)
        {
            LoadAssetBundleDependencies(path);
            string assetBundleName = AssetsConfig.GetAssetBundleName(path);
            AssetBundle assetBundle = AssetReferences.Instance.Get(assetBundleName);
            if (assetBundle == null)
            {
                assetBundle = AssetBundle.LoadFromFile(Path.Combine(AssetsConfig.packagePath, assetBundleName));
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
                    assetBundles[i] = AssetBundle.LoadFromFile(Path.Combine(AssetsConfig.packagePath, assetBundleDependencies[i]));
                }
                AssetReferences.Instance.Reference(assetBundles[i]);
            }
            return assetBundles;
        }

        public IEnumerator LoadAssetBundleAsync(string path, Action<AssetBundle> finishCallback)
        {
            yield return LoadAssetBundleDependenciesAsync(path);
            string assetBundleName = AssetsConfig.GetAssetBundleName(path);
            AssetBundle assetBundle = AssetReferences.Instance.Get(assetBundleName);
            if (assetBundle == null)
            {
                AssetBundleCreateRequest assetBundleCreateRequest;
                if (!asyncCreateRequests.ContainsKey(assetBundleName))
                {
                    assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(Path.Combine(AssetsConfig.packagePath, assetBundleName));
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
            finishCallback(assetBundle);
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
                        assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(Path.Combine(AssetsConfig.packagePath, assetBundleDependencies[i]));
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
            AssetBundle assetBundle = LoadAssetBundle(path);
            T asset = assetBundle.LoadAsset<T>(GetAssetNameByPath(path));
            return asset;
        }

        private IEnumerator LoadAsyncCoroutine<T>(string path, Action<T> finishCallback) where T : UnityEngine.Object
        {
            AssetBundle assetBundle = null;
            yield return LoadAssetBundleAsync(path, delegate (AssetBundle assetBundleLoaded)
            {
                assetBundle = assetBundleLoaded;
            });

            AssetBundleRequest assetBundleRequest = assetBundle.LoadAssetAsync<T>(GetAssetNameByPath(path));
            yield return assetBundleRequest;
            finishCallback((T)assetBundleRequest.asset);
        }

        public void LoadAsync<T>(string path, Action<T> finishCallback) where T : UnityEngine.Object
        {
            StartCoroutine(LoadAsyncCoroutine(path, finishCallback));
        }

        public string GetAssetNameByPath(string path)
        {
            return path.Substring(path.LastIndexOf('/') + 1);
        }
    }
}

