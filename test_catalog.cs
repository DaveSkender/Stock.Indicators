using System;
using Skender.Stock.Indicators;

Console.WriteLine("Testing catalog properties...");

try 
{
    // Test AtrStop catalog
    var atrStopSeries = AtrStop.SeriesListing;
    var atrStopStream = AtrStop.StreamListing;
    
    Console.WriteLine($"AtrStop SeriesListing: {atrStopSeries.Name}");
    Console.WriteLine($"AtrStop StreamListing: {atrStopStream.Name}");
    
    // Test Macd catalog
    var macdSeries = Macd.SeriesListing;
    Console.WriteLine($"Macd SeriesListing: {macdSeries.Name}");
    
    // Test Adl catalog
    var adlSeries = Adl.SeriesListing;
    var adlStream = Adl.StreamListing;
    Console.WriteLine($"Adl SeriesListing: {adlSeries.Name}");
    Console.WriteLine($"Adl StreamListing: {adlStream.Name}");
    
    // Test EMA catalog (has all three types)
    var emaSeries = Ema.SeriesListing;
    var emaStream = Ema.StreamListing;
    var emaBuffer = Ema.BufferListing;
    Console.WriteLine($"Ema SeriesListing: {emaSeries.Name}");
    Console.WriteLine($"Ema StreamListing: {emaStream.Name}");
    Console.WriteLine($"Ema BufferListing: {emaBuffer.Name}");
    
    Console.WriteLine("All catalog properties are accessible!");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
