using UnityEngine;

namespace Fusion.Frameworks.Assets.Editor
{
    public enum BuildType
    {
        File,   // 每个文件一个AssetBundle
        Folder, // 每个文件夹一个AssetBundle
    }

    [CreateAssetMenu]
    public class BuildProperty : ScriptableObject
    {
        [SerializeField]
        public BuildType type = BuildType.File;
    }
}
