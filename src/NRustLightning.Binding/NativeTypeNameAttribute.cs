using System;
using System.Diagnostics;

namespace NRustLightning.Binding
{
    [Conditional("DEBUG")]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.Enum, AllowMultiple = false, Inherited = true)]
    internal sealed class NativeTypeNameAttribute : Attribute
    {
        public NativeTypeNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}