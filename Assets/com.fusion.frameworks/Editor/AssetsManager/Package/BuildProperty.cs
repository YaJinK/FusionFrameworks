using UnityEngine;

namespace Fusion.Frameworks.Assets.Editor
{
    public enum BuildType
    {
        File,   // 每个文件一个AssetBundle
        Folder, // 每个文件夹一个AssetBundle
    }

    public enum FolderCompressType
    {
        LZMA = 0,
        Uncompressed = 1,
        LZ4 = 256,
        UseBuildSetting = 10000
    }

    [CreateAssetMenu]
    public class BuildProperty : ScriptableObject
    {
        [SerializeField]
        public BuildType type = BuildType.File;

        [SerializeField]
        public FolderCompressType compressType = FolderCompressType.UseBuildSetting;
    }
}
