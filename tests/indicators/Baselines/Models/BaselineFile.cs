namespace Tests.Indicators.Baselines.Models;

/// <summary>
/// Represents a complete baseline file containing metadata and results.
/// </summary>
/// <param name="Metadata">Metadata describing the baseline generation context.</param>
/// <param name="Results">Array of result entries, one per date.</param>
public sealed record BaselineFile(
    BaselineMetadata Metadata,
    IReadOnlyList<BaselineResult> Results);

/// <summary>
/// Metadata for a baseline file.
/// </summary>
/// <param name="IndicatorName">Name of the indicator (e.g., "Sma", "Macd").</param>
/// <param name="ScenarioName">Test scenario identifier (e.g., "Standard").</param>
/// <param name="GeneratedAt">UTC timestamp when baseline was generated.</param>
/// <param name="LibraryVersion">Library version string (e.g., "3.0.0").</param>
/// <param name="WarmupPeriodCount">Number of periods required for indicator warmup.</param>
public sealed record BaselineMetadata(
    string IndicatorName,
    string ScenarioName,
    DateTime GeneratedAt,
    string LibraryVersion,
    int WarmupPeriodCount);

/// <summary>
/// Single result entry in a baseline file.
/// </summary>
/// <param name="Date">Date of the result.</param>
/// <param name="Properties">Dictionary of property names to values (e.g., {"sma": 214.52}).</param>
public sealed record BaselineResult(
    DateTime Date,
    IReadOnlyDictionary<string, double?> Properties);
