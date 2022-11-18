using UnityEngine;
using Fusion.Frameworks.Assets;
using System.IO;
using System;

namespace Fusion.Frameworks.DynamicDLL
{
    public class DLLManager : MonoBehaviour
    {
        private static DLLManager instance = null;
        private static ILRuntime.Runtime.Enviorment.AppDomain appDomain;

        public static DLLManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject dllManagerObject = new GameObject("DLLManager");
                    instance = dllManagerObject.AddComponent<DLLManager>();
                    DontDestroyOnLoad(dllManagerObject);

                    appDomain = new ILRuntime.Runtime.Enviorment.AppDomain();

                    TextAsset textAsset = AssetsManager.Instance.Load<TextAsset>("Scripts/Extra");
                    MemoryStream memoryStream = new MemoryStream(textAsset.bytes);

                    appDomain.LoadAssembly(memoryStream);

                    Type classType = Type.GetType("Fusion.Frameworks.DynamicDLL.DLLCustomBinder, Assembly-CSharp");
                    DLLBinder dllBinder = classType != null ? (DLLBinder)Activator.CreateInstance(classType, appDomain) : new DLLBinder(appDomain);
                    dllBinder.Register();
                }
                return instance;
            }
        }

        public T Instantiate<T>(string type, params object[] objects)
        {
            return appDomain.Instantiate<T>(type, objects);
        }
    }
}

