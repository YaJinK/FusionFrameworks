using UnityEngine;

namespace Fusion.Frameworks.DynamicDLL.Editor
{
    [CreateAssetMenu(fileName = "Assets/GameAssets/DLLSetting", menuName = "FusionConfig/DLL Setting")]
    public class DLLSetting : ScriptableObject
    {
        [SerializeField]
        [Tooltip("Only Folder")]
        public UnityEngine.Object[] scriptsForPack;

        [SerializeField]
        [Tooltip(@"[ClassFullName],[AssemblyName]
Example: UnityEngine.MonoBehaviour,UnityEngine")]
        public string[] customAdapters;

        [SerializeField]
        [Tooltip(@"[ClassFullName]
Example: UnityEngine.GameObject")]
        public string[] customMethodDelegates;
    }
}
