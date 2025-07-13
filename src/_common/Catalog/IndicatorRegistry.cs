namespace Skender.Stock.Indicators;

/// <summary>
/// Registry for indicator listings.
/// </summary>
/// <remarks>
/// The indicator registry provides a central point for registering and retrieving
/// indicator listings. It supports various query operations based on style, category, etc.
/// All listings must be explicitly defined and registered.
/// </remarks>
public static class IndicatorRegistry
{
    private static readonly List<IndicatorListing> _registry = [];
    private static readonly object _syncLock = new();
    private static volatile bool _isInitialized;

    /// <summary>
    /// Initialize the registry with default indicators from the catalog
    /// </summary>
    private static void EnsureInitialized()
    {
        if (!_isInitialized)
        {
            lock (_syncLock)
            {
                if (!_isInitialized)
                {
                    // Register all indicators from the catalog
                    foreach (IndicatorListing listing in IndicatorCatalog.Catalog)
                    {
                        Register(listing);
                    }

                    _isInitialized = true;
                }
            }
        }
    }

    /// <summary>
    /// Registers an indicator listing in the registry.
    /// </summary>
    /// <param name="listing">The indicator listing to register.</param>
    /// <exception cref="ArgumentNullException">Thrown if listing is null.</exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if another listing with the same ID and style already exists.
    /// </exception>
    public static void Register(IndicatorListing listing)
    {
        ArgumentNullException.ThrowIfNull(listing);

        lock (_syncLock)
        {
            // Check if a listing with the same UIID and style already exists
            if (_registry.Any(x =>
                string.Equals(x.Uiid, listing.Uiid, StringComparison.OrdinalIgnoreCase) &&
                x.Style == listing.Style))
            {
                throw new InvalidOperationException(
                    $"An indicator with ID '{listing.Uiid}' and style '{listing.Style}' is already registered.");
            }

            _registry.Add(listing);
        }
    }

    /// <summary>
    /// Registers all indicators from the catalog.
    /// </summary>
    public static void RegisterCatalog()
    {
        // Clear the registry first to avoid duplicates
        Clear();

        // Register all indicators from the catalog
        foreach (IndicatorListing listing in IndicatorCatalog.Catalog)
        {
            Register(listing);
        }
    }

    /// <summary>
    /// Registers all indicators from the catalog in the specified assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies to search for indicators.</param>
    public static void RegisterCatalog(System.Reflection.Assembly[] assemblies)
    {
        RegisterCatalog();
    }

    /// <summary>
    /// Automatically registers indicators with appropriate attributes.
    /// </summary>
    [Obsolete("RegisterAuto is obsolete. Use RegisterCatalog instead.")]
    public static void RegisterAuto()
    {
        // With the removal of code generation system, this is now just forwarding to RegisterCatalog
        RegisterCatalog();
    }

    /// <summary>
    /// Gets all registered indicators.
    /// </summary>
    /// <returns>A read-only collection of all registered indicator listings.</returns>
    public static IReadOnlyCollection<IndicatorListing> GetAll()
    {
        EnsureInitialized();

        lock (_syncLock)
        {
            return _registry.ToList();
        }
    }

    /// <summary>
    /// Gets all registered indicators.
    /// </summary>
    /// <returns>A read-only collection of all registered indicator listings.</returns>
    public static IReadOnlyCollection<IndicatorListing> GetIndicators() => GetAll();

    /// <summary>
    /// Gets an indicator by its ID.
    /// </summary>
    /// <param name="id">The unique ID of the indicator.</param>
    /// <returns>The first indicator listing with the specified ID, or null if not found.</returns>
    public static IndicatorListing? GetIndicator(string id)
    {
        EnsureInitialized();

        lock (_syncLock)
        {
            return _registry.FirstOrDefault(x => string.Equals(x.Uiid, id, StringComparison.OrdinalIgnoreCase));
        }
    }

    /// <summary>
    /// Gets all registered indicators with the specified ID.
    /// </summary>
    /// <param name="id">The unique ID of the indicator.</param>
    /// <returns>All indicator listings with the specified ID.</returns>
    public static IReadOnlyCollection<IndicatorListing> GetById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return Array.Empty<IndicatorListing>();
        }

        EnsureInitialized();

        lock (_syncLock)
        {
            return _registry
                .Where(x => string.Equals(x.Uiid, id, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }

    /// <summary>
    /// Gets all registered indicators with the specified style.
    /// </summary>
    /// <param name="style">The style of indicators to retrieve.</param>
    /// <returns>All indicator listings with the specified style.</returns>
    public static IReadOnlyCollection<IndicatorListing> GetByStyle(Style style)
    {
        EnsureInitialized();

        lock (_syncLock)
        {
            return _registry
                .Where(x => x.Style == style)
                .ToList();
        }
    }

    /// <summary>
    /// Gets all registered indicators in the specified category.
    /// </summary>
    /// <param name="category">The category of indicators to retrieve.</param>
    /// <returns>All indicator listings in the specified category.</returns>
    public static IReadOnlyCollection<IndicatorListing> GetByCategory(Category category)
    {
        EnsureInitialized();

        lock (_syncLock)
        {
            return _registry
                .Where(x => x.Category == category)
                .ToList();
        }
    }

    /// <summary>
    /// Searches for indicators by name or ID, using a partial match.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <returns>All indicator listings that match the search query.</returns>
    public static IReadOnlyCollection<IndicatorListing> Search(string query)
    {
        EnsureInitialized();

        if (string.IsNullOrWhiteSpace(query))
        {
            return GetAll();
        }

        string normalizedQuery = query.Trim();

        lock (_syncLock)
        {
            return _registry
                .Where(x => x.Uiid.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ||
                           x.Name.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }

    /// <summary>
    /// Clears the registry of all registered indicators.
    /// </summary>
    /// <remarks>
    /// This method is primarily intended for testing purposes.
    /// </remarks>
    public static void Clear()
    {
        lock (_syncLock)
        {
            _registry.Clear();
            _isInitialized = false;
        }
    }

    /// <summary>
    /// Gets the catalog of indicators, optionally filtered by style and/or category.
    /// </summary>
    /// <returns>All indicator listings matching the specified filters.</returns>
    public static IReadOnlyCollection<IndicatorListing> GetCatalog() => GetAll();

    /// <summary>
    /// Gets the catalog of indicators filtered by style.
    /// </summary>
    /// <param name="style">The style of indicators to retrieve.</param>
    /// <returns>All indicator listings with the specified style.</returns>
    public static IReadOnlyCollection<IndicatorListing> GetCatalog(Style style) => GetByStyle(style);

    /// <summary>
    /// Gets the catalog of indicators filtered by category.
    /// </summary>
    /// <param name="category">The category of indicators to retrieve.</param>
    /// <returns>All indicator listings in the specified category.</returns>
    public static IReadOnlyCollection<IndicatorListing> GetCatalog(Category category) => GetByCategory(category);

    /// <summary>
    /// Gets indicator without triggering initialization.
    /// </summary>
    /// <remarks>For testing purposes only.</remarks>
    private static IndicatorListing? GetIndicatorWithoutInitialization(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        lock (_syncLock)
        {
            return _registry.FirstOrDefault(x => string.Equals(x.Uiid, id, StringComparison.OrdinalIgnoreCase));
        }
    }
}
