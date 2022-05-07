---
title: Performance benchmarks
permalink: /performance/
relative_path: performance.md
layout: page
---

# {{ page.title }} for v1.23.1

These are the execution times for the current indicators using two years of historical daily stock quotes (502 periods) with default or typical parameters.

``` bash
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1645 (21H2)
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.100-preview.3.22179.4
```

## indicators

|             Method |        Mean |     Error |    StdDev |      Median |
|------------------- |------------:|----------:|----------:|------------:|
|             GetAdl |    66.62 μs |  0.380 μs |  0.337 μs |    66.52 μs |
|      GetAdlWithSma |    75.92 μs |  0.332 μs |  0.295 μs |    75.82 μs |
|             GetAdx |   296.09 μs |  1.766 μs |  1.379 μs |   296.07 μs |
|       GetAlligator |   167.43 μs |  0.919 μs |  0.859 μs |   167.03 μs |
|            GetAlma |    70.66 μs |  0.351 μs |  0.293 μs |    70.63 μs |
|           GetAroon |   157.27 μs |  0.358 μs |  0.280 μs |   157.34 μs |
|             GetAtr |   188.57 μs |  0.639 μs |  0.566 μs |   188.45 μs |
|         GetAwesome |    79.11 μs |  0.257 μs |  0.227 μs |    79.10 μs |
|            GetBeta |   374.35 μs |  1.364 μs |  1.065 μs |   374.24 μs |
|          GetBetaUp |   385.70 μs |  1.529 μs |  1.355 μs |   385.77 μs |
|        GetBetaDown |   365.05 μs |  0.805 μs |  0.672 μs |   364.91 μs |
|         GetBetaAll |   718.63 μs |  4.417 μs |  3.916 μs |   717.31 μs |
|  GetBollingerBands |   314.12 μs |  9.200 μs | 26.397 μs |   319.38 μs |
|             GetBop |    98.77 μs |  1.959 μs |  3.107 μs |    98.42 μs |
|             GetCci |   227.57 μs |  3.560 μs |  3.156 μs |   227.59 μs |
|      GetChaikinOsc |   145.21 μs |  1.805 μs |  1.409 μs |   145.87 μs |
|      GetChandelier |   386.93 μs |  5.976 μs |  5.590 μs |   386.23 μs |
|            GetChop |   197.25 μs |  2.952 μs |  2.761 μs |   197.49 μs |
|             GetCmf |   177.85 μs |  2.625 μs |  2.578 μs |   177.33 μs |
|      GetConnorsRsi |   275.01 μs |  2.521 μs |  2.105 μs |   274.96 μs |
|     GetCorrelation |   267.60 μs |  5.164 μs |  4.831 μs |   268.24 μs |
|            GetDema |    72.95 μs |  0.899 μs |  0.751 μs |    73.15 μs |
|            GetDoji |   144.59 μs |  2.768 μs |  3.501 μs |   143.81 μs |
|        GetDonchian |   377.27 μs |  7.450 μs | 13.993 μs |   375.20 μs |
|             GetDpo |   188.26 μs |  3.755 μs |  6.478 μs |   187.14 μs |
|        GetElderRay |   147.04 μs |  2.856 μs |  4.361 μs |   146.88 μs |
|             GetEma |    71.79 μs |  1.425 μs |  3.006 μs |    71.89 μs |
|            GetEpma |   148.45 μs |  2.949 μs |  5.392 μs |   148.42 μs |
|             GetFcb |   460.78 μs |  8.722 μs | 19.688 μs |   463.56 μs |
| GetFisherTransform |   152.96 μs |  2.741 μs |  3.659 μs |   153.82 μs |
|      GetForceIndex |    82.72 μs |  1.620 μs |  2.272 μs |    82.67 μs |
|         GetFractal |   124.31 μs |  2.456 μs |  5.789 μs |   123.67 μs |
|           GetGator |   261.78 μs |  5.089 μs |  8.361 μs |   261.63 μs |
|      GetHeikinAshi |   240.07 μs |  4.757 μs | 11.936 μs |   243.50 μs |
|             GetHma |   399.21 μs |  7.026 μs |  8.886 μs |   398.97 μs |
|     GetHtTrendline |   641.51 μs | 12.535 μs | 12.311 μs |   637.08 μs |
|           GetHurst | 2,117.94 μs | 12.550 μs | 11.739 μs | 2,121.38 μs |
|        GetIchimoku | 1,094.54 μs | 11.918 μs | 11.148 μs | 1,095.23 μs |
|            GetKama |   129.64 μs |  1.527 μs |  1.500 μs |   129.21 μs |
|         GetKlinger |   105.39 μs |  2.074 μs |  3.740 μs |   106.44 μs |
|         GetKeltner |   502.41 μs |  4.767 μs |  4.459 μs |   502.70 μs |
|            GetMacd |   141.94 μs |  1.382 μs |  1.292 μs |   141.71 μs |
|     GetMaEnvelopes |   102.15 μs |  2.039 μs |  5.152 μs |    98.60 μs |
|            GetMama |   501.65 μs |  5.240 μs |  4.376 μs |   502.09 μs |
|        GetMarubozu |   161.35 μs |  1.289 μs |  1.143 μs |   161.66 μs |
|             GetMfi |   218.62 μs |  0.881 μs |  0.824 μs |   218.65 μs |
|             GetObv |    77.14 μs |  1.518 μs |  1.420 μs |    77.49 μs |
|      GetObvWithSma |    89.55 μs |  1.784 μs |  2.055 μs |    90.21 μs |
|    GetParabolicSar |   108.18 μs |  0.514 μs |  0.429 μs |   108.00 μs |
|          GetPivots |   176.60 μs |  1.158 μs |  1.084 μs |   176.36 μs |
|     GetPivotPoints |    93.75 μs |  1.217 μs |  1.138 μs |    93.22 μs |
|             GetPmo |    74.50 μs |  1.482 μs |  2.172 μs |    75.41 μs |
|             GetPrs |   109.62 μs |  0.613 μs |  0.574 μs |   109.50 μs |
|      GetPrsWithSma |   111.83 μs |  2.232 μs |  6.441 μs |   107.86 μs |
|             GetPvo |   200.41 μs |  2.062 μs |  1.928 μs |   200.64 μs |
|           GetRenko |   121.43 μs |  2.286 μs |  2.139 μs |   121.70 μs |
|        GetRenkoAtr |   119.53 μs |  2.382 μs |  4.109 μs |   118.29 μs |
|             GetRoc |    54.92 μs |  0.493 μs |  0.437 μs |    54.84 μs |
|           GetRocWb |    77.31 μs |  0.519 μs |  0.460 μs |    77.13 μs |
|      GetRocWithSma |    68.77 μs |  1.093 μs |  0.969 μs |    68.99 μs |
|   GetRollingPivots |   474.20 μs |  4.078 μs |  3.815 μs |   474.67 μs |
|             GetRsi |    61.74 μs |  1.214 μs |  1.925 μs |    60.68 μs |
|           GetSlope |   138.64 μs |  0.465 μs |  0.412 μs |   138.45 μs |
|             GetSma |    91.80 μs |  0.648 μs |  0.506 μs |    91.59 μs |
|     GetSmaExtended |   356.09 μs |  5.748 μs |  5.377 μs |   357.08 μs |
|             GetSmi |   121.38 μs |  2.147 μs |  4.136 μs |   119.19 μs |
|            GetSmma |    92.08 μs |  0.467 μs |  0.414 μs |    92.04 μs |
|      GetStarcBands |   424.51 μs |  8.355 μs | 13.491 μs |   427.57 μs |
|             GetStc |   355.72 μs |  6.463 μs |  6.045 μs |   353.85 μs |
|          GetStdDev |   132.42 μs |  0.805 μs |  0.713 μs |   132.35 μs |
|   GetStdDevWithSma |   153.01 μs |  3.032 μs |  6.656 μs |   149.41 μs |
|  GetStdDevChannels |   168.93 μs |  0.848 μs |  0.752 μs |   168.78 μs |
|           GetStoch |   210.24 μs |  2.375 μs |  3.554 μs |   208.99 μs |
|       GetStochSMMA |   184.92 μs |  0.340 μs |  0.265 μs |   184.96 μs |
|        GetStochRsi |   302.66 μs |  6.020 μs | 10.059 μs |   299.87 μs |
|      GetSuperTrend |   287.71 μs |  2.722 μs |  2.546 μs |   286.72 μs |
|            GetTema |    62.77 μs |  0.342 μs |  0.285 μs |    62.71 μs |
|            GetTrix |    53.48 μs |  0.301 μs |  0.282 μs |    53.35 μs |
|     GetTrixWithSma |    58.61 μs |  0.337 μs |  0.315 μs |    58.49 μs |
|             GetTsi |    71.80 μs |  0.279 μs |  0.233 μs |    71.76 μs |
|              GetT3 |    69.35 μs |  0.423 μs |  0.395 μs |    69.32 μs |
|      GetUlcerIndex |   254.11 μs |  1.717 μs |  1.522 μs |   253.43 μs |
|        GetUltimate |   310.61 μs |  0.974 μs |  0.814 μs |   310.45 μs |
|  GetVolatilityStop |   288.53 μs |  4.148 μs |  3.880 μs |   287.93 μs |
|          GetVortex |   103.74 μs |  0.555 μs |  0.463 μs |   103.62 μs |
|            GetVwap |    78.00 μs |  0.967 μs |  0.857 μs |    77.52 μs |
|            GetVwma |    93.67 μs |  0.568 μs |  0.504 μs |    93.58 μs |
|       GetWilliamsR |   169.70 μs |  0.644 μs |  0.537 μs |   169.82 μs |
|             GetWma |    79.10 μs |  0.518 μs |  0.433 μs |    78.99 μs |
|          GetZigZag |   153.30 μs |  2.952 μs |  4.327 μs |   151.85 μs |
