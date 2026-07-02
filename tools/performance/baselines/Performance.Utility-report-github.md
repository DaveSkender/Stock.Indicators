```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8737/25H2/2025Update/HudsonValley2)
13th Gen Intel Core i9-13900H 2.60GHz, 1 CPU, 20 logical and 14 physical cores
.NET SDK 10.0.301
  [Host]   : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3
  ShortRun : .NET 10.0.9 (10.0.9, 10.0.926.27113), X64 RyuJIT x86-64-v3


```
| Method              | Mean         | Error         | StdDev      |
|-------------------- |-------------:|--------------:|------------:|
| **Aggregate**           |   **119.754 μs** |    **81.7190 μs** |   **4.4793 μs** |
| **RemoveWarmupPeriods** |    **28.613 μs** |    **27.4217 μs** |   **1.5031 μs** |
| **ToCandleResults**     |    **17.152 μs** |     **4.0994 μs** |   **0.2247 μs** |
| **ToListBarD**          |    **10.022 μs** |     **5.5848 μs** |   **0.3061 μs** |
| **ToReusableClose**     |    **20.307 μs** |     **4.0874 μs** |   **0.2240 μs** |
| **ToReusableOhlc4**     |    **31.933 μs** |     **3.6137 μs** |   **0.1981 μs** |
| **ToSortedList**        |     **9.467 μs** |     **5.0627 μs** |   **0.2775 μs** |
| **ToStringOutList**     |   **493.465 μs** | **1,669.6268 μs** |  **91.5179 μs** |
| **ToStringOutType**     | **5,308.147 μs** | **2,903.9624 μs** | **159.1760 μs** |
| **Validate**            |     **1.094 μs** |     **0.0262 μs** |   **0.0014 μs** |
