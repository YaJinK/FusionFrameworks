using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Fusion.Frameworks.UI
{
    public enum UIType
    {
        Window,
        Pop
    }
    public class UITempleteData : MonoBehaviour
    {
        [SerializeField]
        private UIType type = UIType.Window;
        public UIType Type { get => type; }
    }
}
