using UnityEditor;
using UnityEngine;

namespace Fusion.Frameworks.Editor
{
    public class Utility
    {
        [MenuItem("Utility/OpenPersistent")]
        public static void OpenPersistent()
        {
            System.Diagnostics.Process.Start("explorer.exe", Application.persistentDataPath.Replace("/", "\\"));
        }
    }
}


