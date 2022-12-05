using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fusion.Frameworks.Assets
{
    public class AssetData
    {
        private AssetBundle assetBundle = null;
        private int referenceCount = 0;
        private Coroutine releaseCoroutine = null;

        public int ReferenceCount
        {
            get { return referenceCount; }
        }

        public AssetBundle AssetBundle { get => assetBundle; }
        public Coroutine ReleaseCoroutine { get => releaseCoroutine; set => releaseCoroutine = value; }

        public AssetData(AssetBundle assetBundle)
        {
            this.assetBundle = assetBundle;
            Reference();
        }

        public void Reference()
        {
            referenceCount++;
        }
        public void Release(int count = 1)
        {
            if (referenceCount > 0)
            {
                referenceCount -= count;
            }
            if (referenceCount <= 0)
            {
                referenceCount = 0;
            }
        }

        public void ReleaseAssetBundle()
        {
            assetBundle.Unload(true);
        }

    }

    /// <summary>
    /// 记录AssetBundle的引用数量，当引用数量为0时会销毁AssetBundle以及加载出来的资源
    /// </summary>
    [DisallowMultipleComponent]
    public class AssetReferences : MonoBehaviour
    {
        private static AssetReferences instance;
        private Dictionary<string, AssetData> assetReferences = new Dictionary<string, AssetData>();
        private float releaseDelay = 4.0f;
        public static AssetReferences Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject asssetReferencesObject = new GameObject("AssetReferences");
                    instance = asssetReferencesObject.AddComponent<AssetReferences>();
                    DontDestroyOnLoad(asssetReferencesObject);
                }
                return instance;
            }
        }

        public void Reference(AssetBundle assetBundle)
        {
            string name = assetBundle.name;
            AssetData assetData;
            bool result = assetReferences.TryGetValue(name, out assetData);
            if (result)
            {
                if (assetData.ReleaseCoroutine != null) {
                    StopCoroutine(assetData.ReleaseCoroutine);
                    assetData.ReleaseCoroutine = null;
                }
                assetData.Reference();
            } else
            {
                assetReferences[name] = new AssetData(assetBundle);
            }
        }

        public void Release(AssetBundle assetBundle, int count = 1)
        {
            string name = assetBundle.name;
            Release(name, count);
        }

        public void ReleaseImmediate(AssetBundle assetBundle, int count = 1)
        {
            string name = assetBundle.name;
            ReleaseImmediate(name, count);
        }

        private void ReleaseAssetData(string name)
        {
            AssetData assetData = assetReferences[name];
            assetData.ReleaseAssetBundle();
            assetReferences.Remove(name);
        }
        private IEnumerator ReleaseAssetDataCountdown(string name)
        {
            yield return new WaitForSecondsRealtime(releaseDelay);
            ReleaseAssetData(name);
        }

        public void Release(string name, int count = 1)
        {
            name = name.ToLower();
            if (assetReferences.ContainsKey(name))
            {
                AssetData assetData = assetReferences[name];
                assetData.Release(count);
                if (assetReferences[name].ReferenceCount == 0)
                {
                    assetData.ReleaseCoroutine = StartCoroutine(ReleaseAssetDataCountdown(name));
                }
            }
        }

        public void ReleaseImmediate(string name, int count = 1)
        {
            name = name.ToLower();
            if (assetReferences.ContainsKey(name))
            {
                AssetData assetData = assetReferences[name];
                assetData.Release(count);
                if (assetReferences[name].ReferenceCount == 0)
                {
                    ReleaseAssetData(name);
                }
            }
        }

        public AssetBundle Get(string name)
        {
            name = name.ToLower();
            AssetData assetData;
            bool result = assetReferences.TryGetValue(name, out assetData);
            if (result)
            {
                return assetData.AssetBundle;
            }
            else
            {
                return null;
            }
        }

        public void Clear(string[] ignoreList = null)
        {
            Dictionary<string, int> ignoreMap = new Dictionary<string, int>();

            if (ignoreList != null)
            {
                for (int index = 0; index < ignoreList.Length; index++)
                {
                    if (!ignoreMap.ContainsKey(ignoreList[index]))
                    {
                        ignoreMap[ignoreList[index]] = 1;
                    } else
                    {
                        ignoreMap[ignoreList[index]]++;
                    }
                    if (AssetsManager.AssetBundleManifest != null)
                    {
                        string[] assetBundleDependencies = AssetsManager.AssetBundleManifest.GetAllDependencies(ignoreList[index]);
                        for (int i = 0; i < assetBundleDependencies.Length; i++)
                        {
                            if (!ignoreMap.ContainsKey(assetBundleDependencies[i]))
                            {
                                ignoreMap[assetBundleDependencies[i]] = 1;
                            }
                            else
                            {
                                ignoreMap[assetBundleDependencies[i]]++;
                            }
                        }
                    }
                }
            }
            
            List<AssetData> list = assetReferences.Values.ToList();
            foreach (AssetData data in list)
            {
                if (!ignoreMap.ContainsKey(data.AssetBundle.name))
                {
                    ReleaseImmediate(data.AssetBundle, data.ReferenceCount);
                } else
                {
                    data.Release(data.ReferenceCount - ignoreMap[data.AssetBundle.name]);
                }
            }
        }
    }
}

