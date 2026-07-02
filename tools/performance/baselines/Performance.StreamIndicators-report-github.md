```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8737/25H2/2025Update/HudsonValley2)
13th Gen Intel Core i9-13900H 2.60GHz, 1 CPU, 20 logical and 14 physical cores
.NET SDK 10.0.301
  [Host]   : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  ShortRun : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3


```
| Method | Mean       | Error      | StdDev   |
|------- |-----------:|-----------:|---------:|
| **AdlHub** | **3,699.2 μs** | **3,174.3 μs** | **824.4 μs** |
| **AtrHub** | **4,064.7 μs** | **3,609.2 μs** | **937.3 μs** |
| **EmaHub** |   **739.7 μs** |   **586.7 μs** | **152.4 μs** |
