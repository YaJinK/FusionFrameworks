using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace Fusion.Frameworks.UI
{
    public class UIUtility
    {
        public static T Find<T>(GameObject gameObject, string path) where T : MonoBehaviour
        {
            Transform childTransform = gameObject.transform.Find(path);
            return childTransform.GetComponent<T>();
        }

        public static GameObject Find(GameObject gameObject, string path)
        {
            Transform childTransform = gameObject.transform.Find(path);
            return childTransform.gameObject;
        }

        public static void RegisterButtonAction(GameObject gameObject, Action callback)
        {
            Button button = gameObject.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(delegate
            {
                if (callback != null)
                {
                    callback();
                }
            });
        }

        public static void SetText(GameObject gameObject, string str)
        {
            TextMeshProUGUI textMeshProUGUI = gameObject.GetComponent<TextMeshProUGUI>();
            textMeshProUGUI.text = str;
        }
    }

}
