namespace DotNetLightning.LDK.Primitives
{
    public struct FFIResult
    {
        private enum Kind : uint
        {
            EmptyPointerProvided,
            InvalidSeedLength,
        }
    }
}