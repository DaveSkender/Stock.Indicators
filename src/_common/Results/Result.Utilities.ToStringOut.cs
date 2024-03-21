using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Skender.Stock.Indicators;

// RESULTS UTILITIES: ToStringOut

public static partial class ResultUtility
{
    private static readonly JsonSerializerOptions prettyJsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    /// <summary>
    /// Converts any results (or quotes) series into a string for output.
    /// Use extension method as `results.ToStringOut()` or `quotes.ToStringOut()`.
    /// See other overrides to specify alternate output formats.
    /// </summary>
    /// <typeparam name="TSeries"></typeparam>
    /// <param name="series">Any IEnumerable<typeparamref name="TSeries"/></param>
    /// <returns>
    /// String with fixed-width data columns, numbers with 4 decimals shown,
    /// and named headers.
    /// </returns>
    public static string ToStringOut<TSeries>(
        this IEnumerable<TSeries> series)
        where TSeries : ISeries, new()
        => series.ToStringOut(OutType.FixedWidth, 4);

    public static string ToStringOut<TSeries>(
        this IEnumerable<TSeries> series, OutType outType)
        where TSeries : ISeries, new()
        => series.ToStringOut(outType, int.MaxValue);

    public static string ToStringOut<TSeries>(
        this IEnumerable<TSeries> series,
        OutType outType, int decimalsToDisplay)
        where TSeries : ISeries, new()
    {
        // JSON OUTPUT
        if (outType == OutType.JSON)
        {
            if (decimalsToDisplay != int.MaxValue)
            {
                string message
                    = $"ToStringOut() for JSON output ignores number format N{decimalsToDisplay}.";
                Console.WriteLine(message);
            }

            return JsonSerializer.Serialize(series, prettyJsonOptions);
        }

        // initialize results
        List<TSeries> seriesList = series.ToList();
        int qtyResults = seriesList.Count;

        // compose content and format containers
        PropertyInfo[] headerProps =
        [.. typeof(TSeries).GetProperties()];

        int qtyProps = headerProps.Length;

        int[] stringSizeMax = new int[qtyProps];
        string[] stringHeaders = new string[qtyProps];
        string[] stringFormats = new string[qtyProps];
        bool[] stringNumeric = new bool[qtyProps];

        string[][] stringContent = new string[qtyResults][];

        // use specified decimal format type
        string numberFormat = decimalsToDisplay == int.MaxValue
            ? string.Empty
            : $"N{decimalsToDisplay}";

        // define property formats
        for (int p = 0; p < qtyProps; p++)
        {
            PropertyInfo prop = headerProps[p];

            // containers
            stringHeaders[p] = prop.Name;

            // determine type format and width
            Type? nullableType = Nullable.GetUnderlyingType(prop.PropertyType);
            TypeCode code = Type.GetTypeCode(nullableType ?? prop.PropertyType);

            stringNumeric[p] = code switch
            {
                TypeCode.Double => true,
                TypeCode.Decimal => true,
                TypeCode.DateTime => false,
                _ => false
            };

            string formatType = code switch
            {
                TypeCode.Double => numberFormat,
                TypeCode.Decimal => numberFormat,
                TypeCode.DateTime => "o",
                _ => string.Empty
            };

            stringFormats[p] = string.IsNullOrEmpty(formatType)
                ? $"{{0}}"
                : $"{{0:{formatType}}}";

            // is max length?
            if (outType == OutType.FixedWidth
                && prop.Name.Length > stringSizeMax[p])
            {
                stringSizeMax[p] = prop.Name.Length;
            }
        }

        // get formatted result string values
        for (int i = 0; i < qtyResults; i++)
        {
            TSeries s = seriesList[i];

            PropertyInfo[] resultProps =
            [.. s.GetType().GetProperties()];

            stringContent[i] = new string[resultProps.Length];

            for (int p = 0; p < resultProps.Length; p++)
            {
                object? value = resultProps[p].GetValue(s);

                string formattedValue = string.Format(
                    CultureInfo.InvariantCulture, stringFormats[p], value);

                stringContent[i][p] = formattedValue;

                // is max length?
                if (outType == OutType.FixedWidth
                    && formattedValue.Length > stringSizeMax[p])
                {
                    stringSizeMax[p] = formattedValue.Length;
                }
            }
        }

        // CSV OUTPUT
        if (outType == OutType.CSV)
        {
            StringBuilder csv = new(string.Empty);

            csv.AppendLine(string.Join(", ", stringHeaders));

            for (int i = 0; i < stringContent.Length; i++)
            {
                string[] row = stringContent[i];
                csv.AppendLine(string.Join(", ", row));
            }

            return csv.ToString();
        }

        // FIXED WIDTH OUTPUT
        else if (outType == OutType.FixedWidth)
        {
            StringBuilder fw = new(string.Empty);

            // recompose header strings to width
            for (int p = 0; p < qtyProps; p++)
            {
                string s = stringHeaders[p];
                int w = stringSizeMax[p];
                int f = stringNumeric[p] ? w : -w;
                stringHeaders[p] = string.Format(
                    CultureInfo.InvariantCulture, $"{{0,{f}}}", s);
            }
            fw.AppendLine(string.Join("  ", stringHeaders));

            // recompose body strings to width
            for (int i = 0; i < qtyResults; i++)
            {
                for (int p = 0; p < qtyProps; p++)
                {
                    string s = stringContent[i][p];
                    int w = stringSizeMax[p];
                    int f = stringNumeric[p] ? w : -w;
                    stringContent[i][p] = string.Format(
                        CultureInfo.InvariantCulture, $"{{0,{f}}}", s);
                }

                string[] row = stringContent[i];
                fw.AppendLine(string.Join("  ", row));
            }

            return fw.ToString();
        }

        else
        {
            throw new ArgumentOutOfRangeException(nameof(outType));
        }
    }
}
