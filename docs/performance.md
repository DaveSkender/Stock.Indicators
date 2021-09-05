---
title: Performance benchmarks for v1.15.0
permalink: /performance/
layout: default
redirect_from:
 - /tests/performance
 - /tests/performance/
---

# {{ page.title }}

These are the execution times for the current indicators using two years of historical daily stock quotes (502 periods) with default or typical parameters.

``` bash
BenchmarkDotNet=v0.13.0, OS=Windows 10.0.19043.1165 (21H1/May2021Update)
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK=5.0.303
  [Host]     : .NET 5.0.9 (5.0.921.35908), X64 RyuJIT
  DefaultJob : .NET 5.0.9 (5.0.921.35908), X64 RyuJIT
```

## indicators

|             Method |        Mean |     Error |    StdDev |      Median |
|------------------- |------------:|----------:|----------:|------------:|
|             GetAdl |   142.97 μs |  0.497 μs |  0.388 μs |   142.94 μs |
|      GetAdlWithSma |   381.47 μs |  3.150 μs |  3.093 μs |   380.48 μs |
|             GetAdx |   751.34 μs |  2.972 μs |  2.482 μs |   750.57 μs |
|       GetAlligator |   234.74 μs |  1.240 μs |  1.160 μs |   234.56 μs |
|            GetAlma |   215.96 μs |  1.546 μs |  1.370 μs |   215.42 μs |
|           GetAroon |   353.01 μs |  0.803 μs |  0.670 μs |   352.93 μs |
|             GetAtr |   159.49 μs |  1.414 μs |  1.254 μs |   159.12 μs |
|         GetAwesome |   330.95 μs |  1.490 μs |  1.321 μs |   331.14 μs |
|            GetBeta |   959.70 μs |  2.365 μs |  1.975 μs |   959.47 μs |
|  GetBollingerBands |   457.88 μs |  1.584 μs |  1.323 μs |   457.95 μs |
|             GetBop |   282.34 μs |  1.319 μs |  1.030 μs |   282.14 μs |
|             GetCci |   843.90 μs |  2.398 μs |  2.003 μs |   843.66 μs |
|      GetChaikinOsc |   268.11 μs |  0.989 μs |  0.826 μs |   268.18 μs |
|      GetChandelier |   368.09 μs |  3.612 μs |  2.820 μs |   367.56 μs |
|            GetChop |   304.53 μs |  1.161 μs |  0.906 μs |   304.36 μs |
|             GetCmf |   669.74 μs |  1.981 μs |  1.655 μs |   669.63 μs |
|      GetConnorsRsi | 1,235.14 μs | 21.665 μs | 43.764 μs | 1,212.54 μs |
|     GetCorrelation |   870.27 μs |  3.443 μs |  2.875 μs |   869.54 μs |
|        GetDonchian |   340.71 μs |  3.841 μs |  3.207 μs |   339.06 μs |
|       GetDoubleEma |   178.53 μs |  0.528 μs |  0.441 μs |   178.45 μs |
|        GetElderRay |   165.20 μs |  0.599 μs |  0.500 μs |   165.24 μs |
|             GetEma |   100.30 μs |  0.622 μs |  0.582 μs |   100.25 μs |
|            GetEpma | 1,384.73 μs |  3.229 μs |  2.863 μs | 1,384.37 μs |
|             GetFcb |   392.19 μs |  2.354 μs |  2.202 μs |   391.52 μs |
| GetFisherTransform |   280.99 μs |  0.783 μs |  0.694 μs |   280.92 μs |
|      GetForceIndex |   127.98 μs |  0.682 μs |  0.604 μs |   127.83 μs |
|         GetFractal |   104.33 μs |  0.469 μs |  0.392 μs |   104.32 μs |
|           GetGator |   285.22 μs |  1.183 μs |  0.923 μs |   284.83 μs |
|      GetHeikinAshi |   180.60 μs |  4.060 μs | 11.046 μs |   175.51 μs |
|             GetHma | 1,381.84 μs |  3.634 μs |  3.035 μs | 1,381.41 μs |
|     GetHtTrendline |   174.17 μs |  1.182 μs |  1.048 μs |   173.99 μs |
|           GetHurst | 5,577.92 μs | 21.799 μs | 20.390 μs | 5,567.51 μs |
|        GetIchimoku |   938.48 μs |  3.479 μs |  3.084 μs |   938.72 μs |
|            GetKama |   329.93 μs |  1.653 μs |  1.466 μs |   329.66 μs |
|         GetKlinger |   494.76 μs |  1.875 μs |  1.565 μs |   494.16 μs |
|         GetKeltner |   472.58 μs |  2.184 μs |  2.042 μs |   471.92 μs |
|            GetMacd |   217.86 μs |  1.426 μs |  1.264 μs |   217.51 μs |
|     GetMaEnvelopes |   148.26 μs |  0.713 μs |  0.632 μs |   148.01 μs |
|            GetMama |   287.17 μs |  2.075 μs |  1.620 μs |   286.68 μs |
|             GetMfi |   485.62 μs |  1.907 μs |  1.690 μs |   485.18 μs |
|             GetObv |    62.31 μs |  0.372 μs |  0.330 μs |    62.31 μs |
|      GetObvWithSma |   140.16 μs |  1.793 μs |  1.677 μs |   139.93 μs |
|    GetParabolicSar |    94.90 μs |  0.533 μs |  0.472 μs |    94.89 μs |
|     GetPivotPoints |    97.13 μs |  0.690 μs |  0.612 μs |    97.04 μs |
|             GetPmo |   265.15 μs |  1.907 μs |  1.784 μs |   264.94 μs |
|             GetPrs |   133.24 μs |  1.900 μs |  1.586 μs |         N/A |
|      GetPrsWithSma |   205.19 μs |  1.452 μs |  1.287 μs |         N/A |
|             GetPvo |   343.26 μs |  4.831 μs |  4.034 μs |   341.35 μs |
|           GetRenko |    94.18 μs |  0.328 μs |  0.256 μs |    94.18 μs |
|        GetRenkoAtr |   101.58 μs |  0.526 μs |  0.439 μs |   101.42 μs |
|             GetRoc |    94.60 μs |  0.350 μs |  0.310 μs |    94.58 μs |
|           GetRocWb |   200.73 μs |  0.623 μs |  0.552 μs |   200.64 μs |
|      GetRocWithSma |   355.20 μs |  1.624 μs |  1.519 μs |   354.83 μs |
|             GetRsi |   340.61 μs |  1.052 μs |  0.821 μs |   340.70 μs |
|           GetSlope |   880.85 μs |  5.654 μs |  5.012 μs |   879.30 μs |
|             GetSma |   107.26 μs |  0.363 μs |  0.303 μs |   107.21 μs |
|     GetSmaExtended |   942.89 μs |  5.649 μs |  5.284 μs |   940.02 μs |
|            GetSmma |    96.63 μs |  0.469 μs |  0.439 μs |    96.42 μs |
|      GetStarcBands |   418.00 μs |  1.100 μs |  0.918 μs |   417.84 μs |
|          GetStdDev |   296.16 μs |  0.870 μs |  0.726 μs |   296.26 μs |
|   GetStdDevWithSma |   383.25 μs |  2.206 μs |  2.063 μs |   382.67 μs |
|  GetStdDevChannels |   947.06 μs |  2.845 μs |  2.376 μs |   947.12 μs |
|           GetStoch |   403.67 μs |  1.003 μs |  0.838 μs |   403.38 μs |
|        GetStochRsi |   708.34 μs |  2.750 μs |  2.296 μs |   707.45 μs |
|      GetSuperTrend |   301.10 μs |  0.886 μs |  0.692 μs |   301.20 μs |
|       GetTripleEma |   260.73 μs |  0.781 μs |  0.692 μs |   260.85 μs |
|            GetTrix |   319.06 μs |  0.923 μs |  0.771 μs |   319.08 μs |
|     GetTrixWithSma |   374.94 μs |  0.969 μs |  0.757 μs |   375.28 μs |
|             GetTsi |   371.20 μs |  1.001 μs |  0.836 μs |   371.11 μs |
|              GetT3 |   464.78 μs |  0.754 μs |  0.668 μs |   464.94 μs |
|      GetUlcerIndex | 1,513.72 μs | 20.189 μs | 19.828 μs | 1,502.68 μs |
|        GetUltimate |   555.05 μs |  2.761 μs |  2.306 μs |   554.72 μs |
|          GetVolSma |   120.00 μs |  0.459 μs |  0.430 μs |   119.86 μs |
|          GetVortex |   282.67 μs |  0.636 μs |  0.564 μs |   282.51 μs |
|            GetVwap |    98.52 μs |  0.734 μs |  0.573 μs |    98.41 μs |
|       GetWilliamsR |   295.51 μs |  1.869 μs |  1.459 μs |   295.22 μs |
|             GetWma |   734.43 μs |  4.447 μs |  3.942 μs |   733.00 μs |
|          GetZigZag |   147.16 μs |  0.613 μs |  0.512 μs |   147.16 μs |

## quotes functions (mostly internal)

|          Method |         Mean |      Error |     StdDev |
|---------------- |-------------:|-----------:|-----------:|
|            Sort | 37,768.62 ns | 406.995 ns | 360.790 ns |
|        Validate | 40,457.78 ns | 301.177 ns | 266.985 ns |
|       Aggregate |     83.36 ns |   0.699 ns |   0.545 ns |
|  ConvertToBasic | 42,362.26 ns | 144.200 ns | 120.414 ns |
| ConvertToQuotes |  8,378.83 ns |  71.755 ns |  63.609 ns |

## math functions (internal)

| Method | Periods |        Mean |    Error |   StdDev |
|------- |-------- |------------:|---------:|---------:|
| StdDev |      20 |    36.84 ns | 0.194 ns | 0.172 ns |
| StdDev |      50 |    95.47 ns | 0.306 ns | 0.256 ns |
| StdDev |     250 |   530.23 ns | 1.303 ns | 1.088 ns |
| StdDev |    1000 | 2,142.94 ns | 5.994 ns | 5.313 ns |
