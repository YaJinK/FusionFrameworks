using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
#if DEBUG && !DISABLE_ILRUNTIME_DEBUG
using AutoList = System.Collections.Generic.List<object>;
#else
using AutoList = ILRuntime.Other.UncheckedList<object>;
#endif

namespace Fusion.Frameworks.DynamicDLL.Adapters
{   
    public class DLLMonoBaseAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(Fusion.Frameworks.DynamicDLL.Mono.DLLMonoBase);
            }
        }

        public override Type AdaptorType
        {
            get
            {
                return typeof(Adapter);
            }
        }

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adapter(appdomain, instance);
        }

        public class Adapter : Fusion.Frameworks.DynamicDLL.Mono.DLLMonoBase, CrossBindingAdaptorType
        {
            CrossBindingMethodInfo mStart_0 = new CrossBindingMethodInfo("Start");
            CrossBindingMethodInfo mUpdate_1 = new CrossBindingMethodInfo("Update");
            CrossBindingMethodInfo mAwake_2 = new CrossBindingMethodInfo("Awake");
            CrossBindingMethodInfo mFixedUpdate_3 = new CrossBindingMethodInfo("FixedUpdate");
            CrossBindingMethodInfo mLateUpdate_4 = new CrossBindingMethodInfo("LateUpdate");
            CrossBindingMethodInfo mOnDestroy_5 = new CrossBindingMethodInfo("OnDestroy");
            CrossBindingMethodInfo mOnEnable_6 = new CrossBindingMethodInfo("OnEnable");
            CrossBindingMethodInfo mOnDisable_7 = new CrossBindingMethodInfo("OnDisable");
            CrossBindingMethodInfo<UnityEngine.Collider> mOnTriggerEnter_8 = new CrossBindingMethodInfo<UnityEngine.Collider>("OnTriggerEnter");
            CrossBindingMethodInfo<UnityEngine.Collider> mOnTriggerExit_9 = new CrossBindingMethodInfo<UnityEngine.Collider>("OnTriggerExit");
            CrossBindingMethodInfo<UnityEngine.Collider> mOnTriggerStay_10 = new CrossBindingMethodInfo<UnityEngine.Collider>("OnTriggerStay");
            CrossBindingMethodInfo<UnityEngine.Collision> mOnCollisionEnter_11 = new CrossBindingMethodInfo<UnityEngine.Collision>("OnCollisionEnter");
            CrossBindingMethodInfo<UnityEngine.Collision> mOnCollisionExit_12 = new CrossBindingMethodInfo<UnityEngine.Collision>("OnCollisionExit");
            CrossBindingMethodInfo<UnityEngine.Collision> mOnCollisionStay_13 = new CrossBindingMethodInfo<UnityEngine.Collision>("OnCollisionStay");
            CrossBindingMethodInfo mOnMouseEnter_14 = new CrossBindingMethodInfo("OnMouseEnter");
            CrossBindingMethodInfo mOnMouseExit_15 = new CrossBindingMethodInfo("OnMouseExit");
            CrossBindingMethodInfo mOnMouseDown_16 = new CrossBindingMethodInfo("OnMouseDown");
            CrossBindingMethodInfo mOnMouseUp_17 = new CrossBindingMethodInfo("OnMouseUp");
            CrossBindingMethodInfo mOnMouseDrag_18 = new CrossBindingMethodInfo("OnMouseDrag");
            CrossBindingMethodInfo mOnMouseOver_19 = new CrossBindingMethodInfo("OnMouseOver");
            CrossBindingMethodInfo mOnMouseUpAsButton_20 = new CrossBindingMethodInfo("OnMouseUpAsButton");

            bool isInvokingToString;
            ILTypeInstance instance;
            ILRuntime.Runtime.Enviorment.AppDomain appdomain;

            public Adapter()
            {

            }

            public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
            }

            public ILTypeInstance ILInstance { get { return instance; } }

            public override void Start()
            {
                if (mStart_0.CheckShouldInvokeBase(this.instance))
                    base.Start();
                else
                    mStart_0.Invoke(this.instance);
            }

            public override void Update()
            {
                if (mUpdate_1.CheckShouldInvokeBase(this.instance))
                    base.Update();
                else
                    mUpdate_1.Invoke(this.instance);
            }

            public override void Awake()
            {
                if (mAwake_2.CheckShouldInvokeBase(this.instance))
                    base.Awake();
                else
                    mAwake_2.Invoke(this.instance);
            }

            public override void FixedUpdate()
            {
                if (mFixedUpdate_3.CheckShouldInvokeBase(this.instance))
                    base.FixedUpdate();
                else
                    mFixedUpdate_3.Invoke(this.instance);
            }

            public override void LateUpdate()
            {
                if (mLateUpdate_4.CheckShouldInvokeBase(this.instance))
                    base.LateUpdate();
                else
                    mLateUpdate_4.Invoke(this.instance);
            }

            public override void OnDestroy()
            {
                if (mOnDestroy_5.CheckShouldInvokeBase(this.instance))
                    base.OnDestroy();
                else
                    mOnDestroy_5.Invoke(this.instance);
            }

            public override void OnEnable()
            {
                if (mOnEnable_6.CheckShouldInvokeBase(this.instance))
                    base.OnEnable();
                else
                    mOnEnable_6.Invoke(this.instance);
            }

            public override void OnDisable()
            {
                if (mOnDisable_7.CheckShouldInvokeBase(this.instance))
                    base.OnDisable();
                else
                    mOnDisable_7.Invoke(this.instance);
            }

            public override void OnTriggerEnter(UnityEngine.Collider collider)
            {
                if (mOnTriggerEnter_8.CheckShouldInvokeBase(this.instance))
                    base.OnTriggerEnter(collider);
                else
                    mOnTriggerEnter_8.Invoke(this.instance, collider);
            }

            public override void OnTriggerExit(UnityEngine.Collider collider)
            {
                if (mOnTriggerExit_9.CheckShouldInvokeBase(this.instance))
                    base.OnTriggerExit(collider);
                else
                    mOnTriggerExit_9.Invoke(this.instance, collider);
            }

            public override void OnTriggerStay(UnityEngine.Collider collider)
            {
                if (mOnTriggerStay_10.CheckShouldInvokeBase(this.instance))
                    base.OnTriggerStay(collider);
                else
                    mOnTriggerStay_10.Invoke(this.instance, collider);
            }

            public override void OnCollisionEnter(UnityEngine.Collision collision)
            {
                if (mOnCollisionEnter_11.CheckShouldInvokeBase(this.instance))
                    base.OnCollisionEnter(collision);
                else
                    mOnCollisionEnter_11.Invoke(this.instance, collision);
            }

            public override void OnCollisionExit(UnityEngine.Collision collision)
            {
                if (mOnCollisionExit_12.CheckShouldInvokeBase(this.instance))
                    base.OnCollisionExit(collision);
                else
                    mOnCollisionExit_12.Invoke(this.instance, collision);
            }

            public override void OnCollisionStay(UnityEngine.Collision collision)
            {
                if (mOnCollisionStay_13.CheckShouldInvokeBase(this.instance))
                    base.OnCollisionStay(collision);
                else
                    mOnCollisionStay_13.Invoke(this.instance, collision);
            }

            public override void OnMouseEnter()
            {
                if (mOnMouseEnter_14.CheckShouldInvokeBase(this.instance))
                    base.OnMouseEnter();
                else
                    mOnMouseEnter_14.Invoke(this.instance);
            }

            public override void OnMouseExit()
            {
                if (mOnMouseExit_15.CheckShouldInvokeBase(this.instance))
                    base.OnMouseExit();
                else
                    mOnMouseExit_15.Invoke(this.instance);
            }

            public override void OnMouseDown()
            {
                if (mOnMouseDown_16.CheckShouldInvokeBase(this.instance))
                    base.OnMouseDown();
                else
                    mOnMouseDown_16.Invoke(this.instance);
            }

            public override void OnMouseUp()
            {
                if (mOnMouseUp_17.CheckShouldInvokeBase(this.instance))
                    base.OnMouseUp();
                else
                    mOnMouseUp_17.Invoke(this.instance);
            }

            public override void OnMouseDrag()
            {
                if (mOnMouseDrag_18.CheckShouldInvokeBase(this.instance))
                    base.OnMouseDrag();
                else
                    mOnMouseDrag_18.Invoke(this.instance);
            }

            public override void OnMouseOver()
            {
                if (mOnMouseOver_19.CheckShouldInvokeBase(this.instance))
                    base.OnMouseOver();
                else
                    mOnMouseOver_19.Invoke(this.instance);
            }

            public override void OnMouseUpAsButton()
            {
                if (mOnMouseUpAsButton_20.CheckShouldInvokeBase(this.instance))
                    base.OnMouseUpAsButton();
                else
                    mOnMouseUpAsButton_20.Invoke(this.instance);
            }

            public override string ToString()
            {
                IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
                m = instance.Type.GetVirtualMethod(m);
                if (m == null || m is ILMethod)
                {
                    if (!isInvokingToString)
                    {
                        isInvokingToString = true;
                        string res = instance.ToString();
                        isInvokingToString = false;
                        return res;
                    }
                    else
                        return instance.Type.FullName;
                }
                else
                    return instance.Type.FullName;
            }
        }
    }
}
