using System;
using System.Collections.Generic;

namespace Skender.Stock.Indicators;

/// <summary>
/// Builder for creating CompositeIndicatorListing instances using a fluent interface.
/// </summary>
/// <remarks>
/// This class is obsolete. Use standard IndicatorListingBuilder to create separate listings for each style
/// (Series, Stream, Buffer) instead of creating a composite listing.
/// See OneListingPerStyleGuide.md for guidance on the one-listing-per-style approach.
/// </remarks>
[Obsolete("Use standard IndicatorListingBuilder to create separate listings for each style (Series, Stream, Buffer) instead of creating a composite listing. See OneListingPerStyleGuide.md for guidance.")]
public class CompositeIndicatorListingBuilder : IndicatorListingBuilder
{
    private readonly List<Style> _supportedStyles = [];

    /// <summary>
    /// Adds a supported style for the indicator.
    /// </summary>
    /// <param name="style">The style to add to supported styles.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public CompositeIndicatorListingBuilder AddSupportedStyle(Style style)
    {
        if (!_supportedStyles.Contains(style))
        {
            _supportedStyles.Add(style);
        }
        return this;
    }

    /// <summary>
    /// Sets multiple supported styles for the indicator.
    /// </summary>
    /// <param name="styles">The styles to set as supported.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public CompositeIndicatorListingBuilder WithSupportedStyles(params Style[] styles)
    {
        ArgumentNullException.ThrowIfNull(styles);

        _supportedStyles.Clear();
        foreach (var style in styles)
        {
            _supportedStyles.Add(style);
        }
        return this;
    }

    /// <summary>
    /// Sets the name of the indicator.
    /// </summary>
    /// <param name="name">The indicator name.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public new CompositeIndicatorListingBuilder WithName(string name)
    {
        base.WithName(name);
        return this;
    }

    /// <summary>
    /// Sets the ID of the indicator.
    /// </summary>
    /// <param name="id">The indicator ID.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public new CompositeIndicatorListingBuilder WithId(string id)
    {
        base.WithId(id);
        return this;
    }

    /// <summary>
    /// Sets the style of the indicator.
    /// </summary>
    /// <param name="style">The indicator style.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public new CompositeIndicatorListingBuilder WithStyle(Style style)
    {
        base.WithStyle(style);
        return this;
    }

    /// <summary>
    /// Sets the category of the indicator.
    /// </summary>
    /// <param name="category">The indicator category.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public new CompositeIndicatorListingBuilder WithCategory(Category category)
    {
        base.WithCategory(category);
        return this;
    }

    /// <summary>
    /// Adds a parameter to the indicator.
    /// </summary>
    /// <typeparam name="T">The type of the parameter.</typeparam>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="displayName">The display name of the parameter.</param>
    /// <param name="description">Optional description of the parameter.</param>
    /// <param name="isRequired">Whether the parameter is required.</param>
    /// <param name="defaultValue">Optional default value for the parameter.</param>
    /// <param name="minimum">Optional minimum value for the parameter.</param>
    /// <param name="maximum">Optional maximum value for the parameter.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public new CompositeIndicatorListingBuilder AddParameter<T>(
        string parameterName,
        string displayName,
        string? description = null,
        bool isRequired = false,
        object? defaultValue = null,
        double? minimum = null,
        double? maximum = null)
    {
        base.AddParameter<T>(parameterName, displayName, description, isRequired, defaultValue, minimum, maximum);
        return this;
    }

    /// <summary>
    /// Adds an enum parameter to the indicator.
    /// </summary>
    /// <typeparam name="T">The enum type of the parameter.</typeparam>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="displayName">The display name of the parameter.</param>
    /// <param name="description">Optional description of the parameter.</param>
    /// <param name="isRequired">Whether the parameter is required.</param>
    /// <param name="defaultValue">Optional default value for the parameter.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public new CompositeIndicatorListingBuilder AddEnumParameter<T>(
        string parameterName,
        string displayName,
        string? description = null,
        bool isRequired = false,
        T? defaultValue = default)
        where T : struct, Enum
    {
        base.AddEnumParameter<T>(parameterName, displayName, description, isRequired, defaultValue);
        return this;
    }

    /// <summary>
    /// Adds a result to the indicator.
    /// </summary>
    /// <param name="dataName">The name of the data property.</param>
    /// <param name="displayName">The display name of the result.</param>
    /// <param name="dataType">The type of the result.</param>
    /// <param name="isDefault">Whether this is the default result.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public new CompositeIndicatorListingBuilder AddResult(
        string dataName,
        string displayName,
        ResultType dataType = ResultType.Default,
        bool isDefault = false)
    {
        base.AddResult(dataName, displayName, dataType, isDefault);
        return this;
    }

    /// <summary>
    /// Adds common price High, Low, Close results to the indicator.
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public new CompositeIndicatorListingBuilder AddPriceHlcResult()
    {
        base.AddPriceHlcResult();
        return this;
    }

    /// <summary>
    /// Builds the CompositeIndicatorListing instance.
    /// </summary>
    /// <returns>A new CompositeIndicatorListing instance.</returns>
    public new IndicatorListing Build()
    {
        ValidateBeforeBuild();

        // Ensure we have at least one style
        if (_supportedStyles.Count == 0)
        {
            _supportedStyles.Add(base.CurrentStyle);
        }

        // Build base indicator listing and convert to composite
        var baseListing = base.Build();

        // Create a composite indicator listing
        // Cast to object first to avoid type resolution issues during compilation
        object composite = new CompositeIndicatorListing(base.CurrentBaseUrl) {
            Name = baseListing.Name,
            Uiid = baseListing.Uiid,
            Style = baseListing.Style,
            Category = baseListing.Category,
            Parameters = baseListing.Parameters,
            Results = baseListing.Results,
            ReturnType = baseListing.ReturnType,
            LegendTemplate = baseListing.LegendTemplate,
            SupportedStyles = _supportedStyles.AsReadOnly()
        };

        return (IndicatorListing)composite;
    }
}
