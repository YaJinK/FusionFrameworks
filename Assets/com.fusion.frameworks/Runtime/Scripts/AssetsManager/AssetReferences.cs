using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion.Frameworks.Assets
{
    public class AssetData
    {
        private AssetBundle assetBundle = null;
        private int referenceCount = 0;

        public int ReferenceCount
        {
            get { return referenceCount; }
        }

        public AssetBundle AssetBundle { get => assetBundle; }

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
                assetBundle.Unload(true);
            }
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

        public void Release(string name, int count = 1)
        {
            name = name.ToLower();
            AssetData assetData = assetReferences[name];
            if (assetData != null)
            {
                assetData.Release(count);
                if (assetReferences[name].ReferenceCount == 0)
                {
                    assetReferences.Remove(name);
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
    }
}

