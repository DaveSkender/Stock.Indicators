using System;
using System.Text.Json;
using Skender.Stock.Indicators;

namespace MetadataTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get all indicators as JSON
            string allJson = IndicatorMetadata.ToJson();

            Console.WriteLine("==== ALL INDICATORS JSON ====");
            Console.WriteLine(allJson.Substring(0, Math.Min(1000, allJson.Length)) + "...");
            Console.WriteLine();

            // Get all indicators without chart config
            string minimalJson = IndicatorMetadata.ToJson(includeChartConfig: false);

            Console.WriteLine("==== MINIMAL INDICATORS JSON ====");
            Console.WriteLine(minimalJson.Substring(0, Math.Min(1000, minimalJson.Length)) + "...");
            Console.WriteLine();

            // Get a specific indicator
            var sma = IndicatorMetadata.GetById("SMA");

            if (sma != null)
            {
                Console.WriteLine("==== SMA INDICATOR ====");
                Console.WriteLine($"Name: {sma.Name}");
                Console.WriteLine($"ID: {sma.Uiid}");
                Console.WriteLine($"Style: {sma.Style}");
                Console.WriteLine($"Category: {sma.Category}");
                Console.WriteLine($"Parameters: {sma.Parameters?.Count ?? 0}");
                Console.WriteLine($"Results: {sma.Results?.Count ?? 0}");
                Console.WriteLine();

                if (sma.Parameters != null && sma.Parameters.Count > 0)
                {
                    Console.WriteLine("==== SMA PARAMETERS ====");
                    foreach (var param in sma.Parameters)
                    {
                        Console.WriteLine($"  - {param.ParamName} ({param.DisplayName}): Type={param.DataType}, Default={param.DefaultValue}");
                    }
                    Console.WriteLine();
                }
            }

            // Try to get by method name
            var ema = IndicatorMetadata.GetByMethod("GetEma");

            if (ema != null)
            {
                Console.WriteLine("==== EMA INDICATOR BY METHOD ====");
                Console.WriteLine($"Name: {ema.Name}");
                Console.WriteLine($"ID: {ema.Uiid}");
                Console.WriteLine($"Style: {ema.Style}");
                Console.WriteLine($"Category: {ema.Category}");
                Console.WriteLine();
            }
        }
    }
}
