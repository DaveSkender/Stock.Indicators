# Performance benchmarks for v1.1.0

These are the execution times for the current indicators using two years of historical daily stock quotes (502 periods) with default or typical parameters.

``` bash
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19041.572 (2004/?/20H1)
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=5.0.100
  [Host]     : .NET Core 5.0.0 (CoreCLR 5.0.20.51904, CoreFX 5.0.20.51904), X64 RyuJIT
  DefaultJob : .NET Core 5.0.0 (CoreCLR 5.0.20.51904, CoreFX 5.0.20.51904), X64 RyuJIT
```

## indicators

|             Method |        Mean |     Error |    StdDev |      Median |
|------------------- |------------:|----------:|----------:|------------:|
|             GetAdl |   166.29 μs |  3.317 μs |  9.356 μs |   163.07 μs |
|      GetAdlWithSma |   428.63 μs |  3.787 μs |  3.163 μs |   427.75 μs |
|             GetAdx |   845.12 μs | 10.864 μs | 12.933 μs |   843.00 μs |
|           GetAroon |   337.13 μs |  3.841 μs |  3.405 μs |   337.53 μs |
|             GetAtr |   184.31 μs |  3.653 μs |  5.239 μs |   183.19 μs |
|            GetBeta | 1,049.45 μs | 12.113 μs |  9.457 μs | 1,050.90 μs |
|  GetBollingerBands |   442.86 μs |  5.722 μs |  4.468 μs |   442.49 μs |
|             GetCci |   910.38 μs | 22.144 μs | 64.946 μs |   879.66 μs |
|      GetChaikinOsc |   377.55 μs |  1.668 μs |  1.478 μs |   376.92 μs |
|      GetChandelier |   363.40 μs |  3.433 μs |  3.043 μs |   362.35 μs |
|             GetCmf |   662.24 μs |  2.385 μs |  1.991 μs |   661.99 μs |
|      GetConnorsRsi | 1,279.67 μs |  3.615 μs |  3.019 μs | 1,279.85 μs |
|     GetCorrelation |   817.38 μs |  7.546 μs |  6.301 μs |   814.70 μs |
|        GetDonchian |   302.10 μs |  1.735 μs |  1.354 μs |   301.92 μs |
|       GetDoubleEma |   253.41 μs |  3.928 μs |  3.482 μs |   252.23 μs |
|             GetEma |   139.20 μs |  0.670 μs |  0.559 μs |   139.08 μs |
|      GetHeikinAshi |   183.00 μs |  0.617 μs |  0.515 μs |   183.01 μs |
|             GetHma | 1,426.38 μs |  6.791 μs |  6.020 μs | 1,423.66 μs |
|        GetIchimoku |   877.50 μs |  4.985 μs |  4.419 μs |   876.12 μs |
|         GetKeltner |   526.29 μs |  1.989 μs |  1.553 μs |   526.26 μs |
|            GetMacd |   418.10 μs |  3.455 μs |  2.697 μs |   417.17 μs |
|             GetMfi |   498.57 μs |  7.252 μs |  9.429 μs |   496.00 μs |
|             GetObv |    66.55 μs |  0.549 μs |  0.514 μs |    66.41 μs |
|      GetObvWithSma |   192.81 μs |  7.121 μs | 20.771 μs |   187.22 μs |
|    GetParabolicSar |   141.38 μs |  2.811 μs |  4.697 μs |   141.78 μs |
|             GetPmo |   398.54 μs |  7.699 μs | 11.041 μs |   395.49 μs |
|             GetPrs |   201.48 μs |  4.013 μs |  8.466 μs |   201.01 μs |
|      GetPrsWithSma |   273.75 μs |  5.403 μs |  8.571 μs |   271.70 μs |
|             GetRoc |   143.71 μs |  2.868 μs |  8.044 μs |   142.89 μs |
|      GetRocWithSma |   401.29 μs |  5.310 μs |  4.967 μs |   399.92 μs |
|             GetRsi |   482.43 μs |  6.164 μs |  5.766 μs |   481.19 μs |
|           GetSlope |   997.45 μs | 19.623 μs | 40.525 μs | 1,001.09 μs |
|             GetSma |   164.34 μs |  3.247 μs |  7.127 μs |   163.61 μs |
|     GetSmaExtended | 1,136.51 μs | 22.495 μs | 22.093 μs | 1,131.39 μs |
|          GetStdDev |   482.58 μs |  8.562 μs |  8.793 μs |   482.03 μs |
|   GetStdDevWithSma |   594.46 μs | 11.689 μs | 10.933 μs |   594.59 μs |
|           GetStoch |   493.60 μs |  7.157 μs |  6.695 μs |   495.79 μs |
|        GetStochRsi |   741.82 μs |  9.198 μs | 21.859 μs |   737.06 μs |
|       GetTripleEma |   364.59 μs |  1.909 μs |  1.692 μs |   364.03 μs |
|            GetTrix |   435.09 μs |  1.952 μs |  1.731 μs |   434.60 μs |
|     GetTrixWithSma |   491.33 μs |  2.010 μs |  1.678 μs |   491.30 μs |
|      GetUlcerIndex | 1,380.28 μs |  6.563 μs |  5.481 μs | 1,378.85 μs |
|        GetUltimate |   600.91 μs |  4.134 μs |  3.452 μs |   602.13 μs |
|          GetVolSma |   121.56 μs |  0.691 μs |  0.613 μs |   121.65 μs |
|        GetWilliamR |   292.65 μs |  1.621 μs |  1.517 μs |   292.32 μs |
|             GetWma |   756.90 μs |  2.817 μs |  2.353 μs |   757.01 μs |
|          GetZigZag |   232.81 μs |  3.206 μs |  2.503 μs |   232.23 μs |

## internal cleaners

|           Method |     Mean |    Error |   StdDev |
|----------------- |---------:|---------:|---------:|
|   PrepareHistory | 42.15 μs | 0.483 μs | 0.452 μs |
| PrepareBasicData | 30.35 μs | 0.221 μs | 0.196 μs |

## internal math functions

| Method | Periods |        Mean |     Error |   StdDev |
|------- |-------- |------------:|----------:|---------:|
| StdDev |      20 |    37.45 ns |  0.291 ns | 0.258 ns |
| StdDev |      50 |    98.09 ns |  0.831 ns | 0.778 ns |
| StdDev |     250 |   534.89 ns |  1.562 ns | 1.220 ns |
| StdDev |    1000 | 2,167.24 ns | 10.419 ns | 9.236 ns |
