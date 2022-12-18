---
title: Performance benchmarks
description: The Stock Indicators for .NET library is built for speed and production workloads.  Compare our execution times with other options.
permalink: /performance/
relative_path: performance.md
layout: page
---

# {{ page.title }} for v2.4.3

These are the execution times for the current indicators using two years of historical daily stock quotes (502 periods) with default or typical parameters.

``` bash
BenchmarkDotNet=v0.13.2, OS=Windows 10 (10.0.19045.2364)
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.101
```

## indicators

|             Method |         Mean |      Error |     StdDev |
|------------------- |-------------:|-----------:|-----------:|
|             GetAdl |    60.455 μs |  0.5054 μs |  0.4727 μs |
|      GetAdlWithSma |    74.337 μs |  0.7889 μs |  0.7380 μs |
|             GetAdx |    72.515 μs |  0.5760 μs |  0.5106 μs |
|       GetAlligator |    69.575 μs |  1.0438 μs |  1.0720 μs |
|            GetAlma |    60.993 μs |  0.4465 μs |  0.4176 μs |
|           GetAroon |   120.196 μs |  0.8537 μs |  0.6665 μs |
|             GetAtr |    62.853 μs |  0.5032 μs |  0.4707 μs |
|         GetAtrStop |    90.682 μs |  0.8679 μs |  0.7247 μs |
|         GetAwesome |    74.180 μs |  0.6925 μs |  0.6139 μs |
|            GetBeta |   216.605 μs |  3.0936 μs |  3.4385 μs |
|          GetBetaUp |   222.732 μs |  1.2801 μs |  1.1974 μs |
|        GetBetaDown |   222.977 μs |  0.7873 μs |  0.6574 μs |
|         GetBetaAll |   484.641 μs |  2.5323 μs |  1.9771 μs |
|  GetBollingerBands |   107.270 μs |  0.4767 μs |  0.4226 μs |
|             GetBop |    63.907 μs |  0.4611 μs |  0.4088 μs |
|             GetCci |    76.884 μs |  1.1891 μs |  1.6276 μs |
|      GetChaikinOsc |    86.955 μs |  0.2886 μs |  0.2410 μs |
|      GetChandelier |   108.844 μs |  0.5719 μs |  0.4775 μs |
|            GetChop |   111.667 μs |  1.7298 μs |  1.7764 μs |
|             GetCmf |   124.770 μs |  1.0840 μs |  0.9052 μs |
|             GetCmo |    96.011 μs |  0.5825 μs |  0.5448 μs |
|      GetConnorsRsi |   189.439 μs |  3.7360 μs |  4.4474 μs |
|     GetCorrelation |   166.036 μs |  1.5771 μs |  1.3981 μs |
|            GetDema |    53.270 μs |  0.2741 μs |  0.2430 μs |
|            GetDoji |   106.829 μs |  1.5953 μs |  1.7732 μs |
|        GetDonchian |   292.007 μs |  0.8906 μs |  0.6953 μs |
|             GetDpo |    87.319 μs |  0.6342 μs |  0.5933 μs |
|        GetElderRay |    98.689 μs |  0.5413 μs |  0.4799 μs |
|             GetEma |    50.140 μs |  0.3439 μs |  0.3217 μs |
|       GetEmaStream |     9.586 μs |  0.0401 μs |  0.0335 μs |
|            GetEpma |    91.291 μs |  0.5130 μs |  0.4548 μs |
|             GetFcb |   332.353 μs |  6.4971 μs |  8.4480 μs |
| GetFisherTransform |    87.551 μs |  0.3960 μs |  0.3092 μs |
|      GetForceIndex |    58.975 μs |  0.3212 μs |  0.3005 μs |
|         GetFractal |    92.614 μs |  1.7937 μs |  2.3946 μs |
|           GetGator |   106.031 μs |  0.6935 μs |  0.6487 μs |
|      GetHeikinAshi |   152.642 μs |  0.5308 μs |  0.4433 μs |
|             GetHma |   170.629 μs |  1.2705 μs |  1.1263 μs |
|     GetHtTrendline |   119.626 μs |  0.6042 μs |  0.5651 μs |
|           GetHurst | 1,023.893 μs |  3.2497 μs |  2.7137 μs |
|        GetIchimoku |   834.125 μs | 11.6206 μs | 13.3823 μs |
|            GetKama |    65.207 μs |  0.2537 μs |  0.2119 μs |
|         GetKlinger |    67.653 μs |  0.4325 μs |  0.3834 μs |
|         GetKeltner |   110.223 μs |  2.0373 μs |  3.0493 μs |
|             GetKvo |    69.055 μs |  0.2975 μs |  0.2638 μs |
|            GetMacd |    87.513 μs |  0.6737 μs |  0.6302 μs |
|     GetMaEnvelopes |    84.906 μs |  1.6940 μs |  2.1424 μs |
|            GetMama |   109.138 μs |  0.5741 μs |  0.5090 μs |
|        GetMarubozu |   135.062 μs |  1.0313 μs |  0.9647 μs |
|             GetMfi |    85.421 μs |  0.3662 μs |  0.3058 μs |
|             GetObv |    61.110 μs |  0.2663 μs |  0.2361 μs |
|      GetObvWithSma |    72.593 μs |  0.1961 μs |  0.1638 μs |
|    GetParabolicSar |    65.411 μs |  0.2999 μs |  0.2658 μs |
|     GetPivotPoints |    77.078 μs |  0.2984 μs |  0.2492 μs |
|          GetPivots |   165.328 μs |  3.0450 μs |  3.6248 μs |
|             GetPmo |    72.869 μs |  0.6026 μs |  0.5032 μs |
|             GetPrs |    97.875 μs |  0.6059 μs |  0.5371 μs |
|      GetPrsWithSma |   101.490 μs |  1.2524 μs |  1.3920 μs |
|             GetPvo |    76.323 μs |  0.2626 μs |  0.2050 μs |
|           GetRenko |    92.471 μs |  0.3405 μs |  0.2844 μs |
|        GetRenkoAtr |    96.572 μs |  0.3696 μs |  0.4108 μs |
|             GetRoc |    57.653 μs |  0.1554 μs |  0.1377 μs |
|           GetRocWb |    77.752 μs |  0.2855 μs |  0.2531 μs |
|      GetRocWithSma |    67.572 μs |  1.1408 μs |  1.4833 μs |
|   GetRollingPivots |   343.210 μs |  1.1581 μs |  0.9042 μs |
|             GetRsi |    53.448 μs |  0.3220 μs |  0.3012 μs |
|           GetSlope |    95.077 μs |  1.8625 μs |  2.2873 μs |
|             GetSma |    55.914 μs |  0.2879 μs |  0.2552 μs |
|     GetSmaAnalysis |    76.484 μs |  0.3810 μs |  0.3181 μs |
|             GetSmi |    65.075 μs |  0.6833 μs |  0.6057 μs |
|            GetSmma |    52.511 μs |  0.3075 μs |  0.2876 μs |
|      GetStarcBands |   114.777 μs |  0.3766 μs |  0.3145 μs |
|             GetStc |   129.219 μs |  0.9415 μs |  0.8346 μs |
|          GetStdDev |   102.799 μs |  0.6610 μs |  0.6183 μs |
|   GetStdDevWithSma |   119.736 μs |  1.4357 μs |  1.1989 μs |
|  GetStdDevChannels |   103.186 μs |  0.5439 μs |  0.4542 μs |
|           GetStoch |   105.560 μs |  1.4450 μs |  1.6061 μs |
|       GetStochSMMA |    86.398 μs |  0.8397 μs |  0.7443 μs |
|        GetStochRsi |   114.761 μs |  0.7467 μs |  0.6984 μs |
|      GetSuperTrend |    89.431 μs |  1.2348 μs |  1.1550 μs |
|              GetT3 |    57.429 μs |  0.3706 μs |  0.3467 μs |
|            GetTema |    54.666 μs |  0.2554 μs |  0.2264 μs |
|              GetTr |    58.663 μs |  0.4847 μs |  0.4534 μs |
|            GetTrix |    58.582 μs |  0.3360 μs |  0.2978 μs |
|     GetTrixWithSma |    62.583 μs |  0.6077 μs |  0.5074 μs |
|             GetTsi |    61.501 μs |  0.3184 μs |  0.2486 μs |
|      GetUlcerIndex |   226.753 μs |  0.7907 μs |  0.6603 μs |
|        GetUltimate |    86.705 μs |  0.3515 μs |  0.2935 μs |
|  GetVolatilityStop |   104.329 μs |  1.6074 μs |  1.4249 μs |
|          GetVortex |    70.375 μs |  0.4634 μs |  0.4335 μs |
|            GetVwap |    58.609 μs |  0.3439 μs |  0.3217 μs |
|            GetVwma |    72.225 μs |  1.4308 μs |  2.0520 μs |
|       GetWilliamsR |   100.836 μs |  0.5436 μs |  0.4540 μs |
|             GetWma |    65.481 μs |  0.3126 μs |  0.2771 μs |
|          GetZigZag |   166.377 μs |  0.4545 μs |  0.3549 μs |
