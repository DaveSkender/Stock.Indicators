```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8737/25H2/2025Update/HudsonValley2)
13th Gen Intel Core i9-13900H 2.60GHz, 1 CPU, 20 logical and 14 physical cores
.NET SDK 10.0.301
  [Host]     : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  Job-YBWYST : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3


```
| Method         | Mean      | Error     | StdDev    |
|--------------- |----------:|----------:|----------:|
| **AbsDblNul**      | **0.0051 ns** | **0.0066 ns** | **0.0040 ns** |
| **AbsDblVal**      | **0.0383 ns** | **0.0129 ns** | **0.0077 ns** |
| **NaN2NullDblNul** | **0.0019 ns** | **0.0045 ns** | **0.0030 ns** |
| **NaN2NullDblVal** | **0.0422 ns** | **0.0071 ns** | **0.0047 ns** |
| **NaN2NullNanNul** | **0.0099 ns** | **0.0100 ns** | **0.0066 ns** |
| **NaN2NullNaNVal** | **0.0122 ns** | **0.0088 ns** | **0.0058 ns** |
| **Null2NaNDblNul** | **0.0002 ns** | **0.0006 ns** | **0.0004 ns** |
| **Null2NaNDblVal** | **0.0040 ns** | **0.0045 ns** | **0.0030 ns** |
| **Null2NaNDecNul** | **0.0035 ns** | **0.0040 ns** | **0.0027 ns** |
| **Null2NaNDecVal** | **0.0055 ns** | **0.0061 ns** | **0.0036 ns** |
