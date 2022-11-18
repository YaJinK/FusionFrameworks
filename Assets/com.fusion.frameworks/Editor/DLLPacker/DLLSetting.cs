using UnityEngine;

namespace Fusion.Frameworks.DynamicDLL.Editor
{
    [CreateAssetMenu(fileName = "Assets/GameAssets/DLLSetting", menuName = "FusionConfig/DLL Setting")]
    public class DLLSetting : ScriptableObject
    {
        [SerializeField]
        [Tooltip("Only Folder or C# Script")]
        public UnityEngine.Object[] scriptsForPack;

        [SerializeField]
        public string[] adpters;
    }
}
