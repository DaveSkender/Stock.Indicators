using System.Text;
using System.Text.Json;

namespace Skender.Stock.Indicators;

/// <summary>
/// Provides extension methods for converting ISeries lists to formatted strings.
/// </summary>
public static class StringOut
{
    /// <summary>
    /// Converts an IEnumerable of ISeries to a formatted string.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the list.</typeparam>
    /// <param name="list">The list of elements to convert.</param>
    /// <param name="outType">The output format type.</param>
    /// <param name="limitQty">The maximum number of elements to include in the output.</param>
    /// <param name="startIndex">The starting index of the elements to include in the output.</param>
    /// <param name="endIndex">The ending index of the elements to include in the output.</param>
    /// <returns>A formatted string representing the list of elements.</returns>
    public static string ToStringOut<T>(this IEnumerable<T> list, OutType outType = OutType.FixedWidth, int? limitQty = null, int? startIndex = null, int? endIndex = null) where T : ISeries
    {
        if (list == null || !list.Any())
        {
            return string.Empty;
        }

        var limitedList = list;

        if (limitQty.HasValue)
        {
            limitedList = limitedList.Take(limitQty.Value);
        }

        if (startIndex.HasValue && endIndex.HasValue)
        {
            limitedList = limitedList.Skip(startIndex.Value).Take(endIndex.Value - startIndex.Value + 1);
        }

        switch (outType)
        {
            case OutType.CSV:
                return ToCsv(limitedList);
            case OutType.JSON:
                return ToJson(limitedList);
            case OutType.FixedWidth:
            default:
                return ToFixedWidth(limitedList);
        }
    }

    private static string ToCsv<T>(IEnumerable<T> list) where T : ISeries
    {
        var sb = new StringBuilder();
        var properties = typeof(T).GetProperties();

        sb.AppendLine(string.Join(",", properties.Select(p => p.Name)));

        foreach (var item in list)
        {
            sb.AppendLine(string.Join(",", properties.Select(p => p.GetValue(item))));
        }

        return sb.ToString();
    }

    private static string ToJson<T>(IEnumerable<T> list) where T : ISeries
    {
        return JsonSerializer.Serialize(list);
    }

    private static string ToFixedWidth<T>(IEnumerable<T> list) where T : ISeries
    {
        var sb = new StringBuilder();
        var properties = typeof(T).GetProperties();

        var headers = properties.Select(p => p.Name).ToArray();
        var values = list.Select(item => properties.Select(p => p.GetValue(item)?.ToString() ?? string.Empty).ToArray()).ToArray();

        var columnWidths = new int[headers.Length];

        for (int i = 0; i < headers.Length; i++)
        {
            columnWidths[i] = Math.Max(headers[i].Length, values.Max(row => row[i].Length));
        }

        sb.AppendLine(string.Join(" ", headers.Select((header, index) => header.PadRight(columnWidths[index]))));

        foreach (var row in values)
        {
            sb.AppendLine(string.Join(" ", row.Select((value, index) => value.PadRight(columnWidths[index]))));
        }

        return sb.ToString();
    }
}
