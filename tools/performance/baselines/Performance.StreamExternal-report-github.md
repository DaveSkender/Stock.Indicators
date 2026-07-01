```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8737/25H2/2025Update/HudsonValley2)
13th Gen Intel Core i9-13900H 2.60GHz, 1 CPU, 20 logical and 14 physical cores
.NET SDK 10.0.301
  [Host]   : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  ShortRun : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3


```
| Method    | Mean       | Error        | StdDev      |
|---------- |-----------:|-------------:|------------:|
| **EmaSeries** |   **5.024 μs** |     **1.399 μs** |   **0.0767 μs** |
| **EmaStream** | **946.799 μs** | **2,917.831 μs** | **159.9362 μs** |
