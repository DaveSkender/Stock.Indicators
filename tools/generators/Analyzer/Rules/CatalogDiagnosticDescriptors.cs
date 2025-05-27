namespace Generators.Analyzer.Rules;

/// <summary>
/// Contains diagnostic descriptors for the catalog system analyzer.
/// </summary>
internal static class CatalogDiagnosticDescriptors
{
    // Common categories
    private const string CatalogCategory = "Catalog";

    // SID001: Missing Indicator Listing
    private const string SID001_RuleId = "SID001";
    private const string SID001_Title = "Missing Indicator Listing";
    private const string SID001_MessageFormat = "Class '{0}' has indicator attributes but no Listing property";
    private const string SID001_Description =
        "Indicator classes with attributes should have a static Listing property to define their catalog metadata.";

    public static readonly DiagnosticDescriptor SID001_MissingListingDescriptor = new(
        id: SID001_RuleId,
        title: SID001_Title,
        messageFormat: SID001_MessageFormat,
        category: CatalogCategory,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: SID001_Description);

    // SID002: Missing Parameters
    private const string SID002_RuleId = "SID002";
    private const string SID002_Title = "Missing Parameter in Listing";
    private const string SID002_MessageFormat = "Parameter '{0}' in method '{1}' is missing from the indicator listing";
    private const string SID002_Description =
        "All parameters in indicator methods/constructors should be defined in the indicator listing.";

    public static readonly DiagnosticDescriptor SID002_MissingParameterDescriptor = new(
        id: SID002_RuleId,
        title: SID002_Title,
        messageFormat: SID002_MessageFormat,
        category: CatalogCategory,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: SID002_Description);

    // SID003: Extraneous Parameters
    private const string SID003_RuleId = "SID003";
    private const string SID003_Title = "Extraneous Parameter in Listing";
    private const string SID003_MessageFormat = "Parameter '{0}' in the listing is not found in any implementation method";
    private const string SID003_Description =
        "Parameters defined in the indicator listing should exist in the implementation methods/constructors.";

    public static readonly DiagnosticDescriptor SID003_ExtraneousParameterDescriptor = new(
        id: SID003_RuleId,
        title: SID003_Title,
        messageFormat: SID003_MessageFormat,
        category: CatalogCategory,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: SID003_Description);

    // SID004: Parameter Type Mismatch
    private const string SID004_RuleId = "SID004";
    private const string SID004_Title = "Parameter Type Mismatch";
    private const string SID004_MessageFormat = "Parameter '{0}' has type '{1}' in the implementation but '{2}' in the listing";
    private const string SID004_Description =
        "Parameter types in the listing should match the types in the implementation methods/constructors.";

    public static readonly DiagnosticDescriptor SID004_ParameterTypeMismatchDescriptor = new(
        id: SID004_RuleId,
        title: SID004_Title,
        messageFormat: SID004_MessageFormat,
        category: CatalogCategory,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: SID004_Description);

    // SID005: Missing Results
    private const string SID005_RuleId = "SID005";
    private const string SID005_Title = "Missing Result in Listing";
    private const string SID005_MessageFormat = "Result property '{0}' is missing from the indicator listing";
    private const string SID005_Description =
        "All result properties from indicator return types should be defined in the indicator listing.";

    public static readonly DiagnosticDescriptor SID005_MissingResultDescriptor = new(
        id: SID005_RuleId,
        title: SID005_Title,
        messageFormat: SID005_MessageFormat,
        category: CatalogCategory,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: SID005_Description);
}
