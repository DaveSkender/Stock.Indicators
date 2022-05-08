---
title: Performance benchmarks
permalink: /performance/
relative_path: performance.md
layout: page
---

# {{ page.title }} for v1.23.4

These are the execution times for the current indicators using two years of historical daily stock quotes (502 periods) with default or typical parameters.

``` bash
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1645 (21H2)
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.100-preview.3.22179.4
```

## indicators

|             Method |        Mean |     Error |    StdDev |      Median |
|------------------- |------------:|----------:|----------:|------------:|
|             GetAdl |    59.83 μs |  0.391 μs |  0.366 μs |    59.81 μs |
|      GetAdlWithSma |    73.13 μs |  0.460 μs |  0.408 μs |    73.06 μs |
|             GetAdx |   293.98 μs |  1.582 μs |  1.235 μs |   293.76 μs |
|       GetAlligator |   168.43 μs |  1.036 μs |  0.969 μs |   168.38 μs |
|            GetAlma |    68.26 μs |  0.487 μs |  0.432 μs |    68.06 μs |
|           GetAroon |   141.65 μs |  0.384 μs |  0.340 μs |   141.59 μs |
|             GetAtr |   194.47 μs |  1.197 μs |  1.061 μs |   194.08 μs |
|         GetAwesome |    79.44 μs |  0.437 μs |  0.409 μs |    79.45 μs |
|            GetBeta |   385.15 μs |  7.469 μs | 17.459 μs |   375.59 μs |
|          GetBetaUp |   388.15 μs |  2.820 μs |  2.499 μs |   388.00 μs |
|        GetBetaDown |   369.95 μs |  3.296 μs |  2.921 μs |   368.62 μs |
|         GetBetaAll |   771.95 μs | 10.072 μs |  8.928 μs |   769.87 μs |
|  GetBollingerBands |   268.66 μs |  4.258 μs |  3.774 μs |   266.66 μs |
|             GetBop |    73.79 μs |  1.368 μs |  1.280 μs |    73.21 μs |
|             GetCci |   191.19 μs |  1.428 μs |  1.336 μs |   190.65 μs |
|      GetChaikinOsc |   119.13 μs |  1.872 μs |  1.751 μs |   118.45 μs |
|      GetChandelier |   332.09 μs |  6.355 μs |  5.944 μs |   329.57 μs |
|            GetChop |   168.83 μs |  2.284 μs |  2.136 μs |   168.18 μs |
|             GetCmf |   149.01 μs |  2.122 μs |  1.985 μs |   148.82 μs |
|      GetConnorsRsi |   214.23 μs |  1.552 μs |  1.296 μs |   214.52 μs |
|     GetCorrelation |   228.80 μs |  2.288 μs |  2.140 μs |   228.18 μs |
|            GetDema |    63.40 μs |  0.901 μs |  0.843 μs |    63.36 μs |
|            GetDoji |   117.63 μs |  1.608 μs |  1.504 μs |   116.75 μs |
|        GetDonchian |   325.13 μs |  3.476 μs |  3.252 μs |   323.43 μs |
|             GetDpo |   163.31 μs |  1.722 μs |  1.610 μs |   162.85 μs |
|        GetElderRay |   126.16 μs |  1.664 μs |  1.475 μs |   125.64 μs |
|             GetEma |    61.93 μs |  1.127 μs |  2.520 μs |    60.86 μs |
|            GetEpma |    96.63 μs |  1.397 μs |  1.306 μs |    95.98 μs |
|             GetFcb |   439.60 μs |  4.523 μs |  4.231 μs |   438.48 μs |
| GetFisherTransform |   127.32 μs |  1.477 μs |  1.382 μs |   126.94 μs |
|      GetForceIndex |    63.70 μs |  1.202 μs |  1.124 μs |    63.51 μs |
|         GetFractal |   110.35 μs |  1.599 μs |  1.496 μs |   109.74 μs |
|           GetGator |   240.21 μs |  1.687 μs |  1.578 μs |   239.69 μs |
|      GetHeikinAshi |   160.05 μs |  1.039 μs |  0.972 μs |   159.76 μs |
|             GetHma |   320.50 μs |  1.863 μs |  1.651 μs |   319.97 μs |
|     GetHtTrendline |   583.27 μs |  6.498 μs |  6.078 μs |   581.64 μs |
|           GetHurst | 1,935.88 μs | 21.198 μs | 19.829 μs | 1,933.68 μs |
|        GetIchimoku |   965.56 μs |  8.604 μs |  8.049 μs |   964.39 μs |
|            GetKama |   127.68 μs |  1.793 μs |  1.497 μs |   126.99 μs |
|         GetKlinger |    99.37 μs |  1.985 μs |  5.300 μs |   100.57 μs |
|         GetKeltner |   477.38 μs |  7.548 μs |  6.691 μs |   477.18 μs |
|            GetMacd |   132.25 μs |  2.618 μs |  3.015 μs |   132.84 μs |
|     GetMaEnvelopes |   106.46 μs |  2.024 μs |  2.166 μs |   106.10 μs |
|            GetMama |   488.01 μs |  6.204 μs |  5.499 μs |   488.84 μs |
|        GetMarubozu |   150.27 μs |  2.936 μs |  4.571 μs |   149.89 μs |
|             GetMfi |   205.82 μs |  4.085 μs |  4.195 μs |   205.10 μs |
|             GetObv |    71.80 μs |  1.391 μs |  2.165 μs |    71.52 μs |
|      GetObvWithSma |    84.42 μs |  1.639 μs |  1.822 μs |    84.17 μs |
|    GetParabolicSar |   121.56 μs |  1.991 μs |  1.765 μs |   121.80 μs |
|          GetPivots |   194.79 μs |  3.564 μs |  3.160 μs |   195.07 μs |
|     GetPivotPoints |   103.28 μs |  1.964 μs |  2.101 μs |   103.08 μs |
|             GetPmo |    75.10 μs |  1.211 μs |  1.074 μs |    75.32 μs |
|             GetPrs |   113.41 μs |  2.220 μs |  2.077 μs |   113.44 μs |
|      GetPrsWithSma |   117.78 μs |  2.255 μs |  2.214 μs |   118.11 μs |
|             GetPvo |   201.61 μs |  3.111 μs |  2.758 μs |   201.46 μs |
|           GetRenko |    98.40 μs |  1.961 μs |  2.549 μs |    98.36 μs |
|        GetRenkoAtr |   106.29 μs |  1.975 μs |  1.847 μs |   105.83 μs |
|             GetRoc |    53.64 μs |  1.158 μs |  3.413 μs |    51.51 μs |
|           GetRocWb |    73.00 μs |  0.381 μs |  0.338 μs |    72.92 μs |
|      GetRocWithSma |    58.30 μs |  0.214 μs |  0.179 μs |    58.28 μs |
|   GetRollingPivots |   411.03 μs |  2.151 μs |  2.012 μs |   410.66 μs |
|             GetRsi |    47.49 μs |  0.237 μs |  0.210 μs |    47.43 μs |
|           GetSlope |    86.02 μs |  0.192 μs |  0.161 μs |    85.97 μs |
|             GetSma |    86.81 μs |  1.338 μs |  1.251 μs |    86.26 μs |
|     GetSmaExtended |   303.70 μs |  1.408 μs |  1.249 μs |   303.16 μs |
|             GetSmi |   112.68 μs |  0.244 μs |  0.190 μs |   112.71 μs |
|            GetSmma |    86.02 μs |  0.238 μs |  0.211 μs |    86.04 μs |
|      GetStarcBands |   358.81 μs |  1.192 μs |  1.056 μs |   358.53 μs |
|             GetStc |   314.00 μs |  0.644 μs |  0.538 μs |   314.11 μs |
|          GetStdDev |   124.25 μs |  0.542 μs |  0.480 μs |   124.21 μs |
|   GetStdDevWithSma |   138.15 μs |  0.991 μs |  0.879 μs |   137.98 μs |
|  GetStdDevChannels |   120.78 μs |  0.707 μs |  0.661 μs |   120.61 μs |
|           GetStoch |   173.62 μs |  0.526 μs |  0.439 μs |   173.58 μs |
|       GetStochSMMA |   157.98 μs |  2.860 μs |  4.536 μs |   156.09 μs |
|        GetStochRsi |   287.33 μs |  2.377 μs |  2.223 μs |   286.54 μs |
|      GetSuperTrend |   281.25 μs |  1.726 μs |  1.530 μs |   281.21 μs |
|            GetTema |    61.62 μs |  0.203 μs |  0.180 μs |    61.56 μs |
|            GetTrix |    52.39 μs |  0.129 μs |  0.100 μs |    52.38 μs |
|     GetTrixWithSma |    57.23 μs |  0.314 μs |  0.293 μs |    57.10 μs |
|             GetTsi |    70.22 μs |  0.204 μs |  0.181 μs |    70.23 μs |
|              GetT3 |    67.60 μs |  0.338 μs |  0.300 μs |    67.56 μs |
|      GetUlcerIndex |   253.48 μs |  1.664 μs |  1.475 μs |   252.88 μs |
|        GetUltimate |   303.08 μs |  0.582 μs |  0.486 μs |   303.05 μs |
|  GetVolatilityStop |   291.39 μs |  2.531 μs |  2.114 μs |   291.53 μs |
|          GetVortex |    95.88 μs |  0.610 μs |  0.509 μs |    95.73 μs |
|            GetVwap |    70.99 μs |  0.253 μs |  0.212 μs |    71.00 μs |
|            GetVwma |    82.49 μs |  0.645 μs |  0.572 μs |    82.50 μs |
|       GetWilliamsR |   146.99 μs |  0.367 μs |  0.286 μs |   146.93 μs |
|             GetWma |    79.75 μs |  0.476 μs |  0.422 μs |    79.64 μs |
|          GetZigZag |   155.27 μs |  0.433 μs |  0.361 μs |   155.26 μs |
