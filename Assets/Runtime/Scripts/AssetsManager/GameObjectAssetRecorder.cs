using System.Collections.Generic;
using UnityEngine;

namespace Fusion.Frameworks.Assets
{
    public class AssetRecord
    {
        private string name = null;
        private int count = 0;

        public AssetRecord(string name)
        {
            this.name = name;
            Record();
        }
        public int Count { get => count; }
        public string Name { get => name; }

        public void Record()
        {
            count++;
        }
    }

    /// <summary>
    /// 记录了GameObject引用了哪些AssetBundle，销毁GameObject时，会减去对应的引用数
    /// </summary>
    [DisallowMultipleComponent]
    public class GameObjectAssetRecorder : MonoBehaviour
    {
        private Dictionary<string, AssetRecord> assetRecords = new Dictionary<string, AssetRecord>();


        public void Record(string assetBundleName)
        {
            if (assetRecords.ContainsKey(assetBundleName))
            {
                assetRecords[assetBundleName].Record();
            }
            else
            {
                assetRecords[assetBundleName] = new AssetRecord(assetBundleName);
            }
            if (AssetsManager.AssetBundleManifest != null)
            {
                string[] assetBundleDependencies = AssetsManager.AssetBundleManifest.GetAllDependencies(assetBundleName);
                for (int i = 0; i < assetBundleDependencies.Length; i++)
                {
                    if (assetRecords.ContainsKey(assetBundleDependencies[i]))
                    {
                        assetRecords[assetBundleDependencies[i]].Record();
                    }
                    else
                    {
                        assetRecords[assetBundleDependencies[i]] = new AssetRecord(assetBundleDependencies[i]);
                    }
                }
            }
        }

        public void Release()
        {
            Dictionary<string, AssetRecord>.Enumerator enumerator = assetRecords.GetEnumerator();
            while (enumerator.MoveNext())
            {
                AssetRecord assetRecord = enumerator.Current.Value;
                AssetReferences.Instance.Release(assetRecord.Name, assetRecord.Count);
            }
            assetRecords.Clear();
        }

        public void ReleaseImmediate()
        {
            Dictionary<string, AssetRecord>.Enumerator enumerator = assetRecords.GetEnumerator();
            while (enumerator.MoveNext())
            {
                AssetRecord assetRecord = enumerator.Current.Value;
                AssetReferences.Instance.ReleaseImmediate(assetRecord.Name, assetRecord.Count);
            }
            assetRecords.Clear();
        }
    }
}

