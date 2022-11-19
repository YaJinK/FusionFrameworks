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
    unsafe class Fusion_Frameworks_Scenes_TransformData_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Fusion.Frameworks.Scenes.TransformData);

            field = type.GetField("info", flag);
            app.RegisterCLRFieldGetter(field, get_info_0);
            app.RegisterCLRFieldSetter(field, set_info_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_info_0, AssignFromStack_info_0);

            args = new Type[]{};
            method = type.GetConstructor(flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Ctor_0);

        }



        static object get_info_0(ref object o)
        {
            return ((Fusion.Frameworks.Scenes.TransformData)o).info;
        }

        static StackObject* CopyToStack_info_0(ref object o, ILIntepreter __intp, StackObject* __ret, AutoList __mStack)
        {
            var result_of_this_method = ((Fusion.Frameworks.Scenes.TransformData)o).info;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_info_0(ref object o, object v)
        {
            ((Fusion.Frameworks.Scenes.TransformData)o).info = (System.String)v;
        }

        static StackObject* AssignFromStack_info_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, AutoList __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @info = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            ((Fusion.Frameworks.Scenes.TransformData)o).info = @info;
            return ptr_of_this_method;
        }


        static StackObject* Ctor_0(ILIntepreter __intp, StackObject* __esp, AutoList __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);

            var result_of_this_method = new Fusion.Frameworks.Scenes.TransformData();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


    }
}
