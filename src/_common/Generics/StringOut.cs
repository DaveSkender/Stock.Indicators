using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Json;


namespace Skender.Stock.Indicators;

/// <summary>
/// Provides extension methods for converting ISeries lists to formatted strings.
/// </summary>
public static class StringOut
{
    private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;
    private static readonly string[] First = ["i"];
    private static readonly JsonSerializerOptions jsonOptions = new() {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
        WriteIndented = false
    };

    /// <summary>
    /// Converts a list of ISeries to a formatted string.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list, which must implement ISeries.</typeparam>
    /// <param name="list">The list of ISeries elements to convert.</param>
    /// <param name="outType">The output format type (FixedWidth, CSV, JSON).</param>
    /// <param name="limitQty">Optional. The maximum number of elements to include in the output.</param>
    /// <param name="startIndex">Optional. The starting index of the elements to include in the output.</param>
    /// <param name="endIndex">Optional. The ending index of the elements to include in the output.</param>
    /// <param name="numberPrecision">Optional. The number of decimal places for numeric values.</param>
    /// <returns>A formatted string representation of the list.</returns>
    public static string ToStringOut<T>(
        this IEnumerable<T> list,
        OutType outType = OutType.FixedWidth,
        int? limitQty = null,
        int? startIndex = null,
        int? endIndex = null,
        int? numberPrecision = null)
        where T : ISeries
    {
        if (list == null || !list.Any())
        {
            return string.Empty;
        }

        IEnumerable<T> limitedList = list;

        if (limitQty.HasValue)
        {
            limitedList = limitedList.Take(limitQty.Value);
        }

        if (startIndex.HasValue && endIndex.HasValue)
        {
            limitedList = limitedList.Skip(startIndex.Value).Take(endIndex.Value - startIndex.Value + 1);
        }

        return outType switch {
            OutType.CSV => ToCsv(limitedList, numberPrecision),
            OutType.JSON => ToJson(limitedList),
            OutType.FixedWidth => ToFixedWidth(limitedList.ToList(), numberPrecision ?? 2),
            _ => throw new ArgumentOutOfRangeException(nameof(outType), outType, "Bad output argument."),
        };
    }

    /// <summary>
    /// Converts a list of ISeries to a JSON formatted string.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list, which must implement ISeries.</typeparam>
    /// <param name="list">The list of ISeries elements to convert.</param>
    /// <returns>A JSON formatted string representation of the list.</returns>
    public static string ToJson<T>(this IEnumerable<T> list) where T : ISeries
        => JsonSerializer.Serialize(list, jsonOptions);

    /// <summary>
    /// Converts a list of ISeries to a CSV formatted string.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list, which must implement ISeries.</typeparam>
    /// <param name="list">The list of ISeries elements to convert.</param>
    /// <param name="numberPrecision">Optional. The number of decimal places for numeric values.</param>
    /// <returns>A CSV formatted string representation of the list.</returns>
    public static string ToCsv<T>(
        this IEnumerable<T> list,
        int? numberPrecision = null)
        where T : ISeries
    {
        ArgumentNullException.ThrowIfNull(list);

        StringBuilder sb = new();

        PropertyInfo[] properties = typeof(T).GetProperties();

        // Exclude redundant IReusable 'Value' and 'Date' properties
        properties = properties.Where(p => p.Name is not "Value" and not "Date").ToArray();

        // Determine date formats per DateTime property
        Dictionary<string, string> dateFormats = DetermineDateFormats(properties, list);

        // Write header
        sb.AppendLine(string.Join(",", properties.Select(p => p.Name)));

        // Write data
        foreach (T item in list)
        {
            sb.AppendLine(string.Join(",", properties.Select(p => FormatValue(p, item, numberPrecision, dateFormats))));
        }

        return sb.ToString();  // includes a trailing newline
    }

    /// <summary>
    /// Converts a list of ISeries to a fixed-width formatted string.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list, which must implement ISeries.</typeparam>
    /// <param name="list">The list of ISeries elements to convert.</param>
    /// <param name="numberPrecision">Optional. The number of decimal places for numeric values.</param>
    /// <returns>A fixed-width formatted string representation of the list.</returns>
    public static string ToFixedWidth<T>(
        this IEnumerable<T> list,
        int numberPrecision = 2)
        where T : ISeries
    {
        ArgumentNullException.ThrowIfNull(list);

        StringBuilder sb = new();
        PropertyInfo[] properties = typeof(T).GetProperties();

        // Exclude redundant IReusable 'Value' and 'Date' properties
        properties = properties.Where(p => p.Name is not "Value" and not "Date").ToArray();

        // Determine date formats per DateTime property
        Dictionary<string, string> dateFormats = DetermineDateFormats(properties, list);

        // Determine column widths and alignment
        int[] columnWidths = DetermineColumnWidths(properties, list, numberPrecision, dateFormats, out bool[] isNumeric);

        string[] headers = First.Concat(properties.Select(p => p.Name)).ToArray();
        bool[] headersIsNumeric = new bool[headers.Length];

        // First column 'i' is numeric
        headersIsNumeric[0] = true;
        for (int i = 1; i < headers.Length; i++)
        {
            headersIsNumeric[i] = isNumeric[i - 1];
        }

        // Evaluate and format data
        string[][] dataRows = list.Select((item, index) => {

            string[] values = properties
                .Select(p => {

                    object? value = p.GetValue(item);

                    // format dates
                    if (p.PropertyType == typeof(DateTime))
                    {
                        string format = dateFormats[p.Name];
                        return ((DateTime)value!).ToString(format, Culture);
                    }

                    // format numbers
                    else
                    {
                        return value is IFormattable formattable
                            ? formattable.ToString($"F{numberPrecision}", Culture) ?? string.Empty
                            : value?.ToString() ?? string.Empty;
                    }
                })
                .ToArray();

            // Prepend index
            string[] row = new[] { index.ToString(Culture) }.Concat(values).ToArray();
            return row;

        }).ToArray();

        // Update column widths based on data rows
        for (int i = 0; i < headers.Length; i++)
        {
            foreach (string[] row in dataRows)
            {
                if (i < row.Length)
                {
                    columnWidths[i] = Math.Max(columnWidths[i], row[i].Length);
                }
            }
        }

        // Create header line with proper alignment
        string headerLine = string.Join("  ", headers.Select((header, index) =>
            headersIsNumeric[index] ? header.PadLeft(columnWidths[index]) : header.PadRight(columnWidths[index])
        ));
        sb.AppendLine(headerLine);

        // Create separator
        sb.AppendLine(new string('-', columnWidths.Sum(w => w + 2) - 2));

        // Create data lines with proper alignment
        foreach (string[] row in dataRows)
        {
            string dataLine = string.Join("  ", row.Select((value, index) =>
                headersIsNumeric[index] ? value.PadLeft(columnWidths[index]) : value.PadRight(columnWidths[index])
            ));
            sb.AppendLine(dataLine);
        }

        return sb.ToString();  // includes a trailing newline
    }

    /// <summary>
    /// Determines the appropriate date formats for DateTime properties based on the variability of the date values.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list, which must implement ISeries.</typeparam>
    /// <param name="properties">The array of PropertyInfo objects representing the properties of the type.</param>
    /// <param name="list">The list of ISeries elements to analyze.</param>
    /// <returns>A dictionary mapping property names to date format strings.</returns>
    private static Dictionary<string, string> DetermineDateFormats<T>(
        PropertyInfo[] properties,
        IEnumerable<T> list)
        where T : ISeries
    {
        List<PropertyInfo> dateTimeProperties = properties.Where(p => p.PropertyType == typeof(DateTime)).ToList();
        Dictionary<string, string> dateFormats = [];

        foreach (PropertyInfo? prop in dateTimeProperties)
        {
            List<string> dateValues = list.Select(item => ((DateTime)prop.GetValue(item)!).ToString("o", Culture)).ToList();

            bool sameHour = dateValues.Select(d => d.Substring(11, 2)).Distinct().Count() == 1;
            bool sameMinute = dateValues.Select(d => d.Substring(14, 2)).Distinct().Count() == 1;
            bool sameSecond = dateValues.Select(d => d.Substring(17, 2)).Distinct().Count() == 1;

            dateFormats[prop.Name] = sameHour && sameMinute ? "yyyy-MM-dd" : sameSecond ? "yyyy-MM-dd HH:mm" : "yyyy-MM-dd HH:mm:ss";
        }

        return dateFormats;
    }

    /// <summary>
    /// Determines the column widths and alignment for the properties of the type.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list, which must implement ISeries.</typeparam>
    /// <param name="properties">The array of PropertyInfo objects representing the properties of the type.</param>
    /// <param name="list">The list of ISeries elements to analyze.</param>
    /// <param name="numberPrecision">The number of decimal places for numeric values.</param>
    /// <param name="dateFormats">A dictionary mapping property names to date format strings.</param>
    /// <param name="isNumeric">An output array indicating whether each property is numeric.</param>
    /// <returns>An array of integers representing the column widths for each property.</returns>
    private static int[] DetermineColumnWidths<T>(
        PropertyInfo[] properties,
        IEnumerable<T> list,
        int numberPrecision,
        Dictionary<string, string> dateFormats,
        out bool[] isNumeric)
        where T : ISeries
    {
        int propertyCount = properties.Length;
        isNumeric = new bool[propertyCount];
        int[] columnWidths = new int[propertyCount];

        // Determine if each property is numeric
        for (int i = 0; i < propertyCount; i++)
        {
            isNumeric[i] = properties[i].PropertyType.IsNumeric();
            columnWidths[i] = properties[i].Name.Length;
        }

        // Include the first column 'i'
        columnWidths = new int[propertyCount + 1];
        isNumeric = new bool[propertyCount + 1];
        columnWidths[0] = "i".Length;
        isNumeric[0] = true; // 'i' is numeric

        for (int i = 0; i < propertyCount; i++)
        {
            isNumeric[i + 1] = properties[i].PropertyType.IsNumeric();
            columnWidths[i + 1] = properties[i].Name.Length;
        }

        // Update index column
        int index = 0;
        foreach (T item in list)
        {
            // Update index column
            string indexStr = index.ToString(Culture);
            columnWidths[0] = Math.Max(columnWidths[0], indexStr.Length);

            for (int i = 0; i < propertyCount; i++)
            {
                object? value = properties[i].GetValue(item);
                string formattedValue;

                if (properties[i].PropertyType == typeof(DateTime))
                {
                    string format = dateFormats[properties[i].Name];
                    formattedValue = ((DateTime)value!).ToString(format, Culture);
                }
                else
                {
                    formattedValue = properties[i].PropertyType.IsNumeric()
                        ? value is IFormattable formattable
                            ? formattable.ToString($"F{numberPrecision}", Culture) ?? string.Empty
                            : value?.ToString() ?? string.Empty
                        : value?.ToString() ?? string.Empty;
                }

                columnWidths[i + 1] = Math.Max(columnWidths[i + 1], formattedValue.Length);
            }

            index++;
        }

        return columnWidths;
    }

    /// <summary>
    /// Formats the value of a property for output.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list, which must implement ISeries.</typeparam>
    /// <param name="prop">The PropertyInfo object representing the property.</param>
    /// <param name="item">The item from which to get the property value.</param>
    /// <param name="numberPrecision">The number of decimal places for numeric values.</param>
    /// <param name="dateFormats">A dictionary mapping property names to date format strings.</param>
    /// <returns>The formatted value as a string.</returns>
    private static string FormatValue<T>(PropertyInfo prop, T item, int? numberPrecision, Dictionary<string, string> dateFormats) where T : ISeries
    {
        object? value = prop.GetValue(item);
        if (prop.PropertyType == typeof(DateTime))
        {
            string format = dateFormats[prop.Name];
            return ((DateTime)value!).ToString(format, Culture);
        }
        else
        {
            return numberPrecision.HasValue
                ? value is IFormattable formattable
                    ? formattable.ToString($"F{numberPrecision}", Culture) ?? string.Empty
                    : value?.ToString() ?? string.Empty
                : value?.ToString() ?? string.Empty;
        }
    }
}
