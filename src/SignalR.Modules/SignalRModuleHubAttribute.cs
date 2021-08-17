using System;

namespace SignalR.Modules
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class SignalRModuleHubAttribute : Attribute
    {
        public Type ModuleHubType { get; set; }

        public SignalRModuleHubAttribute(Type moduleHubType)
        {
            ModuleHubType = moduleHubType;
        }
    }
}
