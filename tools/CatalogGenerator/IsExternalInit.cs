// This polyfill allows record types in netstandard2.0
using System.ComponentModel;

namespace System.Runtime.CompilerServices;

/// <summary>
/// Polyfill for the <see cref="IsExternalInit"/> class to enable record types in netstandard2.0.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
internal static class IsExternalInit { }
