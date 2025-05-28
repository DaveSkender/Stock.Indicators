using System;
using System.Collections.Generic;

namespace Skender.Stock.Indicators;

/// <summary>
/// Builder for creating CompositeIndicatorListing instances using a fluent interface.
/// </summary>
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
