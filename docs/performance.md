---
title: Performance benchmarks
permalink: /performance/
layout: default
---

# {{ page.title }} for v1.23.0

These are the execution times for the current indicators using two years of historical daily stock quotes (502 periods) with default or typical parameters.

``` bash
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1645 (21H2)
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.100-preview.3.22179.4
```

## indicators

|             Method |        Mean |     Error |    StdDev |      Median |
|------------------- |------------:|----------:|----------:|------------:|
|             GetAdl |    59.29 μs |  0.299 μs |  0.265 μs |    59.24 μs |
|      GetAdlWithSma |    66.55 μs |  1.225 μs |  1.086 μs |    66.28 μs |
|             GetAdx |   257.10 μs |  4.978 μs | 14.443 μs |   249.97 μs |
|       GetAlligator |   182.15 μs |  2.116 μs |  1.876 μs |   182.47 μs |
|            GetAlma |    71.53 μs |  1.353 μs |  1.266 μs |    71.48 μs |
|           GetAroon |   152.24 μs |  3.040 μs |  5.635 μs |   149.92 μs |
|             GetAtr |   167.26 μs |  2.958 μs |  2.470 μs |   167.17 μs |
|         GetAwesome |    74.77 μs |  0.801 μs |  1.013 μs |    74.55 μs |
|            GetBeta |   354.73 μs |  7.716 μs | 22.629 μs |   343.89 μs |
|          GetBetaUp |   362.75 μs |  4.459 μs |  4.171 μs |   360.57 μs |
|        GetBetaDown |   357.38 μs |  5.576 μs |  4.943 μs |   356.32 μs |
|         GetBetaAll |   639.10 μs |  9.953 μs |  9.310 μs |   639.29 μs |
|  GetBollingerBands |   244.02 μs |  2.264 μs |  2.007 μs |   243.85 μs |
|             GetBop |    75.34 μs |  0.934 μs |  0.874 μs |    75.19 μs |
|             GetCci |    83.95 μs |  1.024 μs |  0.908 μs |    83.93 μs |
|      GetChaikinOsc |   125.17 μs |  1.368 μs |  1.280 μs |   124.81 μs |
|      GetChandelier |   285.72 μs |  4.243 μs |  3.543 μs |   284.83 μs |
|            GetChop |   137.77 μs |  2.725 μs |  3.138 μs |   136.90 μs |
|             GetCmf |   136.78 μs |  0.779 μs |  0.691 μs |   136.72 μs |
|      GetConnorsRsi |   251.66 μs |  1.842 μs |  1.723 μs |   251.15 μs |
|     GetCorrelation |   174.41 μs |  2.103 μs |  1.756 μs |   174.70 μs |
|            GetDema |   114.39 μs |  2.263 μs |  3.388 μs |   113.60 μs |
|            GetDoji |   106.38 μs |  1.944 μs |  1.723 μs |   106.22 μs |
|        GetDonchian |   324.79 μs |  6.442 μs |  6.893 μs |   322.47 μs |
|             GetDpo |   166.44 μs |  3.259 μs |  7.289 μs |   165.70 μs |
|        GetElderRay |   128.05 μs |  1.989 μs |  1.764 μs |   127.42 μs |
|             GetEma |    61.51 μs |  1.134 μs |  1.114 μs |    61.09 μs |
|            GetEpma |   100.90 μs |  1.867 μs |  2.795 μs |    99.51 μs |
|             GetFcb |   389.18 μs |  8.034 μs | 23.689 μs |   386.52 μs |
| GetFisherTransform |   102.52 μs |  0.997 μs |  0.933 μs |   102.94 μs |
|      GetForceIndex |    72.45 μs |  1.435 μs |  2.475 μs |    72.69 μs |
|         GetFractal |   111.69 μs |  2.115 μs |  1.978 μs |   112.52 μs |
|           GetGator |   274.61 μs |  5.458 μs |  8.001 μs |   275.53 μs |
|      GetHeikinAshi |   179.17 μs |  3.542 μs |  8.066 μs |   180.96 μs |
|             GetHma |   316.44 μs |  6.276 μs | 14.421 μs |   313.64 μs |
|     GetHtTrendline |   184.61 μs |  3.390 μs |  5.177 μs |   183.27 μs |
|           GetHurst | 1,138.39 μs | 14.870 μs | 12.418 μs | 1,137.51 μs |
|        GetIchimoku |   940.84 μs | 18.523 μs | 36.128 μs |   937.84 μs |
|            GetKama |    97.58 μs |  2.223 μs |  6.556 μs |    98.25 μs |
|         GetKlinger |    88.46 μs |  1.520 μs |  1.690 μs |    88.36 μs |
|         GetKeltner |   443.21 μs |  7.845 μs | 11.743 μs |   446.16 μs |
|            GetMacd |   174.18 μs |  3.435 μs |  3.213 μs |   174.12 μs |
|     GetMaEnvelopes |    93.68 μs |  0.986 μs |  0.923 μs |    93.48 μs |
|            GetMama |   161.82 μs |  2.737 μs |  2.560 μs |   161.78 μs |
|        GetMarubozu |   139.22 μs |  2.637 μs |  2.467 μs |   139.00 μs |
|             GetMfi |   185.35 μs |  3.385 μs |  8.111 μs |   184.59 μs |
|             GetObv |    67.49 μs |  1.272 μs |  1.189 μs |    67.14 μs |
|      GetObvWithSma |    81.28 μs |  1.604 μs |  3.521 μs |    80.96 μs |
|    GetParabolicSar |    89.53 μs |  1.922 μs |  5.666 μs |    86.02 μs |
|          GetPivots |   150.53 μs |  0.773 μs |  0.645 μs |   150.63 μs |
|     GetPivotPoints |    85.28 μs |  0.631 μs |  0.527 μs |    85.17 μs |
|             GetPmo |    68.55 μs |  0.242 μs |  0.202 μs |    68.53 μs |
|             GetPrs |    92.39 μs |  0.329 μs |  0.275 μs |    92.33 μs |
|      GetPrsWithSma |   106.57 μs |  2.117 μs |  5.651 μs |   106.40 μs |
|             GetPvo |   196.76 μs |  0.986 μs |  0.823 μs |   196.70 μs |
|           GetRenko |    88.32 μs |  0.410 μs |  0.384 μs |    88.23 μs |
|        GetRenkoAtr |   105.79 μs |  1.964 μs |  2.939 μs |   105.83 μs |
|             GetRoc |    54.64 μs |  1.090 μs |  1.909 μs |    54.50 μs |
|           GetRocWb |    81.08 μs |  1.524 μs |  2.628 μs |    80.12 μs |
|      GetRocWithSma |    71.69 μs |  1.413 μs |  2.438 μs |    71.54 μs |
|   GetRollingPivots |   374.07 μs |  2.711 μs |  2.403 μs |   374.08 μs |
|             GetRsi |    52.98 μs |  1.056 μs |  2.820 μs |    51.42 μs |
|           GetSlope |    85.16 μs |  0.564 μs |  0.500 μs |    84.98 μs |
|             GetSma |    79.24 μs |  0.367 μs |  0.307 μs |    79.20 μs |
|     GetSmaExtended |   146.63 μs |  0.448 μs |  0.374 μs |   146.66 μs |
|             GetSmi |    94.90 μs |  0.774 μs |  0.724 μs |    94.53 μs |
|            GetSmma |    82.50 μs |  0.252 μs |  0.210 μs |    82.42 μs |
|      GetStarcBands |   312.94 μs |  1.507 μs |  1.258 μs |   312.67 μs |
|             GetStc |   333.73 μs |  2.920 μs |  2.438 μs |   334.47 μs |
|          GetStdDev |    98.46 μs |  0.282 μs |  0.236 μs |    98.40 μs |
|   GetStdDevWithSma |   108.74 μs |  0.457 μs |  0.382 μs |   108.68 μs |
|  GetStdDevChannels |   120.30 μs |  0.733 μs |  0.650 μs |   120.04 μs |
|           GetStoch |   217.28 μs |  4.267 μs |  5.241 μs |   217.48 μs |
|       GetStochSMMA |   167.51 μs |  2.459 μs |  3.020 μs |   166.51 μs |
|        GetStochRsi |   253.49 μs |  0.958 μs |  0.850 μs |   253.45 μs |
|      GetSuperTrend |   250.58 μs |  1.925 μs |  1.800 μs |   250.03 μs |
|            GetTema |   143.35 μs |  0.903 μs |  0.800 μs |   143.15 μs |
|            GetTrix |   196.70 μs |  3.922 μs |  7.072 μs |   195.55 μs |
|     GetTrixWithSma |   257.05 μs |  5.036 μs |  8.133 μs |   255.56 μs |
|             GetTsi |    64.09 μs |  1.221 μs |  3.042 μs |    63.92 μs |
|              GetT3 |    72.22 μs |  1.418 μs |  2.408 μs |    72.90 μs |
|      GetUlcerIndex |   225.74 μs |  4.260 μs |  4.906 μs |   226.14 μs |
|        GetUltimate |   116.34 μs |  2.245 μs |  2.997 μs |   115.52 μs |
|  GetVolatilityStop |   244.42 μs |  1.029 μs |  0.859 μs |   244.24 μs |
|          GetVortex |    71.76 μs |  0.179 μs |  0.140 μs |    71.80 μs |
|            GetVwap |    70.58 μs |  0.179 μs |  0.140 μs |    70.61 μs |
|            GetVwma |    81.93 μs |  0.309 μs |  0.258 μs |    81.98 μs |
|       GetWilliamsR |   157.88 μs |  1.258 μs |  1.050 μs |   157.53 μs |
|             GetWma |    71.40 μs |  0.308 μs |  0.257 μs |    71.32 μs |
|          GetZigZag |   138.37 μs |  1.142 μs |  1.068 μs |   137.82 μs |
