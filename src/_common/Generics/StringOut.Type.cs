using System.Globalization;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace Skender.Stock.Indicators;

/// <summary>
/// Provides extension methods for converting ISeries instances to formatted strings.
/// </summary>
public static partial class StringOut
{
    private static readonly CultureInfo culture = CultureInfo.InvariantCulture;

    /// <summary>
    /// Writes the string representation of an ISeries instance to the console.
    /// </summary>
    /// <typeparam name="T">The type of the ISeries instance.</typeparam>
    /// <param name="obj">The ISeries instance to write to the console.</param>
    /// <returns>The string representation of the ISeries instance.</returns>
    public static string ToConsole<T>(this T obj) where T : ISeries
    {
        string? output = obj.ToStringOut();
        Console.WriteLine(output);
        return output ?? string.Empty;
    }

    /// <summary>
    /// Converts an ISeries instance to a formatted string.
    /// </summary>
    /// <typeparam name="T">The type of the ISeries instance.</typeparam>
    /// <param name="obj">The ISeries instance to convert.</param>
    /// <returns>A formatted string representation of the ISeries instance.</returns>
    public static string ToStringOut<T>(this T obj) where T : ISeries
    {
        ArgumentNullException.ThrowIfNull(obj);
        StringBuilder sb = new();

        // Header names
        string[] headers = ["Property", "Type", "Value", "Description"];

        // Get properties of the object
        PropertyInfo[] properties = GetStringOutProperties(typeof(T));

        // Lists to hold column data
        List<string> names = new(properties.Length);
        List<string> types = new(properties.Length);
        List<string> values = new(properties.Length);
        List<string> descriptions = new(properties.Length);

        // Get descriptions from XML documentation
        Dictionary<string, string> descriptionDict
            = GetPropertyDescriptionsFromXml(typeof(T));

        // Populate the lists
        foreach (PropertyInfo prop in properties)
        {
            string name = prop.Name;
            string type = prop.PropertyType.Name;
            object? value = prop.GetValue(obj);

            // add values to lists
            names.Add(name);
            types.Add(type);

            switch (value)
            {
                case DateTime dateTimeValue:
                    values.Add(dateTimeValue.Kind == DateTimeKind.Utc
                        ? dateTimeValue.ToString("u", culture)
                        : dateTimeValue.ToString("s", culture));
                    break;

                case DateOnly dateOnlyValue:
                    values.Add(dateOnlyValue.ToString("yyyy-MM-dd", culture));
                    break;

                case DateTimeOffset dateTimeOffsetValue:
                    values.Add(dateTimeOffsetValue.ToString("o", culture));
                    break;

                case string stringValue:
                    // limit string size
                    if (stringValue.Length > 35)
                    {
                        stringValue = $"{stringValue.AsSpan(0, 32)}...";
                    }

                    values.Add(stringValue);
                    break;

                default:
                    values.Add(value?.ToString() ?? string.Empty);
                    break;
            }

            // get/add description from XML documentation
            descriptionDict.TryGetValue(name, out string? description);

            if (description == null)
            {
                description = string.Empty;
            }
            else if (description.Length > 50)
            {
                description = $"{description.AsSpan(0, 47)}...";
            }

            descriptions.Add(description);
        }

        // Calculate the maximum width for each column
        int widthOfName = MaxWidth(headers[0], names);
        int widthOfType = MaxWidth(headers[1], types);
        int widthOfValue = MaxWidth(headers[2], values);
        int widthOfDesc = MaxWidth(headers[3], descriptions);

        // Ensure at least 2 spaces between columns
        string format = $"{{0,-{widthOfName}}}  {{1,-{widthOfType}}}  {{2,{widthOfValue}}}  {{3}}";

        // Build the header
        sb.AppendFormat(culture, format, headers[0], headers[1], headers[2], headers[3]).AppendLine();

        // Build the separator line
        int totalWidth = widthOfName + widthOfType + widthOfValue + Math.Min(widthOfDesc, 30) + 6; // +6 for spaces
        sb.AppendLine(new string('-', totalWidth));

        // Build each row
        for (int i = 0; i < names.Count; i++)
        {
            string row = string.Format(culture, format, names[i], types[i], values[i], descriptions[i]);
            sb.AppendLine(row);
        }

        return sb.ToString().TrimEnd();
    }

    /// <summary>
    /// Calculates the maximum width of a column based on the header and values.
    /// </summary>
    /// <param name="header">The header of the column.</param>
    /// <param name="values">The list of values in the column.</param>
    /// <returns>The maximum width of the column.</returns>
    private static int MaxWidth(string header, List<string> values)
    {
        int maxValue = values.Count != 0 ? values.Max(static v => v.Length) : 0;
        return Math.Max(header.Length, maxValue);
    }

    /// <summary>
    /// Retrieves property descriptions from the XML documentation file.
    /// </summary>
    /// <param name="type">The type whose property descriptions are to be retrieved.</param>
    /// <returns>A dictionary containing property names and their descriptions.</returns>
    private static Dictionary<string, string> GetPropertyDescriptionsFromXml(Type type)
    {
        Dictionary<string, string> descriptions = [];

        // Get the assembly of the type
        Assembly assembly = type.Assembly;
        string? assemblyLocation = assembly.Location;

        // Assume the XML documentation file is in the same directory as the assembly
        string xmlFilePath = Path.ChangeExtension(assemblyLocation, ".xml");

        if (!File.Exists(xmlFilePath))
        {
            // XML documentation file not found
            return descriptions;
        }

        // Load the XML documentation file
        XDocument xdoc = XDocument.Load(xmlFilePath);

        // Build the prefix for property members
        string memberPrefix = "P:" + type.FullName + ".";

        // Query all member elements
        foreach (XElement memberElement in xdoc.Descendants("member"))
        {
            string? nameAttribute = memberElement.Attribute("name")?.Value;

            if (nameAttribute?.StartsWith(memberPrefix, StringComparison.OrdinalIgnoreCase) == true)
            {
                string propName = nameAttribute[memberPrefix.Length..];

                // Get the summary element
                XElement? summaryElement = memberElement.Element("summary");
                descriptions[propName] = summaryElement?.ParseXmlElement() ?? string.Empty;
            }
        }

        return descriptions;
    }

    /// <summary>
    /// Ensures that the text content of an XML documentation properly
    /// converts HTML refs like <see cref="long"/> and <see langword="get"/>."/>
    /// </summary>
    /// <param name="summaryElement"><see cref="XElement"/> to be cleaned.</param>
    /// <returns>The cleaned text content of the XML documentation.</returns>
    private static string ParseXmlElement(this XElement? summaryElement)
    {
        if (summaryElement == null)
        {
            return string.Empty;
        }

        // Handle <see> elements
        foreach (XNode node in summaryElement.DescendantNodes().ToList())
        {
            if (node is XElement element && element.Name.LocalName == "see")
            {
                foreach (XAttribute attribute in element.Attributes().ToList())
                {
                    string word = attribute.Value.Split('.').Last();
                    element.ReplaceWith($"'{new XText(word)}'");
                }
            }
        }

        // Return summary text without line breaks
        return summaryElement.Value
            .Replace("\n", " ", StringComparison.Ordinal)
            .Replace("\r", " ", StringComparison.Ordinal)
            .Trim();
    }

    /// <summary>
    /// Retrieves the public instance properties of a type that are not marked with
    /// <see cref="JsonIgnoreAttribute"/> or <see cref="ObsoleteAttribute"/>.
    /// </summary>
    /// <param name="type">The type whose properties are to be retrieved.</param>
    /// <returns>An array of <see cref="PropertyInfo"/> objects representing the properties of the type.</returns>
    private static PropertyInfo[] GetStringOutProperties(Type type)
        => type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(static p => p.GetCustomAttribute<JsonIgnoreAttribute>() == null
                     && p.GetCustomAttribute<ObsoleteAttribute>() == null)
            .ToArray();
}
