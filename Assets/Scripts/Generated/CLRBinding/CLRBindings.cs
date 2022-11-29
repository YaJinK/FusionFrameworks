using System;
using System.Collections.Generic;
using System.Reflection;
#if DEBUG && !DISABLE_ILRUNTIME_DEBUG
using AutoList = System.Collections.Generic.List<object>;
#else
using AutoList = ILRuntime.Other.UncheckedList<object>;
#endif
namespace ILRuntime.Runtime.Generated
{
    class CLRBindings
    {

//will auto register in unity
#if UNITY_5_3_OR_NEWER
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif
        static private void RegisterBindingAction()
        {
            ILRuntime.Runtime.CLRBinding.CLRBindingUtils.RegisterBindingAction(Initialize);
        }


        /// <summary>
        /// Initialize the CLR binding, please invoke this AFTER CLR Redirection registration
        /// </summary>
        public static void Initialize(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            Fusion_Frameworks_DynamicDLL_Mono_DLLMonoBase_Binding.Register(app);
            UnityEngine_Debug_Binding.Register(app);
            UnityEngine_MonoBehaviour_Binding.Register(app);
            UnityEngine_WaitForSeconds_Binding.Register(app);
            System_NotSupportedException_Binding.Register(app);
            Fusion_Frameworks_UI_UIObject_Binding.Register(app);
            Fusion_Frameworks_UI_UIUtility_Binding.Register(app);
            Fusion_Frameworks_UI_UIManager_Binding.Register(app);
            Fusion_Frameworks_Assets_AssetsUtility_Binding.Register(app);
            Fusion_Frameworks_UI_UIData_Binding.Register(app);
            UnityEngine_GameObject_Binding.Register(app);
            Fusion_Frameworks_Scenes_ScenesManager_Binding_LoadAsyncTask_Binding.Register(app);
            Fusion_Frameworks_Scenes_SceneUIHandler_Binding.Register(app);
            Fusion_Frameworks_Scenes_TransformData_Binding.Register(app);
            Fusion_Frameworks_Scenes_ScenesManager_Binding.Register(app);
            UnityEngine_Vector3_Binding.Register(app);
            UnityEngine_Transform_Binding.Register(app);
            System_Int32_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_ILTypeInstance_Binding.Register(app);
            Fusion_Frameworks_Assets_AssetsManager_Binding.Register(app);
            UnityEngine_TextAsset_Binding.Register(app);
            LitJson_JsonMapper_Binding.Register(app);
        }

        /// <summary>
        /// Release the CLR binding, please invoke this BEFORE ILRuntime Appdomain destroy
        /// </summary>
        public static void Shutdown(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
        }
    }
}
