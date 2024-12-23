using System.Reflection;
using System.Text;


namespace Skender.Stock.Indicators;

/// <summary>
/// Provides extension methods for converting ISeries lists to formatted strings.
/// </summary>
public static partial class StringOut
{
    private static readonly string[] IndexHeaderName = ["i"];

    /// <summary>
    /// Default formats for numeric and date properties.
    /// 'Key' value is either property type or property name.
    /// 'Value' is the format string for the property's ToString().
    /// <remarks>
    /// The last matching key is used, so when users provide
    /// an 'args' dictionary, it will override these defaults.
    /// </remarks>
    /// </summary>
    private static readonly Dictionary<string, string> defaultArgs = new()
    {
        { "Decimal" , "auto" },
        { "Double"  , "N6" },
        { "Single"  , "N6" },
        { "Int16"   , "N0" },
        { "Int32"   , "N0" },
        { "Int64"   , "N0" },
        { "DateOnly", "yyyy-MM-dd" },
        { "DateTime", "yyyy-MM-dd HH:mm:ss" },
        { "DateTimeOffset", "yyyy-MM-dd HH:mm:ss" },
        { "Timestamp", "auto" }
    };

    /// <summary>
    /// Converts a list of ISeries to a fixed-width formatted string and writes it to the console.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list, which must implement ISeries.</typeparam>
    /// <param name="list">The list of ISeries elements to convert.</param>
    /// <returns>The fixed-width formatted string representation of the list.</returns>
    public static string ToConsole<T>(this IEnumerable<T> list) where T : ISeries
    {
        string? output = list.ToFixedWidth();
        Console.WriteLine(output);
        return output ?? string.Empty;
    }

    /// <summary>
    /// Converts a list of ISeries to a fixed-width formatted string.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list, which must implement ISeries.</typeparam>
    /// <param name="list">The list of ISeries elements to convert.</param>
    /// <param name="args">Optional formatting overrides.</param>
    /// <returns>A fixed-width formatted string representation of the list.</returns>
    public static string ToFixedWidth<T>(
        this IEnumerable<T> list,
        Dictionary<string, string>? args = null)
        where T : ISeries
    {
        ArgumentNullException.ThrowIfNull(list);

        Dictionary<string, string> formatArgs = defaultArgs
            .Concat(args ?? [])
            .GroupBy(kvp => kvp.Key.ToUpperInvariant())
            .ToDictionary(g => g.Key, g => g.Last().Value);

        // Get properties of the object,
        // excluding those with JsonIgnore or Obsolete attributes
        PropertyInfo[] properties = typeof(T)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(prop =>
                !Attribute.IsDefined(prop, typeof(JsonIgnoreAttribute)) &&
                !Attribute.IsDefined(prop, typeof(ObsoleteAttribute)))
            .ToArray();

        // Define header values
        string[] headers = IndexHeaderName
            .Concat(properties.Select(p => p.Name))
            .ToArray();

        int columnCount = headers.Length;

        // Set formatting for each column
        string[] formats = new string[columnCount];
        bool[] alignLeft = new bool[columnCount];
        int[] columnWidth = headers.Select(header => header.Length).ToArray();

        formats[0] = "N0";     // index is always an integer
        alignLeft[0] = false;  // index is always right-aligned

        for (int i = 1; i < columnCount; i++)
        {
            PropertyInfo property = properties[i - 1];

            // try by property type
            formats[i] = formatArgs.TryGetValue(
                ColloquialTypeName(property.PropertyType).ToUpperInvariant(),
                    out string? typeFormat)
                ? typeFormat
                : string.Empty;

            // try by property name (overrides type)
            formats[i] = formatArgs.TryGetValue(
                property.Name.ToUpperInvariant(),
                    out string? nameFormat)
                ? nameFormat
                : formats[i];

            // handle auto-detect
            if (formats[i] == "auto")
            {
                formats[i] = AutoFormat(property, list);
            }

            // set alignment
            alignLeft[i] = !property.PropertyType.IsNumeric();
        }

        // Compile formatted values
        string[][] dataRows = list.Select((item, index) => {
            string[] row = new string[columnCount];

            row[0] = index.ToString(formats[0], culture);

            for (int i = 1; i < columnCount; i++)
            {
                object? value = properties[i - 1].GetValue(item);
                row[i] = value is IFormattable formattable
                    ? formattable.ToString(formats[i], culture) ?? string.Empty
                    : value?.ToString() ?? string.Empty;

                columnWidth[i] = Math.Max(columnWidth[i], row[i].Length);
            }

            return row;
        }).ToArray();

        columnWidth[0] = dataRows.Max(row => row[0].Length);

        // Compile formatted string
        StringBuilder sb = new();

        // Create header line with proper alignment
        sb.AppendLine(string.Join("  ",
            headers.Select((header, index) => alignLeft[index]
                ? header.PadRight(columnWidth[index])
                : header.PadLeft(columnWidth[index])
        )));

        // Create separator
        sb.AppendLine(new string('-', columnWidth.Sum(w => w + 2) - 2));

        // Create data lines with proper alignment
        foreach (string[] row in dataRows)
        {
            sb.AppendLine(string.Join("  ",
                row.Select((value, index) => alignLeft[index]
                    ? value.PadRight(columnWidth[index])
                    : value.PadLeft(columnWidth[index])
            )));
        }

        return sb.ToString();  // includes a trailing newline
    }

    /// <summary>
    /// Determines the appropriate date precision or decimal places
    /// based on the first 1,000 actual values.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list, which must implement ISeries.</typeparam>
    /// <param name="property">The array of PropertyInfo objects representing the properties of the type.</param>
    /// <param name="list">The list of ISeries elements to analyze.</param>
    /// <returns>Format to be used in ToString()</returns>
    private static string AutoFormat<T>(
        PropertyInfo property,
        IEnumerable<T> list)
        where T : ISeries
    {
        Type propertyType = property.PropertyType;

        // auto-detect date format from precision
        if (propertyType == typeof(DateOnly))
        {
            return "yyyy-MM-dd";
        }

        else if (propertyType == typeof(DateTime) || propertyType == typeof(DateTimeOffset))
        {
            List<string> dateValues = list
                .Take(1000)
                .Select(item => ((DateTime)property.GetValue(item)!).ToString("o", culture)).ToList();

            bool sameHour = dateValues.Select(d => d.Substring(11, 2)).Distinct().Count() == 1;
            bool sameMinute = dateValues.Select(d => d.Substring(14, 2)).Distinct().Count() == 1;
            bool sameSecond = dateValues.Select(d => d.Substring(17, 2)).Distinct().Count() == 1;

            return sameHour && sameMinute && sameSecond
                 ? "yyyy-MM-dd"
                 : sameSecond
                   ? "yyyy-MM-dd HH:mm"
                   : "yyyy-MM-dd HH:mm:ss";
        }

        // auto-detect decimal places
        else if (propertyType == typeof(decimal))
        {
            int decimalPlaces = list
                .Take(1000)
                .Select(item => ((decimal)property.GetValue(item)!).GetDecimalPlaces())
                .Max();

            return $"N{decimalPlaces}";
        }
        else
        {
            return string.Empty;
        }
    }


    public static string ColloquialTypeName(Type type)
    {
        if (type == null)
        {
            return string.Empty;
        }

        // Handle nullable types
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            type = Nullable.GetUnderlyingType(type) ?? type; // Extract the underlying type
        }

        // Return the type's C# alias if it exists, or the type's name otherwise
        if (type.IsPrimitive || type == typeof(string) || type == typeof(decimal) || type == typeof(DateTime))
        {
            // Return the type's C# alias if it exists, or the type's name otherwise
            return type.Name;
        }
        else
        {
            // Return the type's C# alias if it exists, or the type's name otherwise
            return type.Name;
        }
    }
}
