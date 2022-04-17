---
title: Performance benchmarks
permalink: /performance/
layout: default
---

# {{ page.title }} for v1.20.1

These are the execution times for the current indicators using two years of historical daily stock quotes (502 periods) with default or typical parameters.

``` bash
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1348 (21H2)
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.100
```

## indicators

|             Method |        Mean |    Error |   StdDev |
|------------------- |------------:|---------:|---------:|
|             GetAdl |    59.68 μs | 0.612 μs | 0.511 μs |
|      GetAdlWithSma |    67.30 μs | 0.374 μs | 0.312 μs |
|             GetAdx |   227.22 μs | 1.261 μs | 1.179 μs |
|       GetAlligator |   167.23 μs | 0.986 μs | 0.874 μs |
|            GetAlma |    67.03 μs | 0.328 μs | 0.310 μs |
|           GetAroon |   142.20 μs | 1.460 μs | 0.931 μs |
|             GetAtr |   154.96 μs | 0.970 μs | 0.810 μs |
|         GetAwesome |    73.39 μs | 0.325 μs | 0.289 μs |
|            GetBeta |   221.70 us | 1.780 us | 1.580 us |
|          GetBetaUp |   247.50 us | 4.910 us | 5.050 us |
|        GetBetaDown |   238.00 us | 4.040 us | 3.580 us |
|         GetBetaAll |   510.90 us | 9.090 us | 8.500 us |
|  GetBollingerBands |   234.42 μs | 2.059 μs | 1.926 μs |
|             GetBop |    68.98 μs | 0.377 μs | 0.352 μs |
|             GetCci |    84.67 μs | 0.498 μs | 0.465 μs |
|      GetChaikinOsc |   120.65 μs | 0.722 μs | 0.603 μs |
|      GetChandelier |   261.32 μs | 1.992 μs | 1.766 μs |
|            GetChop |   123.77 μs | 2.411 μs | 2.476 μs |
|             GetCmf |   127.10 μs | 1.330 μs | 1.179 μs |
|      GetConnorsRsi |   241.61 μs | 1.411 μs | 1.178 μs |
|     GetCorrelation |   161.66 μs | 0.403 μs | 0.336 μs |
|        GetDonchian |   306.92 μs | 2.007 μs | 1.779 μs |
|       GetDema |   100.55 μs | 0.709 μs | 0.592 μs |
|             GetDpo |   148.78 μs | 0.998 μs | 0.833 μs |
|        GetElderRay |   117.82 μs | 0.587 μs | 0.520 μs |
|             GetEma |    56.83 μs | 0.394 μs | 0.350 μs |
|            GetEpma |   100.15 μs | 0.331 μs | 0.276 μs |
|             GetFcb |   347.82 μs | 2.644 μs | 2.473 μs |
| GetFisherTransform |    89.40 μs | 0.474 μs | 0.420 μs |
|      GetForceIndex |    59.93 μs | 0.317 μs | 0.296 μs |
|         GetFractal |    95.72 μs | 0.244 μs | 0.190 μs |
|           GetGator |   215.27 μs | 3.762 μs | 3.141 μs |
|      GetHeikinAshi |   175.03 μs | 0.752 μs | 0.704 μs |
|             GetHma |   272.68 μs | 2.637 μs | 2.338 μs |
|     GetHtTrendline |   172.83 μs | 0.517 μs | 0.432 μs |
|           GetHurst | 1,011.27 μs | 6.046 μs | 5.359 μs |
|        GetIchimoku |   873.87 μs | 4.815 μs | 4.504 μs |
|            GetKama |    73.26 μs | 0.215 μs | 0.179 μs |
|         GetKlinger |    72.53 μs | 0.358 μs | 0.299 μs |
|         GetKeltner |   380.42 μs | 1.897 μs | 1.682 μs |
|            GetMacd |   143.73 μs | 0.983 μs | 0.821 μs |
|     GetMaEnvelopes |    86.72 μs | 0.262 μs | 0.218 μs |
|            GetMama |   139.66 μs | 0.942 μs | 0.787 μs |
|        GetMarubozu |   121.28 μs | 0.531 μs | 0.471 μs |
|             GetMfi |   170.07 μs | 1.090 μs | 0.910 μs |
|             GetObv |    62.35 μs | 0.243 μs | 0.215 μs |
|      GetObvWithSma |    68.61 μs | 0.340 μs | 0.284 μs |
|    GetParabolicSar |    89.93 μs | 0.345 μs | 0.306 μs |
|          GetPivots |   152.28 μs | 0.719 μs | 0.637 μs |
|     GetPivotPoints |    89.46 μs | 1.640 μs | 1.454 μs |
|             GetPmo |    72.27 μs | 0.722 μs | 0.640 μs |
|             GetPrs |    99.20 μs | 0.969 μs | 0.859 μs |
|      GetPrsWithSma |   104.46 μs | 0.530 μs | 0.443 μs |
|             GetPvo |   202.49 μs | 0.312 μs | 0.292 μs |
|           GetRenko |    94.08 μs | 0.379 μs | 0.336 μs |
|        GetRenkoAtr |   106.52 μs | 0.576 μs | 0.539 μs |
|             GetRoc |    51.75 μs | 0.234 μs | 0.208 μs |
|           GetRocWb |    72.85 μs | 0.723 μs | 0.604 μs |
|      GetRocWithSma |    64.66 μs | 0.605 μs | 0.537 μs |
|   GetRollingPivots |   327.67 μs | 1.436 μs | 1.344 μs |
|             GetRsi |    53.78 μs | 0.559 μs | 0.523 μs |
|           GetSlope |    89.19 μs | 0.617 μs | 0.547 μs |
|             GetSma |    83.13 μs | 0.216 μs | 0.202 μs |
|     GetSmaExtended |   161.42 μs | 1.721 μs | 1.610 μs |
|             GetSmi |    97.64 μs | 0.958 μs | 0.800 μs |
|            GetSmma |    87.28 μs | 0.452 μs | 0.401 μs |
|      GetStarcBands |   326.62 μs | 2.351 μs | 2.084 μs |
|             GetStc |   350.88 μs | 3.121 μs | 2.767 μs |
|          GetStdDev |    99.60 μs | 0.198 μs | 0.185 μs |
|   GetStdDevWithSma |   108.41 μs | 0.873 μs | 0.774 μs |
|  GetStdDevChannels |   132.02 μs | 0.365 μs | 0.305 μs |
|           GetStoch |   190.20 μs | 2.084 μs | 1.949 μs |
|       GetStochSMMA |   168.71 μs | 1.463 μs | 1.222 μs |
|        GetStochRsi |   248.17 μs | 1.562 μs | 1.385 μs |
|      GetSuperTrend |   250.22 μs | 1.499 μs | 1.329 μs |
|       GetTema |   145.27 μs | 0.634 μs | 0.593 μs |
|            GetTrix |   184.90 μs | 3.073 μs | 2.724 μs |
|     GetTrixWithSma |   240.74 μs | 2.872 μs | 2.687 μs |
|             GetTsi |    59.69 μs | 0.749 μs | 0.626 μs |
|              GetT3 |    68.81 μs | 0.475 μs | 0.421 μs |
|      GetUlcerIndex |   236.90 μs | 0.593 μs | 0.555 μs |
|        GetUltimate |   110.23 μs | 0.766 μs | 0.679 μs |
|  GetVolatilityStop |   255.43 μs | 3.474 μs | 3.080 μs |
|          GetVolSma |   158.92 μs | 1.231 μs | 1.152 μs |
|          GetVortex |    72.22 μs | 0.549 μs | 0.487 μs |
|            GetVwap |    73.41 μs | 0.248 μs | 0.207 μs |
|            GetVwma |    88.79 μs | 0.417 μs | 0.390 μs |
|       GetWilliamsR |   154.25 μs | 1.395 μs | 1.165 μs |
|             GetWma |    68.90 μs | 0.380 μs | 0.317 μs |
|          GetZigZag |   140.25 μs | 0.718 μs | 0.637 μs |
