---
title: Performance benchmarks
permalink: /performance/
layout: default
---

# {{ page.title }} for v1.23.1

These are the execution times for the current indicators using two years of historical daily stock quotes (502 periods) with default or typical parameters.

``` bash
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1645 (21H2)
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.100-preview.3.22179.4
```

## indicators

|             Method |        Mean |    Error |    StdDev |
|------------------- |------------:|---------:|----------:|
|             GetAdl |    59.39 μs | 0.252 μs |  0.224 μs |
|      GetAdlWithSma |    66.30 μs | 0.519 μs |  0.460 μs |
|             GetAdx |   229.82 μs | 0.915 μs |  0.764 μs |
|       GetAlligator |   167.72 μs | 0.303 μs |  0.236 μs |
|            GetAlma |    61.21 μs | 0.225 μs |  0.188 μs |
|           GetAroon |   139.82 μs | 0.931 μs |  0.826 μs |
|             GetAtr |   154.56 μs | 0.346 μs |  0.289 μs |
|         GetAwesome |    67.54 μs | 0.403 μs |  0.337 μs |
|            GetBeta |   307.10 μs | 1.568 μs |  1.310 μs |
|          GetBetaUp |   335.99 μs | 4.596 μs |  3.838 μs |
|        GetBetaDown |   322.55 μs | 1.940 μs |  1.620 μs |
|         GetBetaAll |   569.74 μs | 6.003 μs |  5.013 μs |
|  GetBollingerBands |   217.19 μs | 1.566 μs |  1.388 μs |
|             GetBop |    68.91 μs | 0.314 μs |  0.279 μs |
|             GetCci |    76.51 μs | 0.393 μs |  0.348 μs |
|      GetChaikinOsc |   108.72 μs | 0.825 μs |  0.772 μs |
|      GetChandelier |   263.37 μs | 2.949 μs |  2.614 μs |
|            GetChop |   123.58 μs | 0.417 μs |  0.370 μs |
|             GetCmf |   121.87 μs | 0.571 μs |  0.477 μs |
|      GetConnorsRsi |   226.91 μs | 0.569 μs |  0.444 μs |
|     GetCorrelation |   152.55 μs | 0.575 μs |  0.449 μs |
|            GetDema |    94.64 μs | 0.410 μs |  0.363 μs |
|            GetDoji |    96.74 μs | 1.690 μs |  1.581 μs |
|        GetDonchian |   298.03 μs | 3.098 μs |  2.898 μs |
|             GetDpo |   145.39 μs | 0.422 μs |  0.352 μs |
|        GetElderRay |   116.37 μs | 0.416 μs |  0.347 μs |
|             GetEma |    54.71 μs | 0.627 μs |  0.556 μs |
|            GetEpma |    90.58 μs | 0.411 μs |  0.321 μs |
|             GetFcb |   340.07 μs | 2.477 μs |  2.069 μs |
| GetFisherTransform |    82.10 μs | 0.973 μs |  0.862 μs |
|      GetForceIndex |    59.70 μs | 0.368 μs |  0.326 μs |
|         GetFractal |    93.09 μs | 1.681 μs |  1.799 μs |
|           GetGator |   216.54 μs | 0.581 μs |  0.485 μs |
|      GetHeikinAshi |   155.47 μs | 1.191 μs |  1.055 μs |
|             GetHma |   277.02 μs | 1.019 μs |  0.903 μs |
|     GetHtTrendline |   164.50 μs | 0.692 μs |  0.578 μs |
|           GetHurst | 1,055.10 μs | 2.993 μs |  2.653 μs |
|        GetIchimoku |   822.77 μs | 1.822 μs |  1.423 μs |
|            GetKama |    73.49 μs | 0.212 μs |  0.198 μs |
|         GetKlinger |    72.51 μs | 0.198 μs |  0.175 μs |
|         GetKeltner |   377.04 μs | 1.297 μs |  1.013 μs |
|            GetMacd |   132.49 μs | 0.612 μs |  0.511 μs |
|     GetMaEnvelopes |    84.29 μs | 0.245 μs |  0.205 μs |
|            GetMama |   131.27 μs | 0.690 μs |  0.611 μs |
|        GetMarubozu |   115.04 μs | 1.588 μs |  1.485 μs |
|             GetMfi |   163.01 μs | 1.143 μs |  0.954 μs |
|             GetObv |    61.37 μs | 0.441 μs |  0.368 μs |
|      GetObvWithSma |    69.70 μs | 0.215 μs |  0.191 μs |
|    GetParabolicSar |    84.97 μs | 0.493 μs |  0.411 μs |
|          GetPivots |   148.83 μs | 0.912 μs |  0.809 μs |
|     GetPivotPoints |    82.34 μs | 0.503 μs |  0.471 μs |
|             GetPmo |    59.55 μs | 0.332 μs |  0.295 μs |
|             GetPrs |    87.86 μs | 0.435 μs |  0.386 μs |
|      GetPrsWithSma |    91.31 μs | 0.212 μs |  0.165 μs |
|             GetPvo |   183.40 μs | 0.407 μs |  0.340 μs |
|           GetRenko |    87.68 μs | 0.252 μs |  0.197 μs |
|        GetRenkoAtr |    93.52 μs | 0.609 μs |  0.540 μs |
|             GetRoc |    46.24 μs | 0.105 μs |  0.088 μs |
|           GetRocWb |    69.23 μs | 0.363 μs |  0.303 μs |
|      GetRocWithSma |    60.94 μs | 1.189 μs |  1.504 μs |
|   GetRollingPivots |   344.17 μs | 3.007 μs |  2.666 μs |
|             GetRsi |    48.49 μs | 0.194 μs |  0.172 μs |
|           GetSlope |    86.35 μs | 0.196 μs |  0.164 μs |
|             GetSma |    77.08 μs | 0.221 μs |  0.184 μs |
|     GetSmaExtended |   141.47 μs | 0.779 μs |  0.651 μs |
|             GetSmi |    94.33 μs | 0.793 μs |  0.703 μs |
|            GetSmma |    80.94 μs | 0.459 μs |  0.383 μs |
|      GetStarcBands |   312.42 μs | 2.395 μs |  2.000 μs |
|             GetStc |   307.96 μs | 1.346 μs |  1.194 μs |
|          GetStdDev |    97.14 μs | 0.914 μs |  0.810 μs |
|   GetStdDevWithSma |   107.64 μs | 0.734 μs |  0.613 μs |
|  GetStdDevChannels |   120.25 μs | 0.771 μs |  0.684 μs |
|           GetStoch |   194.51 μs | 0.560 μs |  0.467 μs |
|       GetStochSMMA |   164.97 μs | 1.229 μs |  1.150 μs |
|        GetStochRsi |   245.07 μs | 2.382 μs |  2.228 μs |
|      GetSuperTrend |   241.19 μs | 0.532 μs |  0.416 μs |
|            GetTema |   130.66 μs | 0.490 μs |  0.458 μs |
|            GetTrix |   181.19 μs | 3.592 μs |  9.771 μs |
|     GetTrixWithSma |   226.15 μs | 1.197 μs |  2.127 μs |
|             GetTsi |    52.73 μs | 0.189 μs |  0.158 μs |
|              GetT3 |    60.10 μs | 0.349 μs |  0.310 μs |
|      GetUlcerIndex |   225.93 μs | 1.038 μs |  0.920 μs |
|        GetUltimate |   104.90 μs | 0.310 μs |  0.259 μs |
|  GetVolatilityStop |   244.41 μs | 1.090 μs |  0.910 μs |
|          GetVortex |    72.01 μs | 0.261 μs |  0.231 μs |
|            GetVwap |    71.08 μs | 0.824 μs |  0.688 μs |
|            GetVwma |    82.14 μs | 0.294 μs |  0.245 μs |
|       GetWilliamsR |   158.08 μs | 0.746 μs |  0.661 μs |
|             GetWma |    78.70 μs | 2.365 μs |  6.899 μs |
|          GetZigZag |   154.31 μs | 4.078 μs | 11.959 μs |
