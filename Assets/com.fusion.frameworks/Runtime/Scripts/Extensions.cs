using UnityEditor;
using UnityEngine;

namespace Fusion.Frameworks{
    public static class Extensions
    {
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (component == null)
                component = gameObject.AddComponent<T>();
            return component;
        }

        public static void SetOverrideSorting(this Canvas canvas, bool value)
        {
            bool originActive = canvas.gameObject.activeSelf;
            canvas.gameObject.SetActive(true);
            canvas.overrideSorting = value;
            canvas.gameObject.SetActive(originActive);
        }
    }
}

