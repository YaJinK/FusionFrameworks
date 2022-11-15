using UnityEngine;

namespace Fusion.Frameworks.ILRuntime.Editor
{
    [CreateAssetMenu(fileName = "Assets/GameAssets/DLLSetting", menuName = "FusionConfig/DLL Setting")]
    public class DLLSetting : ScriptableObject
    {
        [SerializeField]
        [Tooltip("Only Folder or C# Script")]
        public Object[] scriptsForPack;
    }
}
