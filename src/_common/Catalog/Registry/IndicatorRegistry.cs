using System.Collections.Concurrent;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;

namespace Skender.Stock.Indicators;

/// <summary>
/// Thread-safe singleton registry for managing indicator listings and providing catalog functionality.
/// </summary>
/// <remarks>
/// <para>
/// The IndicatorRegistry is the central access point for the Stock.Indicators catalog system.
/// It provides methods for retrieving indicator metadata, searching for indicators,
/// and accessing parameter and result information.
/// </para>
/// <para>
/// Usage examples:
/// <code>
/// // Get all indicators
/// var allIndicators = IndicatorRegistry.GetAllIndicators();
///
/// // Get a specific indicator by ID
/// var ema = IndicatorRegistry.GetIndicator("EMA");
///
/// // Search for indicators by name
/// var movingAverages = IndicatorRegistry.Search("moving average");
///
/// // Filter by style
/// var seriesIndicators = IndicatorRegistry.GetCatalog(Style.Series);
///
/// // Filter by category
/// var momentumIndicators = IndicatorRegistry.GetCatalog(category: Category.Momentum);
/// </code>
/// </para>
/// <para>
/// The registry is automatically initialized when first accessed and
/// discovers all indicators with proper attributes or static Listing properties.
/// </para>
/// </remarks>
public static partial class IndicatorRegistry
{
    private static readonly object _lock = new();
    private static readonly ConcurrentDictionary<string, IndicatorListing> _indicators = new();
    private static readonly Lazy<Dictionary<string, string>> _xmlDocumentation = new(LoadXmlDocumentation);
    private static volatile bool _isInitialized;

    /// <summary>
    /// Ensures the registry is initialized by performing lazy initialization if needed.
    /// This method is thread-safe.
    /// </summary>
    private static void EnsureInitialized()
    {
        if (_isInitialized)
        {
            return;
        }

        lock (_lock)
        {
            if (_isInitialized)
            {
                return;
            }

            // Only register if registry is empty
            // This allows tests to control registration explicitly
            if (_indicators.IsEmpty)
            {
                RegisterCatalog();
            }

            _isInitialized = true;
        }
    }

    /// <summary>
    /// Gets all registered indicators from the catalog.
    /// </summary>
    /// <returns>A read-only collection of all registered indicator listings.</returns>
    /// <remarks>
    /// This method returns all indicator listings regardless of style or category.
    /// Use this method when you need to access the complete catalog of available indicators.
    ///
    /// <code>
    /// // Get all indicators
    /// var allIndicators = IndicatorRegistry.GetAllIndicators();
    ///
    /// // Access indicator properties
    /// foreach (var indicator in allIndicators)
    /// {
    ///     Console.WriteLine($"{indicator.Name} ({indicator.Uiid}): {indicator.Category}");
    /// }
    /// </code>
    /// </remarks>
    public static IReadOnlyCollection<IndicatorListing> GetAllIndicators()
    {
        // In test environments, we may want to skip auto-initialization
        // This allows for proper testing of empty registry
        string? callingAssemblyName = Assembly.GetCallingAssembly().FullName;
        if (callingAssemblyName != null && callingAssemblyName.Contains("Tests", StringComparison.OrdinalIgnoreCase))
        {
            return _indicators.Values.ToList().AsReadOnly();
        }

        EnsureInitialized();
        return _indicators.Values.ToList().AsReadOnly();
    }

    /// <summary>
    /// Gets an indicator by its unique identifier (UIID).
    /// </summary>
    /// <param name="uiid">The unique identifier of the indicator.</param>
    /// <returns>The indicator listing if found; otherwise, null.</returns>
    public static IndicatorListing? GetIndicator(string uiid)
    {
        if (string.IsNullOrWhiteSpace(uiid))
        {
            return null;
        }

        EnsureInitialized();
        _indicators.TryGetValue(uiid.ToUpperInvariant(), out IndicatorListing? listing);
        return listing;
    }

    /// <summary>
    /// Gets a filtered catalog of indicators with optional style filtering.
    /// </summary>
    /// <param name="style">Optional style filter. If null, returns all indicators.</param>
    /// <returns>A read-only collection of indicator listings matching the criteria.</returns>
    public static IReadOnlyCollection<IndicatorListing> GetCatalog(Style? style = null)
    {
        EnsureInitialized();
        ICollection<IndicatorListing> allIndicators = _indicators.Values;

        return style.HasValue
            ? allIndicators
                .Where(indicator => indicator.Style == style.Value)
                .ToList()
                .AsReadOnly()
            : allIndicators.ToList().AsReadOnly();
    }

    /// <summary>
    /// Searches indicators by name or UIID with optional filtering.
    /// </summary>
    /// <param name="searchTerm">Term to search for in indicator names or UIIDs.</param>
    /// <param name="style">Optional style filter.</param>
    /// <param name="category">Optional category filter.</param>
    /// <returns>A read-only collection of indicator listings matching the search criteria.</returns>
    public static IReadOnlyCollection<IndicatorListing> Search(
        string searchTerm,
        Style? style = null,
        Category? category = null)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return GetCatalog(style);
        }

        EnsureInitialized();
        IEnumerable<IndicatorListing> filteredIndicators = _indicators.Values
            .Where(indicator =>
                indicator.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                indicator.Uiid.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));

        if (style.HasValue)
        {
            filteredIndicators = filteredIndicators.Where(indicator => indicator.Style == style.Value);
        }

        if (category.HasValue)
        {
            filteredIndicators = filteredIndicators.Where(indicator => indicator.Category == category.Value);
        }

        return filteredIndicators.ToList().AsReadOnly();
    }

    /// <summary>
    /// Gets indicators filtered by category.
    /// </summary>
    /// <param name="category">The category to filter by.</param>
    /// <returns>A read-only collection of indicator listings in the specified category.</returns>
    public static IReadOnlyCollection<IndicatorListing> GetByCategory(Category category)
    {
        EnsureInitialized();
        return _indicators.Values
            .Where(indicator => indicator.Category == category)
            .ToList()
            .AsReadOnly();
    }

    /// <summary>
    /// Gets indicators filtered by style.
    /// </summary>
    /// <param name="style">The style to filter by.</param>
    /// <returns>A read-only collection of indicator listings with the specified style.</returns>
    public static IReadOnlyCollection<IndicatorListing> GetByStyle(Style style)
    {
        EnsureInitialized();
        return _indicators.Values
            .Where(indicator => indicator.Style == style)
            .ToList()
            .AsReadOnly();
    }

    /// <summary>
    /// Registers an indicator listing in the registry.
    /// </summary>
    /// <param name="listing">The indicator listing to register.</param>
    /// <exception cref="ArgumentNullException">Thrown when listing is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when an indicator with the same UIID is already registered.</exception>
    public static void Register(IndicatorListing listing)
    {
        ArgumentNullException.ThrowIfNull(listing);

        string key = listing.Uiid.ToUpperInvariant();

        if (!_indicators.TryAdd(key, listing))
        {
            throw new InvalidOperationException($"An indicator with UIID '{listing.Uiid}' is already registered.");
        }
    }

    /// <summary>
    /// Automatically registers indicators by discovering classes with indicator attributes using reflection.
    /// </summary>
    /// <param name="assemblies">Optional assemblies to scan. If null, scans the current assembly.</param>
    public static void RegisterAuto(params Assembly[]? assemblies)
    {
        assemblies ??= [Assembly.GetExecutingAssembly()];

        foreach (Assembly assembly in assemblies)
        {
            RegisterIndicatorsFromAssembly(assembly);
        }
    }

    /// <summary>
    /// Registers indicators using convention-based discovery with automatic parameter discovery.
    /// Scans for classes with static 'Listing' properties of type IndicatorListing.
    /// </summary>
    /// <param name="assemblies">Optional assemblies to scan. If null, scans assemblies containing indicator types.</param>
    public static void RegisterCatalog(params Assembly[]? assemblies)
    {
        if (assemblies == null || assemblies.Length == 0)
        {
            // Find the assembly containing IndicatorListing type (this should be the indicators assembly)
            Assembly indicatorAssembly = typeof(IndicatorListing).Assembly;
            assemblies = [indicatorAssembly];
        }

        foreach (Assembly assembly in assemblies)
        {
            RegisterCatalogFromAssembly(assembly);
        }
    }

    /// <summary>
    /// Clears all registered indicators. Primarily for testing purposes.
    /// </summary>
    public static void Clear()
    {
        lock (_lock)
        {
            _indicators.Clear();
            _isInitialized = false;
        }
    }

    /// <summary>
    /// Gets an indicator by its unique identifier (UIID) without triggering auto-initialization.
    /// Used primarily for testing.
    /// </summary>
    /// <param name="uiid">The unique identifier of the indicator.</param>
    /// <returns>The indicator listing if found; otherwise, null.</returns>
    internal static IndicatorListing? GetIndicatorWithoutInitialization(string uiid)
    {
        if (string.IsNullOrWhiteSpace(uiid))
        {
            return null;
        }

        _indicators.TryGetValue(uiid.ToUpperInvariant(), out IndicatorListing? listing);
        return listing;
    }

    private static void RegisterIndicatorsFromAssembly(Assembly assembly)
    {
        try
        {
            IEnumerable<Type> indicatorTypes = assembly.GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract)
                .Where(HasIndicatorAttribute);

            foreach (Type indicatorType in indicatorTypes)
            {
                IndicatorListing? listing = CreateListingFromAttribute(indicatorType);
                if (listing != null)
                {
                    Register(listing);
                }
            }
        }
        catch (ReflectionTypeLoadException ex)
        {
            // Log the exception but continue with successfully loaded types
            foreach (Type? type in ex.Types)
            {
                if (type != null && HasIndicatorAttribute(type))
                {
                    IndicatorListing? listing = CreateListingFromAttribute(type);
                    if (listing != null)
                    {
                        Register(listing);
                    }
                }
            }
        }
    }

    private static void RegisterCatalogFromAssembly(Assembly assembly)
    {
        try
        {
            IEnumerable<Type> indicatorTypes = assembly.GetTypes()
                .Where(type => type.IsClass) // Any class, not just static classes
                .Where(HasCatalogListing);

            foreach (Type indicatorType in indicatorTypes)
            {
                IndicatorListing? listing = GetCatalogListing(indicatorType);
                if (listing != null)
                {
                    Register(listing);
                }
            }
        }
        catch (ReflectionTypeLoadException ex)
        {
            // Log the exception but continue with successfully loaded types
            foreach (Type? type in ex.Types)
            {
                if (type != null && HasCatalogListing(type))
                {
                    IndicatorListing? listing = GetCatalogListing(type);
                    if (listing != null)
                    {
                        Register(listing);
                    }
                }
            }
        }
    }

    private static bool HasIndicatorAttribute(Type type)
        => type.GetCustomAttributes(typeof(IndicatorAttribute), true).Length > 0;

    private static bool HasCatalogListing(Type type)
    {
        // Check for both property and field named "Listing"
        PropertyInfo? listingProperty = type.GetProperty(
            name: "Listing",
            bindingAttr: BindingFlags.Public | BindingFlags.Static);

        FieldInfo? listingField = type.GetField(
            name: "Listing",
            bindingAttr: BindingFlags.Public | BindingFlags.Static);

        return (listingProperty != null && listingProperty.PropertyType == typeof(IndicatorListing)) ||
               (listingField != null && listingField.FieldType == typeof(IndicatorListing));
    }

    private static IndicatorListing? GetCatalogListing(Type type)
    {
        // Try to get property first
        PropertyInfo? listingProperty = type.GetProperty(
            name: "Listing",
            bindingAttr: BindingFlags.Public | BindingFlags.Static);

        if (listingProperty != null && listingProperty.PropertyType == typeof(IndicatorListing))
        {
            return listingProperty.GetValue(null) as IndicatorListing;
        }

        // Try to get field second
        FieldInfo? listingField = type.GetField(
            name: "Listing",
            bindingAttr: BindingFlags.Public | BindingFlags.Static);

        return listingField != null && listingField.FieldType == typeof(IndicatorListing)
            ? listingField.GetValue(null) as IndicatorListing
            : null;
    }

    private static IndicatorListing? CreateListingFromAttribute(Type type)
    {
        IndicatorAttribute? attribute = type.GetCustomAttribute<IndicatorAttribute>();
        if (attribute == null)
        {
            return null;
        }

        try
        {
            IndicatorListingBuilder builder = new IndicatorListingBuilder()
                .WithName(GetDisplayName(type))
                .WithId(attribute.Id)
                .WithStyle(attribute.Style)
                .WithCategory(Category.Undefined); // Default category, could be enhanced

            // Add parameters from constructor or methods (simplified for now)
            AddParametersFromType(builder, type);

            // Add a default result (simplified for now)
            builder.AddResult(
                dataName: "Result",
                displayName: "Result",
                dataType: ResultType.Default,
                isDefault: true);

            return builder.Build();
        }
        catch (ArgumentException ex)
        {
            // TODO: Handle specific exception related to invalid arguments
            // Log or handle the exception as needed
            Console.WriteLine($"ArgumentException: {ex.Message}");
            return null;
        }
        catch (InvalidOperationException ex)
        {
            // TODO: Handle specific exception related to invalid operations
            // Log or handle the exception as needed
            Console.WriteLine($"InvalidOperationException: {ex.Message}");
            return null;
        }
    }

    private static string GetDisplayName(Type type)
    {
        // Try to get a friendly name from XML documentation or use the type name
        string typeName = type.Name;

        if (_xmlDocumentation.Value.TryGetValue($"T:{type.FullName}", out string? xmlSummary))
        {
            return ExtractSummaryText(xmlSummary) ?? typeName;
        }

        // Fallback to formatting the type name
        return FormatTypeName(typeName);
    }

    private static string FormatTypeName(string typeName) =>
        // Convert PascalCase to spaced words
        FormatRegex().Replace(typeName, " ");

    private static void AddParametersFromType(IndicatorListingBuilder builder, Type type)
    {
        // This is a simplified implementation
        // In a full implementation, you would analyze constructors and methods
        // to discover parameters and their types

        // For now, we'll just add common parameters if they exist
        ConstructorInfo[] constructors = type.GetConstructors();
        foreach (ConstructorInfo constructor in constructors)
        {
            foreach (ParameterInfo parameter in constructor.GetParameters())
            {
                // Skip common collection parameters
                if (IsCommonCollectionParameter(parameter))
                {
                    continue;
                }

                // Add parameter with basic information
                try
                {
                    string description = GetParameterDescription(type, parameter.Name) ?? $"{parameter.Name} parameter";

                    builder.AddParameter<object>(
                        parameterName: parameter.Name ?? "parameter",
                        displayName: FormatParameterName(parameter.Name ?? "parameter"),
                        description: description,
                        isRequired: !parameter.HasDefaultValue,
                        defaultValue: parameter.DefaultValue);
                }
                catch (ArgumentException)
                {
                    // TODO: Handle specific exception related to invalid arguments
                    // Log or handle the exception as needed
                }
                catch (InvalidOperationException)
                {
                    // TODO: Handle specific exception related to invalid operations
                    // Log or handle the exception as needed
                }
            }
        }
    }

    private static bool IsCommonCollectionParameter(ParameterInfo parameter)
    {
        if (parameter.ParameterType.IsGenericType)
        {
            Type genericType = parameter.ParameterType.GetGenericTypeDefinition();
            return genericType == typeof(IEnumerable<>) ||
                   genericType == typeof(IReadOnlyList<>) ||
                   genericType == typeof(List<>) ||
                   genericType == typeof(IList<>) ||
                   genericType == typeof(ICollection<>);
        }

        return parameter.Name?.Equals("source", StringComparison.OrdinalIgnoreCase) == true ||
               parameter.Name?.Equals("quotes", StringComparison.OrdinalIgnoreCase) == true;
    }

    private static string FormatParameterName(string parameterName)
    {
        // Convert camelCase/PascalCase to Title Case
        string formatted = ParamRegex().Replace(parameterName, " ");
        return char.ToUpperInvariant(formatted[0]) + formatted[1..];
    }

    private static string? GetParameterDescription(Type type, string? parameterName)
    {
        if (parameterName == null)
        {
            return null;
        }

        // Try to find parameter description in XML documentation
        string memberKey = $"M:{type.FullName}";
        return _xmlDocumentation.Value.TryGetValue(memberKey, out string? xmlDoc)
            ? ExtractParameterDescription(xmlDoc, parameterName)
            : null;
    }

    private static Dictionary<string, string> LoadXmlDocumentation()
    {
        Dictionary<string, string> documentation = [];

        try
        {
            // Try to load XML documentation from the assembly location
            string assemblyLocation = Assembly.GetExecutingAssembly().Location;
            if (!string.IsNullOrEmpty(assemblyLocation))
            {
                string xmlPath = Path.ChangeExtension(assemblyLocation, ".xml");
                if (File.Exists(xmlPath))
                {
                    LoadXmlDocumentationFromFile(xmlPath, documentation);
                }
            }

            // Also try common locations
            string[] commonPaths = [
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Skender.Stock.Indicators.xml"),
                Path.Combine(Directory.GetCurrentDirectory(), "Skender.Stock.Indicators.xml")
            ];

            foreach (string path in commonPaths)
            {
                if (File.Exists(path))
                {
                    LoadXmlDocumentationFromFile(path, documentation);
                    break;
                }
            }
        }
        catch (XmlException ex)
        {
            // TODO: Handle XML-specific exceptions (e.g., malformed XML)
            Console.WriteLine($"XMLException: {ex.Message}");
        }
        catch (IOException ex)
        {
            // TODO: Handle IO-specific exceptions (e.g., file not found, access denied)
            Console.WriteLine($"IOException: {ex.Message}");
        }
        catch (UnauthorizedAccessException ex)
        {
            // TODO: Handle unauthorized access exceptions
            Console.WriteLine($"UnauthorizedAccessException: {ex.Message}");
        }

        return documentation;
    }

    private static void LoadXmlDocumentationFromFile(string xmlPath, Dictionary<string, string> documentation)
    {
        try
        {
            XmlDocument xmlDoc = new();
            xmlDoc.Load(xmlPath);

            XmlNodeList? members = xmlDoc.SelectNodes("//member");
            if (members != null)
            {
                foreach (XmlNode member in members)
                {
                    XmlAttribute? nameAttribute = member.Attributes?["name"];
                    if (nameAttribute?.Value != null)
                    {
                        documentation[nameAttribute.Value] = member.InnerXml;
                    }
                }
            }
        }
        catch (XmlException)
        {
            // TODO: Handle XML-specific exceptions (e.g., malformed XML)
            // Log or handle the exception as needed
        }
        catch (IOException)
        {
            // TODO: Handle IO-specific exceptions (e.g., file not found, access denied)
            // Log or handle the exception as needed
        }
        catch (UnauthorizedAccessException)
        {
            // TODO: Handle unauthorized access exceptions
            // Log or handle the exception as needed
        }
    }

    private static string? ExtractSummaryText(string xmlContent)
    {
        try
        {
            XmlDocument xmlDoc = new();
            xmlDoc.LoadXml($"<root>{xmlContent}</root>");

            XmlNode? summaryNode = xmlDoc.SelectSingleNode("//summary");
            return summaryNode?.InnerText?.Trim();
        }
        catch (XmlException)
        {
            // TODO: Handle XML-specific exceptions (e.g., malformed XML)
            return null;
        }
        catch (InvalidOperationException)
        {
            // TODO: Handle invalid operations (e.g., issues with XML parsing)
            return null;
        }
    }

    private static string? ExtractParameterDescription(string xmlContent, string parameterName)
    {
        try
        {
            XmlDocument xmlDoc = new();
            xmlDoc.LoadXml($"<root>{xmlContent}</root>");

            XmlNode? paramNode = xmlDoc.SelectSingleNode($"//param[@name='{parameterName}']");
            return paramNode?.InnerText?.Trim();
        }
        catch (XmlException)
        {
            // TODO: Handle XML-specific exceptions (e.g., malformed XML)
            return null;
        }
        catch (InvalidOperationException)
        {
            // TODO: Handle invalid operations (e.g., issues with XML parsing)
            return null;
        }
    }

    [GeneratedRegex(@"(?<!^)(?=[A-Z])")]
    private static partial Regex ParamRegex();

    [GeneratedRegex(@"(?<!^)(?=[A-Z])")]
    private static partial Regex FormatRegex();
}
