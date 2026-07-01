```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8737/25H2/2025Update/HudsonValley2)
13th Gen Intel Core i9-13900H 2.60GHz, 1 CPU, 20 logical and 14 physical cores
.NET SDK 10.0.301
  [Host]   : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  ShortRun : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3


```
| Method   | Mean      | Error     | StdDev    |
|--------- |----------:|----------:|----------:|
| **AdlList**  | **10.797 μs** | **1.5566 μs** | **0.2409 μs** |
| **AtrList**  | **15.544 μs** | **1.1687 μs** | **0.1809 μs** |
| **DemaList** |  **9.225 μs** | **0.4582 μs** | **0.1190 μs** |
| **EmaList**  |  **9.019 μs** | **0.9354 μs** | **0.2429 μs** |
| **RsiList**  | **14.303 μs** | **1.4677 μs** | **0.2271 μs** |
| **SmaList**  | **15.860 μs** | **1.5602 μs** | **0.4052 μs** |
