using ILRuntime.Runtime.Enviorment;

namespace Fusion.Frameworks.DynamicDLL
{
    public class DLLCustomBinder : DLLBinder
    {
        public DLLCustomBinder(AppDomain appDomain) : base(appDomain)
        {
        }

        public override void RegisterAdapters()
        {
            base.RegisterAdapters();
        }

        public override void RegisterMethodDelegate()
        {
            base.RegisterMethodDelegate();
        }
    }
}

