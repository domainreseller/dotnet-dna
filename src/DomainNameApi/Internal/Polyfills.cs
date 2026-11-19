#if NETSTANDARD2_0
// Lets the compiler use init-only setters and records on .NET Standard 2.0.
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit
    {
    }
}
#endif
