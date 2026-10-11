using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace FacioQuo.Stock.Indicators;

/// <summary>
/// Resolves indicator methods and their result record types from a catalog listing.
/// </summary>
/// <remarks>
/// A listing names its method and the static class declaring it. Reflection reaches only
/// that class, through a <see cref="DynamicallyAccessedMembersAttribute"/> annotation the
/// trimmer and NativeAOT compiler honor, so resolution survives a trimmed publish.
/// </remarks>
internal static class CatalogMethodResolver
{
    private const DynamicallyAccessedMemberTypes MethodMembers
        = DynamicallyAccessedMemberTypes.PublicMethods;

    private static readonly ConcurrentDictionary<(Type, string), MethodInfo[]> Overloads = new();

    /// <summary>
    /// Gets every public static overload of a listing's method.
    /// </summary>
    /// <remarks>
    /// A listing built outside the catalog may carry only <see cref="IndicatorListing.MethodName"/>;
    /// its declaring class is then taken from the catalog listing with the same method name.
    /// </remarks>
    /// <param name="listing">Listing naming the method.</param>
    /// <returns>All matching overloads; empty when the method cannot be resolved.</returns>
    internal static MethodInfo[] GetOverloads(IndicatorListing listing)
    {
        if (string.IsNullOrWhiteSpace(listing.MethodName))
        {
            return [];
        }

        Type? declaringType = listing.MethodDeclaringType
            ?? FindCatalogListing(listing.MethodName)?.MethodDeclaringType;

        return declaringType is null
            ? []
            : GetOverloads(declaringType, listing.MethodName);
    }

    /// <summary>
    /// Gets the name of the result record type produced by a named indicator method.
    /// </summary>
    /// <param name="declaringType">Static class declaring the method.</param>
    /// <param name="methodName">Method name from a catalog listing.</param>
    /// <returns>
    /// Result record type name, or <c>null</c> when the method does not exist or its
    /// result record cannot be determined.
    /// </returns>
    internal static string? GetResultTypeName(
        [DynamicallyAccessedMembers(MethodMembers)] Type? declaringType,
        string? methodName)
        => declaringType is null || string.IsNullOrWhiteSpace(methodName)
            ? null
            : GetOverloads(declaringType, methodName)
                .Select(GetResultType)
                .FirstOrDefault(static t => t is not null)
                ?.Name;

    /// <summary>
    /// Gets the result record type produced by an indicator method, for any style.
    /// </summary>
    /// <remarks>
    /// Walks base types rather than interfaces: a trimmed app keeps every base type of a
    /// type it keeps, but not necessarily the interface list reflection would enumerate.
    /// </remarks>
    /// <param name="method">Indicator method bound to a catalog listing.</param>
    /// <returns>
    /// Result record type — <c>EmaResult</c> for all three of
    /// <c>IReadOnlyList&lt;EmaResult&gt;</c> (Series), <c>EmaList</c> (Buffer),
    /// and <c>EmaHub</c> (Stream) — or <c>null</c> when it cannot be determined.
    /// </returns>
    internal static Type? GetResultType(MethodInfo method)
    {
        Type returnType = method.ReturnType;

        // Series: IReadOnlyList<TResult>
        if (returnType.IsGenericType
         && returnType.GetGenericTypeDefinition() == typeof(IReadOnlyList<>))
        {
            return returnType.GetGenericArguments()[0];
        }

        for (Type? baseType = returnType; baseType is not null; baseType = baseType.BaseType)
        {
            if (!baseType.IsGenericType)
            {
                continue;
            }

            Type definition = baseType.GetGenericTypeDefinition();

            // Buffer: BufferList<TResult>
            if (definition == typeof(BufferList<>))
            {
                return baseType.GetGenericArguments()[0];
            }

            // Stream: StreamHub<TIn, TOut>
            if (definition == typeof(StreamHub<,>))
            {
                return baseType.GetGenericArguments()[1];
            }
        }

        return null;
    }

    private static IndicatorListing? FindCatalogListing(string methodName)
        => Catalog.Listings.FirstOrDefault(
            l => string.Equals(l.MethodName, methodName, StringComparison.Ordinal));

    private static MethodInfo[] GetOverloads(
        [DynamicallyAccessedMembers(MethodMembers)] Type declaringType,
        string methodName)
    {
        if (Overloads.TryGetValue((declaringType, methodName), out MethodInfo[]? overloads))
        {
            return overloads;
        }

        overloads = GetStaticMethods(declaringType)
            .Where(m => string.Equals(m.Name, methodName, StringComparison.Ordinal))
            .ToArray();

        // only names that resolve are cached, so caller-supplied names cannot grow the cache
        if (overloads.Length > 0)
        {
            Overloads[(declaringType, methodName)] = overloads;
        }

        return overloads;
    }

    /// <summary>
    /// Gets a type's own public static methods, tolerating a type-load failure.
    /// </summary>
    /// <remarks>
    /// Listings are built inside each indicator class's static initializer, so a throw
    /// here would leave that type permanently unusable — taking the calculations down
    /// with the metadata. <c>DeclaredOnly</c> keeps inherited <see cref="object"/>
    /// statics such as <c>ReferenceEquals</c> out of reach of a listing's method name.
    /// </remarks>
    /// <param name="type">Static class to enumerate.</param>
    /// <returns>The type's own public static methods, or empty when it cannot be read.</returns>
    private static MethodInfo[] GetStaticMethods(
        [DynamicallyAccessedMembers(MethodMembers)] Type type)
    {
        try
        {
            return type.GetMethods(
                BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
        }
        catch (Exception ex) when (ex is TypeLoadException or ReflectionTypeLoadException)
        {
            return [];
        }
    }
}
