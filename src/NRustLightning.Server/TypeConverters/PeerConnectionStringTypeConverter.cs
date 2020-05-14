using System;
using System.ComponentModel;

namespace NRustLightning.Server.TypeConverters
{
    public class PeerConnectionStringTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return base.CanConvertFrom(context, sourceType);
        }
    }
}