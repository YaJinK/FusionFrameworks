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

#if FUSION_DYNAMIC_DLL
                    appDomain = new ILRuntime.Runtime.Enviorment.AppDomain();

                    TextAsset textAsset = AssetsManager.Instance.Load<TextAsset>("Scripts/Extra");
                    MemoryStream memoryStream = new MemoryStream(textAsset.bytes);

                    appDomain.LoadAssembly(memoryStream);
                    LitJson.JsonMapper.RegisterILRuntimeCLRRedirection(appDomain);
                    Type classType = Type.GetType("Fusion.Frameworks.DynamicDLL.DLLCustomBinder, Assembly-CSharp");
                    DLLBinder dllBinder = classType != null ? (DLLBinder)Activator.CreateInstance(classType, appDomain) : new DLLBinder(appDomain);
                    dllBinder.Register();
#endif
                }
                return instance;
            }
        }

        public T Instantiate<T>(string className, params object[] objects)
        {
#if FUSION_DYNAMIC_DLL
            Debug.Log(className);
            return appDomain.Instantiate<T>(className, objects);
#else
            Type classType = Type.GetType($"{className}, Assembly-CSharp");
            return classType != null ? (T)Activator.CreateInstance(classType, objects) : default(T);
#endif

        }
    }
}

