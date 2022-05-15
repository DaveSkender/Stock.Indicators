---
title: Performance benchmarks
permalink: /performance/
relative_path: performance.md
layout: page
---

# {{ page.title }} for v1.23.4

These are the execution times for the current indicators using two years of historical daily stock quotes (502 periods) with default or typical parameters.

``` bash
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1706 (21H2)
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.100-preview.3.22179.4
```

## indicators

|             Method |        Mean |     Error |    StdDev |      Median |
|------------------- |------------:|----------:|----------:|------------:|
|             GetAdl |    71.38 μs |  1.336 μs |  1.250 μs |    71.28 μs |
|      GetAdlWithSma |    76.86 μs |  1.925 μs |  5.677 μs |    73.33 μs |
|             GetAdx |   230.86 μs |  1.848 μs |  1.728 μs |   230.97 μs |
|       GetAlligator |   169.78 μs |  0.904 μs |  0.846 μs |   169.58 μs |
|            GetAlma |    66.30 μs |  0.545 μs |  0.510 μs |    66.17 μs |
|           GetAroon |   142.03 μs |  0.703 μs |  0.587 μs |   141.96 μs |
|             GetAtr |   152.06 μs |  1.152 μs |  1.021 μs |   151.79 μs |
|         GetAwesome |    68.75 μs |  0.311 μs |  0.260 μs |    68.66 μs |
|            GetBeta |    319.0 us |   2.19 us |   1.83 us |             |
|          GetBetaUp |    339.7 us |   1.62 us |   1.52 us |             |
|        GetBetaDown |    332.6 us |   1.46 us |   1.22 us |             |
|         GetBetaAll |    590.6 us |   5.70 us |   5.05 us |             |
|  GetBollingerBands |   239.74 μs |  7.939 μs | 23.407 μs |   234.53 μs |
|             GetBop |    69.41 μs |  0.648 μs |  0.606 μs |    69.24 μs |
|             GetCci |    76.83 μs |  1.465 μs |  1.439 μs |    76.43 μs |
|      GetChaikinOsc |   107.48 μs |  0.596 μs |  0.528 μs |   107.36 μs |
|      GetChandelier |   260.24 μs |  0.848 μs |  0.708 μs |   260.60 μs |
|            GetChop |   109.88 μs |  0.906 μs |  0.847 μs |   109.47 μs |
|             GetCmf |   132.26 μs |  0.512 μs |  0.427 μs |   132.35 μs |
|      GetConnorsRsi |   219.20 μs |  1.232 μs |  1.152 μs |   218.71 μs |
|     GetCorrelation |   156.42 μs |  1.625 μs |  1.440 μs |   156.04 μs |
|            GetDema |    57.86 μs |  0.359 μs |  0.318 μs |    57.72 μs |
|            GetDoji |   106.74 μs |  1.193 μs |  1.057 μs |   106.65 μs |
|        GetDonchian |   306.15 μs |  1.218 μs |  1.080 μs |   305.94 μs |
|             GetDpo |   151.96 μs |  0.625 μs |  0.522 μs |   152.11 μs |
|        GetElderRay |   116.42 μs |  0.474 μs |  0.420 μs |   116.35 μs |
|             GetEma |    55.25 μs |  0.380 μs |  0.355 μs |    55.08 μs |
|            GetEpma |    90.82 μs |  0.745 μs |  0.697 μs |    90.56 μs |
|             GetFcb |   337.76 μs |  1.326 μs |  1.176 μs |   337.27 μs |
| GetFisherTransform |    81.23 μs |  1.011 μs |  0.896 μs |    80.74 μs |
|      GetForceIndex |    61.87 μs |  0.883 μs |  0.737 μs |    61.62 μs |
|         GetFractal |    95.52 μs |  0.711 μs |  0.665 μs |    95.39 μs |
|           GetGator |   232.89 μs |  0.435 μs |  0.363 μs |   232.89 μs |
|      GetHeikinAshi |   156.89 μs |  0.739 μs |  0.655 μs |   156.89 μs |
|             GetHma |   294.30 μs |  1.350 μs |  1.262 μs |   294.45 μs |
|     GetHtTrendline |   175.52 μs |  0.794 μs |  0.742 μs |   175.29 μs |
|           GetHurst | 1,092.66 μs | 10.152 μs |  8.477 μs | 1,088.49 μs |
|        GetIchimoku |   854.88 μs |  4.577 μs |  4.281 μs |   854.36 μs |
|            GetKama |    77.14 μs |  0.278 μs |  0.232 μs |    77.18 μs |
|         GetKlinger |    74.45 μs |  0.273 μs |  0.228 μs |    74.42 μs |
|         GetKeltner |   390.93 μs |  0.713 μs |  0.596 μs |   390.91 μs |
|            GetMacd |   125.19 μs |  0.748 μs |  0.625 μs |   125.02 μs |
|     GetMaEnvelopes |    93.48 μs |  0.532 μs |  0.497 μs |    93.28 μs |
|            GetMama |   141.35 μs |  0.724 μs |  0.678 μs |   141.17 μs |
|        GetMarubozu |   134.03 μs |  1.133 μs |  1.060 μs |   134.24 μs |
|             GetMfi |   178.25 μs |  1.079 μs |  1.009 μs |   177.93 μs |
|             GetObv |    64.08 μs |  0.716 μs |  0.559 μs |    63.91 μs |
|      GetObvWithSma |    76.06 μs |  0.362 μs |  0.302 μs |    76.00 μs |
|    GetParabolicSar |    88.27 μs |  0.449 μs |  0.420 μs |    88.30 μs |
|          GetPivots |   166.89 μs |  0.539 μs |  0.450 μs |   166.84 μs |
|     GetPivotPoints |    86.63 μs |  0.401 μs |  0.335 μs |    86.53 μs |
|             GetPmo |    63.42 μs |  0.419 μs |  0.349 μs |    63.42 μs |
|             GetPrs |    96.94 μs |  0.329 μs |  0.292 μs |    96.95 μs |
|      GetPrsWithSma |   103.57 μs |  0.263 μs |  0.219 μs |   103.57 μs |
|             GetPvo |   171.20 μs |  1.149 μs |  0.959 μs |   170.95 μs |
|           GetRenko |    90.85 μs |  0.380 μs |  0.356 μs |    90.91 μs |
|        GetRenkoAtr |    96.93 μs |  0.552 μs |  0.517 μs |    96.76 μs |
|             GetRoc |    50.69 μs |  0.202 μs |  0.179 μs |    50.67 μs |
|           GetRocWb |    75.01 μs |  0.614 μs |  0.574 μs |    74.83 μs |
|      GetRocWithSma |    58.86 μs |  0.336 μs |  0.298 μs |    58.80 μs |
|   GetRollingPivots |   358.69 μs |  1.030 μs |  0.913 μs |   358.76 μs |
|             GetRsi |    49.06 μs |  0.330 μs |  0.308 μs |    48.92 μs |
|           GetSlope |    89.70 μs |  0.538 μs |  0.449 μs |    89.70 μs |
|             GetSma |    88.69 μs |  0.455 μs |  0.380 μs |    88.67 μs |
|     GetSmaExtended |   252.39 μs |  4.832 μs |  4.284 μs |   250.85 μs |
|             GetSmi |   108.62 μs |  0.793 μs |  0.662 μs |   108.60 μs |
|            GetSmma |    88.51 μs |  0.652 μs |  0.578 μs |    88.37 μs |
|      GetStarcBands |   325.86 μs |  1.628 μs |  1.522 μs |   326.18 μs |
|             GetStc |   327.72 μs |  1.984 μs |  1.856 μs |   326.71 μs |
|          GetStdDev |   101.17 μs |  0.456 μs |  0.404 μs |   101.09 μs |
|   GetStdDevWithSma |   114.62 μs |  0.866 μs |  0.723 μs |   114.34 μs |
|  GetStdDevChannels |   126.64 μs |  1.297 μs |  1.213 μs |   126.49 μs |
|           GetStoch |   181.36 μs |  1.091 μs |  0.967 μs |   181.27 μs |
|       GetStochSMMA |   168.92 μs |  0.976 μs |  0.913 μs |   169.12 μs |
|        GetStochRsi |   292.13 μs |  2.173 μs |  1.927 μs |   292.49 μs |
|      GetSuperTrend |   250.54 μs |  1.667 μs |  1.477 μs |   250.31 μs |
|            GetTema |    64.12 μs |  0.560 μs |  0.497 μs |    63.88 μs |
|            GetTrix |    55.02 μs |  0.360 μs |  0.337 μs |    54.91 μs |
|     GetTrixWithSma |    59.67 μs |  0.330 μs |  0.293 μs |    59.60 μs |
|             GetTsi |    57.26 μs |  0.284 μs |  0.251 μs |    57.25 μs |
|              GetT3 |    70.34 μs |  0.342 μs |  0.303 μs |    70.31 μs |
|      GetUlcerIndex |   245.57 μs |  1.270 μs |  1.126 μs |   245.47 μs |
|        GetUltimate |   103.86 μs |  1.988 μs |  2.289 μs |   104.20 μs |
|  GetVolatilityStop |   241.59 μs |  1.118 μs |  0.991 μs |   241.71 μs |
|          GetVortex |    71.48 μs |  0.495 μs |  0.413 μs |    71.40 μs |
|            GetVwap |    72.32 μs |  1.418 μs |  1.184 μs |    72.04 μs |
|            GetVwma |    82.65 μs |  0.702 μs |  0.657 μs |    82.54 μs |
|       GetWilliamsR |   145.73 μs |  0.916 μs |  0.812 μs |   145.53 μs |
|             GetWma |    72.61 μs |  0.419 μs |  0.350 μs |    72.55 μs |
|          GetZigZag |   181.94 μs |  3.560 μs |  4.502 μs |   180.22 μs |
