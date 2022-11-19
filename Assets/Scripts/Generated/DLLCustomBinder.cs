using ILRuntime.Runtime.Enviorment;

namespace Fusion.Frameworks.DynamicDLL
{
    public class DLLCustomBinder : DLLBinder
    {
        public DLLCustomBinder(AppDomain appDomain) : base(appDomain)
        {
        }

        public override void RegisterCLRBinding()
        {
            base.RegisterCLRBinding();
            ILRuntime.Runtime.Generated.CLRBindings.Initialize(appDomain);
        }
    }
}

