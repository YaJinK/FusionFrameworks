using Fusion.Frameworks.DynamicDLL.Adapters;
using ILRuntime.Runtime.Enviorment;
using UnityEngine;

namespace Fusion.Frameworks.DynamicDLL
{
    public class DLLBinder
    {
        protected AppDomain appDomain;
        public DLLBinder(AppDomain appDomain)
        {
            this.appDomain = appDomain;
        }

        public virtual void RegisterAdapters()
        {
            appDomain.RegisterCrossBindingAdaptor(new UIObjectAdapter());
            appDomain.RegisterCrossBindingAdaptor(new UIDataAdapter()); 
            appDomain.RegisterCrossBindingAdaptor(new DLLMonoBaseAdapter()); 
            appDomain.RegisterCrossBindingAdaptor(new CoroutineAdapter());
        }

        public virtual void RegisterMethodDelegate()
        {
            appDomain.DelegateManager.RegisterMethodDelegate<GameObject>();
        }

        public virtual void RegisterCLRBinding()
        {
        }

        public void Register()
        {
            RegisterAdapters();
            RegisterMethodDelegate();
            RegisterCLRBinding();
        }
    }
}

