using Fusion.Frameworks.Version;
using UnityEditor;
using UnityEngine;

namespace Fusion.Frameworks.Editor
{
    public enum CompressType {
        LZMA = 0,
        Uncompressed = 1,
        LZ4 = 256,
    }

    public enum BuildTargetType {
        UseCurrentTarget = 0,
        Android = 13,
        iOS = 9,
        StandaloneWindows = 5,
        StandaloneWindows64 = 19,
        PS4 = 31,
        PS5 = 44,
        Switch = 38,
        WebGL = 20
    }


    [CreateAssetMenu(fileName = "Assets/GameAssets/BuildSetting", menuName = "FusionConfig/Build Setting")]
    public class BuildSetting : ScriptableObject
    {
        [SerializeField]
        public CompressType compressType = CompressType.LZ4;

        [SerializeField]
        public BuildTargetType buildTargetType = BuildTargetType.UseCurrentTarget;

        [SerializeField]
        public SceneAsset initScene;

        [SerializeField]
        public VersionData version;
    }
}
