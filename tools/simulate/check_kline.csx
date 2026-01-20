using System;
using System.Reflection;
using Coinbase.Net.Objects.Models;

var type = typeof(CoinbaseStreamKline);
Console.WriteLine("CoinbaseStreamKline properties:");
foreach (var prop in type.GetProperties())
{
    Console.WriteLine($"  {prop.Name}: {prop.PropertyType.Name}");
}
