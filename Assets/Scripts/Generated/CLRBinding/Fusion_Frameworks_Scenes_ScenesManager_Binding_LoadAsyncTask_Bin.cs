using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.Reflection;
using ILRuntime.CLR.Utils;
#if DEBUG && !DISABLE_ILRUNTIME_DEBUG
using AutoList = System.Collections.Generic.List<object>;
#else
using AutoList = ILRuntime.Other.UncheckedList<object>;
#endif
namespace ILRuntime.Runtime.Generated
{
    unsafe class Fusion_Frameworks_Scenes_ScenesManager_Binding_LoadAsyncTask_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Fusion.Frameworks.Scenes.ScenesManager.LoadAsyncTask);
            args = new Type[]{typeof(Fusion.Frameworks.Scenes.SceneDataHandler)};
            method = type.GetMethod("AddSceneDataHandler", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, AddSceneDataHandler_0);
            args = new Type[]{typeof(Fusion.Frameworks.Scenes.TransformData)};
            method = type.GetMethod("set_TransformData", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_TransformData_1);

            field = type.GetField("finishCallback", flag);
            app.RegisterCLRFieldGetter(field, get_finishCallback_0);
            app.RegisterCLRFieldSetter(field, set_finishCallback_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_finishCallback_0, AssignFromStack_finishCallback_0);

            args = new Type[]{typeof(System.String)};
            method = type.GetConstructor(flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Ctor_0);

        }


        static StackObject* AddSceneDataHandler_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Fusion.Frameworks.Scenes.SceneDataHandler @sceneDataHandler = (Fusion.Frameworks.Scenes.SceneDataHandler)typeof(Fusion.Frameworks.Scenes.SceneDataHandler).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Fusion.Frameworks.Scenes.ScenesManager.LoadAsyncTask instance_of_this_method = (Fusion.Frameworks.Scenes.ScenesManager.LoadAsyncTask)typeof(Fusion.Frameworks.Scenes.ScenesManager.LoadAsyncTask).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.AddSceneDataHandler(@sceneDataHandler);

            return __ret;
        }

        static StackObject* set_TransformData_1(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Fusion.Frameworks.Scenes.TransformData @value = (Fusion.Frameworks.Scenes.TransformData)typeof(Fusion.Frameworks.Scenes.TransformData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Fusion.Frameworks.Scenes.ScenesManager.LoadAsyncTask instance_of_this_method = (Fusion.Frameworks.Scenes.ScenesManager.LoadAsyncTask)typeof(Fusion.Frameworks.Scenes.ScenesManager.LoadAsyncTask).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.TransformData = value;

            return __ret;
        }


        static object get_finishCallback_0(ref object o)
        {
            return ((Fusion.Frameworks.Scenes.ScenesManager.LoadAsyncTask)o).finishCallback;
        }

        static StackObject* CopyToStack_finishCallback_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Fusion.Frameworks.Scenes.ScenesManager.LoadAsyncTask)o).finishCallback;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_finishCallback_0(ref object o, object v)
        {
            ((Fusion.Frameworks.Scenes.ScenesManager.LoadAsyncTask)o).finishCallback = (System.Action)v;
        }

        static StackObject* AssignFromStack_finishCallback_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Action @finishCallback = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            ((Fusion.Frameworks.Scenes.ScenesManager.LoadAsyncTask)o).finishCallback = @finishCallback;
            return ptr_of_this_method;
        }


        static StackObject* Ctor_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @singleName = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = new Fusion.Frameworks.Scenes.ScenesManager.LoadAsyncTask(@singleName);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


    }
}
