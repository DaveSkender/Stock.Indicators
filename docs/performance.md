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
|             GetAdl |    60.28 μs |  0.702 μs |  0.622 μs |    60.10 μs |
|      GetAdlWithSma |    72.82 μs |  0.311 μs |  0.242 μs |    72.81 μs |
|             GetAdx |   247.28 μs |  1.834 μs |  1.626 μs |   246.59 μs |
|       GetAlligator |   169.17 μs |  0.426 μs |  0.333 μs |   169.05 μs |
|            GetAlma |    65.93 μs |  0.313 μs |  0.278 μs |    65.89 μs |
|           GetAroon |   147.69 μs |  0.528 μs |  0.441 μs |   147.67 μs |
|             GetAtr |   167.37 μs |  1.113 μs |  0.987 μs |   167.16 μs |
|         GetAwesome |    83.96 μs |  0.431 μs |  0.360 μs |    83.84 μs |
|            GetBeta |   303.88 μs |  1.745 μs |  1.457 μs |   303.78 μs |
|          GetBetaUp |   326.17 μs |  1.540 μs |  1.365 μs |   326.05 μs |
|        GetBetaDown |   318.65 μs |  2.421 μs |  2.265 μs |   318.15 μs |
|         GetBetaAll |   563.70 μs |  4.203 μs |  3.510 μs |   564.70 μs |
|  GetBollingerBands |   218.13 μs |  1.218 μs |  1.017 μs |   218.14 μs |
|             GetBop |    69.28 μs |  0.319 μs |  0.283 μs |    69.26 μs |
|             GetCci |    76.04 μs |  0.351 μs |  0.293 μs |    75.91 μs |
|      GetChaikinOsc |   111.09 μs |  0.334 μs |  0.279 μs |   111.07 μs |
|      GetChandelier |   284.28 μs |  0.893 μs |  0.746 μs |   284.20 μs |
|            GetChop |   110.05 μs |  0.893 μs |  0.791 μs |   109.91 μs |
|             GetCmf |   132.36 μs |  0.636 μs |  0.497 μs |   132.25 μs |
|      GetConnorsRsi |   219.59 μs |  0.497 μs |  0.388 μs |   219.61 μs |
|     GetCorrelation |   156.57 μs |  1.148 μs |  1.017 μs |   156.47 μs |
|            GetDema |    70.80 μs |  1.561 μs |  4.579 μs |    68.95 μs |
|            GetDoji |   144.17 μs |  2.880 μs |  5.549 μs |   144.54 μs |
|        GetDonchian |   359.05 μs |  7.104 μs | 13.856 μs |   355.15 μs |
|             GetDpo |   186.61 μs |  3.611 μs |  8.993 μs |   186.55 μs |
|        GetElderRay |   142.17 μs |  2.681 μs |  4.094 μs |   141.15 μs |
|             GetEma |    68.97 μs |  1.371 μs |  2.094 μs |    68.83 μs |
|            GetEpma |   106.03 μs |  2.040 μs |  2.003 μs |   106.09 μs |
|             GetFcb |   467.54 μs |  8.002 μs |  7.094 μs |   465.79 μs |
| GetFisherTransform |    95.98 μs |  1.868 μs |  2.556 μs |    96.13 μs |
|      GetForceIndex |    70.94 μs |  1.410 μs |  2.470 μs |    70.93 μs |
|         GetFractal |   122.78 μs |  2.355 μs |  2.803 μs |   122.60 μs |
|           GetGator |   260.60 μs |  4.982 μs |  6.984 μs |   260.34 μs |
|      GetHeikinAshi |   179.24 μs |  3.431 μs |  3.523 μs |   179.07 μs |
|             GetHma |   331.68 μs |  5.720 μs |  5.351 μs |   331.96 μs |
|     GetHtTrendline |   197.13 μs |  3.900 μs |  4.789 μs |   196.61 μs |
|           GetHurst | 1,218.72 μs | 23.769 μs | 30.907 μs | 1,215.29 μs |
|        GetIchimoku | 1,052.01 μs | 20.682 μs | 32.200 μs | 1,048.64 μs |
|            GetKama |    88.10 μs |  1.707 μs |  1.596 μs |    88.36 μs |
|         GetKlinger |    85.54 μs |  1.683 μs |  1.938 μs |    85.43 μs |
|         GetKeltner |   457.88 μs |  9.020 μs | 12.346 μs |   454.59 μs |
|            GetMacd |   147.78 μs |  2.905 μs |  5.868 μs |   146.80 μs |
|     GetMaEnvelopes |   108.06 μs |  2.122 μs |  3.660 μs |   108.03 μs |
|            GetMama |   161.68 μs |  3.205 μs |  6.174 μs |   159.92 μs |
|        GetMarubozu |   157.35 μs |  3.109 μs |  7.626 μs |   155.91 μs |
|             GetMfi |   222.76 μs |  3.478 μs |  3.083 μs |   223.00 μs |
|             GetObv |    74.07 μs |  1.377 μs |  2.653 μs |    74.23 μs |
|      GetObvWithSma |    87.92 μs |  1.651 μs |  2.666 μs |    88.02 μs |
|    GetParabolicSar |   101.78 μs |  2.033 μs |  3.452 μs |   101.76 μs |
|          GetPivots |   200.48 μs |  3.451 μs |  3.059 μs |   201.24 μs |
|     GetPivotPoints |   105.77 μs |  2.097 μs |  4.090 μs |   104.95 μs |
|             GetPmo |    76.33 μs |  1.520 μs |  3.001 μs |    76.38 μs |
|             GetPrs |   114.81 μs |  2.278 μs |  5.799 μs |   114.38 μs |
|      GetPrsWithSma |   126.17 μs |  2.587 μs |  7.586 μs |   125.78 μs |
|             GetPvo |   199.70 μs |  4.435 μs | 13.077 μs |   195.30 μs |
|           GetRenko |    97.80 μs |  1.706 μs |  1.425 μs |    97.52 μs |
|        GetRenkoAtr |   106.07 μs |  2.028 μs |  1.897 μs |   106.10 μs |
|             GetRoc |    55.92 μs |  0.959 μs |  1.375 μs |    55.32 μs |
|           GetRocWb |    83.02 μs |  1.299 μs |  1.152 μs |    82.84 μs |
|      GetRocWithSma |    64.19 μs |  1.203 μs |  1.066 μs |    64.02 μs |
|   GetRollingPivots |   474.40 μs |  9.213 μs | 12.610 μs |   476.64 μs |
|             GetRsi |    56.84 μs |  1.135 μs |  1.554 μs |    57.15 μs |
|           GetSlope |   100.92 μs |  1.961 μs |  2.480 μs |   101.45 μs |
|             GetSma |    99.06 μs |  1.946 μs |  2.971 μs |    99.16 μs |
|     GetSmaExtended |   279.44 μs |  5.361 μs | 12.210 μs |   276.83 μs |
|             GetSmi |   108.79 μs |  0.598 μs |  0.500 μs |   108.59 μs |
|            GetSmma |    88.92 μs |  1.086 μs |  0.963 μs |    88.61 μs |
|      GetStarcBands |   339.35 μs |  1.438 μs |  1.346 μs |   339.06 μs |
|             GetStc |   329.79 μs |  1.411 μs |  1.179 μs |   329.66 μs |
|          GetStdDev |   101.16 μs |  0.600 μs |  0.562 μs |   100.97 μs |
|   GetStdDevWithSma |   114.46 μs |  0.389 μs |  0.345 μs |   114.43 μs |
|  GetStdDevChannels |   125.58 μs |  0.575 μs |  0.510 μs |   125.75 μs |
|           GetStoch |   180.72 μs |  0.599 μs |  0.531 μs |   180.79 μs |
|       GetStochSMMA |   160.76 μs |  0.579 μs |  0.484 μs |   160.78 μs |
|        GetStochRsi |   289.85 μs |  1.900 μs |  1.777 μs |   289.40 μs |
|      GetSuperTrend |   266.10 μs |  1.468 μs |  1.226 μs |   266.04 μs |
|            GetTema |    63.76 μs |  0.258 μs |  0.229 μs |    63.73 μs |
|            GetTrix |    54.61 μs |  0.200 μs |  0.177 μs |    54.59 μs |
|     GetTrixWithSma |    59.87 μs |  0.529 μs |  0.413 μs |    59.77 μs |
|             GetTsi |    68.32 μs |  1.449 μs |  4.249 μs |    69.15 μs |
|              GetT3 |    79.22 μs |  1.576 μs |  4.446 μs |    78.17 μs |
|      GetUlcerIndex |   260.25 μs |  4.307 μs |  3.818 μs |   259.01 μs |
|        GetUltimate |   107.59 μs |  2.111 μs |  4.407 μs |   107.62 μs |
|  GetVolatilityStop |   257.05 μs |  1.133 μs |  0.946 μs |   256.96 μs |
|          GetVortex |    71.22 μs |  0.396 μs |  0.351 μs |    71.12 μs |
|            GetVwap |    71.52 μs |  0.543 μs |  0.481 μs |    71.28 μs |
|            GetVwma |    82.36 μs |  0.348 μs |  0.309 μs |    82.37 μs |
|       GetWilliamsR |   145.14 μs |  0.854 μs |  0.757 μs |   144.91 μs |
|             GetWma |    72.77 μs |  0.528 μs |  0.468 μs |    72.75 μs |
|          GetZigZag |   155.56 μs |  0.714 μs |  0.597 μs |   155.69 μs |
