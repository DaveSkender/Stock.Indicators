# Performance benchmarks for v1.1.7

These are the execution times for the current indicators using two years of historical daily stock quotes (502 periods) with default or typical parameters.

``` bash
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19042
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=5.0.100
  [Host]     : .NET Core 5.0.0 (CoreCLR 5.0.20.51904, CoreFX 5.0.20.51904), X64 RyuJIT
  DefaultJob : .NET Core 5.0.0 (CoreCLR 5.0.20.51904, CoreFX 5.0.20.51904), X64 RyuJIT
```

## indicators

|             Method |        Mean |     Error |    StdDev |      Median |
|------------------- |------------:|----------:|----------:|------------:|
|             GetAdl |   137.24 μs |  1.534 μs |  1.281 μs |   137.30 μs |
|      GetAdlWithSma |   376.36 μs |  5.379 μs |  4.768 μs |   374.32 μs |
|             GetAdx |   745.02 μs | 10.035 μs |  8.896 μs |   743.31 μs |
|           GetAroon |   293.20 μs |  3.375 μs |  2.992 μs |   292.89 μs |
|             GetAtr |   156.83 μs |  1.939 μs |  1.813 μs |   156.16 μs |
|            GetBeta |   936.10 μs | 18.649 μs | 47.468 μs |   917.91 μs |
|  GetBollingerBands |   421.22 μs |  2.632 μs |  2.198 μs |   420.23 μs |
|             GetCci |   938.87 μs | 18.736 μs | 38.692 μs |   947.69 μs |
|      GetChaikinOsc |   261.66 μs |  2.004 μs |  1.777 μs |   260.88 μs |
|      GetChandelier |   347.84 μs |  4.633 μs |  5.149 μs |   344.90 μs |
|             GetCmf |   650.66 μs | 11.334 μs | 10.048 μs |   645.57 μs |
|      GetConnorsRsi | 1,239.76 μs | 19.696 μs | 22.682 μs | 1,226.99 μs |
|     GetCorrelation |   798.39 μs |  8.640 μs |  7.659 μs |   795.42 μs |
|        GetDonchian |   311.99 μs |  3.047 μs |  2.702 μs |   311.41 μs |
|       GetDoubleEma |   187.94 μs |  3.722 μs |  7.519 μs |   183.50 μs |
|             GetEma |    99.38 μs |  0.982 μs |  0.919 μs |    98.93 μs |
|      GetHeikinAshi |   168.49 μs |  0.527 μs |  0.467 μs |   168.40 μs |
|             GetHma | 1,376.12 μs | 14.132 μs | 13.219 μs | 1,371.61 μs |
|        GetIchimoku |   791.79 μs | 10.578 μs |  9.895 μs |   786.88 μs |
|         GetKeltner |   464.88 μs |  3.340 μs |  3.124 μs |   463.44 μs |
|            GetMacd |   301.04 μs |  1.449 μs |  1.284 μs |   300.77 μs |
|             GetMfi |   483.38 μs |  5.222 μs |  4.884 μs |   482.29 μs |
|             GetObv |    57.22 μs |  0.360 μs |  0.301 μs |    57.12 μs |
|      GetObvWithSma |   140.01 μs |  2.717 μs |  3.627 μs |   141.21 μs |
|    GetParabolicSar |    92.30 μs |  0.940 μs |  0.785 μs |    92.00 μs |
|             GetPmo |   256.93 μs |  1.358 μs |  1.271 μs |   256.58 μs |
|             GetPrs |   124.44 μs |  1.214 μs |  1.136 μs |   124.66 μs |
|      GetPrsWithSma |   182.98 μs |  3.401 μs |  3.340 μs |   181.38 μs |
|             GetRoc |    90.65 μs |  1.727 μs |  1.848 μs |    89.81 μs |
|      GetRocWithSma |   303.77 μs |  2.919 μs |  2.730 μs |   302.55 μs |
|             GetRsi |   342.30 μs |  3.700 μs |  3.461 μs |   341.25 μs |
|           GetSlope |   853.82 μs |  8.382 μs |  7.840 μs |   848.86 μs |
|             GetSma |   103.24 μs |  0.947 μs |  0.839 μs |   102.81 μs |
|     GetSmaExtended |   823.31 μs |  9.011 μs |  8.429 μs |   818.74 μs |
|          GetStdDev |   286.72 μs |  1.441 μs |  1.203 μs |   286.63 μs |
|   GetStdDevWithSma |   374.83 μs |  3.881 μs |  3.440 μs |   374.10 μs |
|           GetStoch |   341.61 μs |  3.872 μs |  3.622 μs |   339.28 μs |
|        GetStochRsi |   649.50 μs |  2.592 μs |  2.297 μs |   649.88 μs |
|       GetTripleEma |   264.51 μs |  1.983 μs |  1.758 μs |   263.68 μs |
|            GetTrix |   325.91 μs |  1.490 μs |  1.244 μs |   325.41 μs |
|     GetTrixWithSma |   386.88 μs |  3.600 μs |  3.367 μs |   387.34 μs |
|      GetUlcerIndex | 1,362.65 μs | 21.196 μs | 17.700 μs | 1,353.32 μs |
|        GetUltimate |   583.55 μs |  2.676 μs |  2.089 μs |   583.80 μs |
|          GetVolSma |   116.47 μs |  1.187 μs |  1.110 μs |   116.11 μs |
|       GetWilliamsR |   257.74 μs |  0.407 μs |  0.340 μs |   257.77 μs |
|             GetWma |   742.48 μs |  8.612 μs |  8.056 μs |   737.53 μs |
|          GetZigZag |   138.55 μs |  0.722 μs |  0.603 μs |   138.42 μs |

## internal cleaners

|             Method |     Mean |    Error |   StdDev |   Median |
|------------------- |---------:|---------:|---------:|---------:|
|        SortHistory | 37.42 μs | 0.702 μs | 0.751 μs | 37.33 μs |
|    ValidateHistory | 37.37 μs | 0.361 μs | 0.337 μs | 37.42 μs |
| ConvertToBasicData | 44.66 μs | 1.066 μs | 3.025 μs | 43.16 μs |

## internal math functions

| Method | Periods |        Mean |     Error |   StdDev |
|------- |-------- |------------:|----------:|---------:|
| StdDev |      20 |    37.45 ns |  0.291 ns | 0.258 ns |
| StdDev |      50 |    98.09 ns |  0.831 ns | 0.778 ns |
| StdDev |     250 |   534.89 ns |  1.562 ns | 1.220 ns |
| StdDev |    1000 | 2,167.24 ns | 10.419 ns | 9.236 ns |
