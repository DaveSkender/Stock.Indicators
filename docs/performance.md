---
title: Performance benchmarks
description: The Stock Indicators for .NET library is built for speed and production workloads.  Compare our execution times with other options.
permalink: /performance/
relative_path: performance.md
layout: page
---

# {{ page.title }} for v2.4.4

These are the execution times for the current indicators using two years of historical daily stock quotes (502 periods) with default or typical parameters.

``` bash
BenchmarkDotNet=v0.13.2, OS=Windows 10 (10.0.19045.2364)
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.101
```

## indicators

|             Method |         Mean |      Error |     StdDev |       Median |
|------------------- |-------------:|-----------:|-----------:|-------------:|
|             GetAdl |    61.034 μs |  0.7901 μs |  0.8114 μs |    60.741 μs |
|      GetAdlWithSma |    74.323 μs |  0.6478 μs |  0.5743 μs |    74.437 μs |
|             GetAdx |    72.448 μs |  0.2314 μs |  0.1807 μs |    72.418 μs |
|       GetAlligator |    69.322 μs |  0.7023 μs |  0.6570 μs |    69.119 μs |
|            GetAlma |    61.086 μs |  0.5809 μs |  0.4851 μs |    60.931 μs |
|           GetAroon |   120.073 μs |  0.8540 μs |  0.7989 μs |   119.764 μs |
|             GetAtr |    63.346 μs |  1.2311 μs |  2.8039 μs |    62.167 μs |
|         GetAtrStop |    90.054 μs |  1.0980 μs |  1.0270 μs |    89.931 μs |
|         GetAwesome |    73.975 μs |  0.5179 μs |  0.4845 μs |    74.039 μs |
|            GetBeta |   216.402 μs |  1.6494 μs |  1.6199 μs |   216.013 μs |
|          GetBetaUp |   221.845 μs |  1.2733 μs |  1.1288 μs |   221.516 μs |
|        GetBetaDown |   224.817 μs |  1.3181 μs |  1.1684 μs |   224.277 μs |
|         GetBetaAll |   498.772 μs |  9.7831 μs | 14.6429 μs |   490.886 μs |
|  GetBollingerBands |   107.220 μs |  0.7715 μs |  0.7217 μs |   106.813 μs |
|             GetBop |    65.971 μs |  0.2440 μs |  0.2037 μs |    65.922 μs |
|             GetCci |    77.977 μs |  0.1890 μs |  0.1476 μs |    77.931 μs |
|      GetChaikinOsc |    87.375 μs |  0.2826 μs |  0.2206 μs |    87.391 μs |
|      GetChandelier |   107.862 μs |  0.6564 μs |  0.6140 μs |   107.653 μs |
|            GetChop |   110.636 μs |  0.6162 μs |  0.5764 μs |   110.374 μs |
|             GetCmf |   124.006 μs |  0.9568 μs |  0.7470 μs |   123.741 μs |
|             GetCmo |    96.194 μs |  0.6245 μs |  0.5536 μs |    96.033 μs |
|      GetConnorsRsi |   186.377 μs |  1.3101 μs |  1.0940 μs |   185.830 μs |
|     GetCorrelation |   165.632 μs |  1.4079 μs |  1.3170 μs |   164.983 μs |
|            GetDema |    53.567 μs |  0.5077 μs |  0.4749 μs |    53.468 μs |
|            GetDoji |   106.065 μs |  0.7736 μs |  0.7236 μs |   105.769 μs |
|        GetDonchian |   292.215 μs |  1.7897 μs |  1.5866 μs |   291.963 μs |
|             GetDpo |    87.161 μs |  0.7185 μs |  0.7986 μs |    86.843 μs |
|        GetElderRay |    98.801 μs |  0.6901 μs |  0.5388 μs |    98.760 μs |
|             GetEma |    50.192 μs |  0.2515 μs |  0.2229 μs |    50.143 μs |
|       GetEmaStream |     9.810 μs |  0.1710 μs |  0.2662 μs |     9.724 μs |
|            GetEpma |    91.392 μs |  0.3486 μs |  0.2722 μs |    91.436 μs |
|             GetFcb |   322.993 μs |  1.7412 μs |  1.5435 μs |   322.892 μs |
| GetFisherTransform |    88.675 μs |  1.7462 μs |  1.4581 μs |    88.008 μs |
|      GetForceIndex |    59.269 μs |  0.4668 μs |  0.3898 μs |    59.126 μs |
|         GetFractal |    90.967 μs |  0.4352 μs |  0.3634 μs |    90.829 μs |
|           GetGator |   106.830 μs |  1.5103 μs |  1.7393 μs |   106.116 μs |
|      GetHeikinAshi |   154.106 μs |  0.9357 μs |  0.8753 μs |   153.748 μs |
|             GetHma |   171.537 μs |  2.9274 μs |  2.5950 μs |   170.870 μs |
|     GetHtTrendline |   118.962 μs |  0.4176 μs |  0.3702 μs |   118.835 μs |
|           GetHurst | 1,035.296 μs | 11.7940 μs | 10.4551 μs | 1,032.299 μs |
|        GetIchimoku |   826.871 μs |  4.7374 μs |  4.4313 μs |   825.695 μs |
|            GetKama |    65.395 μs |  0.6666 μs |  0.6235 μs |    65.121 μs |
|         GetKlinger |    69.134 μs |  0.3524 μs |  0.3124 μs |    69.074 μs |
|         GetKeltner |   110.580 μs |  1.9276 μs |  1.8031 μs |   110.803 μs |
|             GetKvo |    69.215 μs |  0.2002 μs |  0.1563 μs |    69.273 μs |
|            GetMacd |    86.728 μs |  0.3876 μs |  0.3237 μs |    86.602 μs |
|     GetMaEnvelopes |    83.547 μs |  0.6209 μs |  0.5185 μs |    83.343 μs |
|            GetMama |   108.808 μs |  0.3046 μs |  0.2378 μs |   108.720 μs |
|        GetMarubozu |   134.390 μs |  0.5463 μs |  0.4843 μs |   134.307 μs |
|             GetMfi |    86.493 μs |  0.2985 μs |  0.2493 μs |    86.476 μs |
|             GetObv |    62.167 μs |  1.1776 μs |  1.8679 μs |    61.295 μs |
|      GetObvWithSma |    72.786 μs |  0.4669 μs |  0.3899 μs |    72.647 μs |
|    GetParabolicSar |    63.435 μs |  0.4689 μs |  0.4157 μs |    63.360 μs |
|     GetPivotPoints |    78.148 μs |  0.6573 μs |  0.6149 μs |    77.958 μs |
|          GetPivots |   165.449 μs |  2.3788 μs |  2.1088 μs |   164.519 μs |
|             GetPmo |    73.029 μs |  0.6563 μs |  0.6139 μs |    72.901 μs |
|             GetPrs |    98.981 μs |  0.3062 μs |  0.2557 μs |    98.919 μs |
|      GetPrsWithSma |   102.390 μs |  0.5642 μs |  0.5542 μs |   102.221 μs |
|             GetPvo |    77.056 μs |  0.4944 μs |  0.4625 μs |    76.846 μs |
|           GetRenko |    93.130 μs |  0.3934 μs |  0.3488 μs |    93.033 μs |
|        GetRenkoAtr |    96.702 μs |  0.3767 μs |  0.3339 μs |    96.608 μs |
|             GetRoc |    57.568 μs |  0.3160 μs |  0.2639 μs |    57.456 μs |
|           GetRocWb |    78.296 μs |  0.4901 μs |  0.4584 μs |    78.146 μs |
|      GetRocWithSma |    66.613 μs |  0.3832 μs |  0.3200 μs |    66.475 μs |
|   GetRollingPivots |   344.440 μs |  2.1112 μs |  1.8715 μs |   344.073 μs |
|             GetRsi |    53.838 μs |  0.2880 μs |  0.2553 μs |    53.791 μs |
|           GetSlope |    92.855 μs |  0.2922 μs |  0.2440 μs |    92.853 μs |
|             GetSma |    55.955 μs |  0.3799 μs |  0.3554 μs |    55.792 μs |
|     GetSmaAnalysis |    79.346 μs |  1.4716 μs |  1.5112 μs |    78.445 μs |
|             GetSmi |    64.805 μs |  0.3383 μs |  0.2999 μs |    64.850 μs |
|            GetSmma |    52.386 μs |  0.2278 μs |  0.1902 μs |    52.349 μs |
|      GetStarcBands |   115.326 μs |  0.8463 μs |  0.8311 μs |   115.171 μs |
|             GetStc |   128.248 μs |  0.6291 μs |  0.5577 μs |   128.164 μs |
|          GetStdDev |   102.770 μs |  0.5488 μs |  0.5133 μs |   102.763 μs |
|   GetStdDevWithSma |   119.964 μs |  1.8881 μs |  2.0203 μs |   119.438 μs |
|  GetStdDevChannels |   102.864 μs |  0.6931 μs |  0.6144 μs |   102.844 μs |
|           GetStoch |   106.396 μs |  1.1548 μs |  1.0237 μs |   106.191 μs |
|       GetStochSMMA |    87.751 μs |  1.6977 μs |  2.5926 μs |    86.560 μs |
|        GetStochRsi |   116.324 μs |  0.9433 μs |  0.8823 μs |   116.133 μs |
|      GetSuperTrend |    89.092 μs |  0.9461 μs |  0.8850 μs |    89.238 μs |
|              GetT3 |    58.499 μs |  1.0269 μs |  1.6872 μs |    57.780 μs |
|            GetTema |    55.020 μs |  0.4521 μs |  0.4007 μs |    54.983 μs |
|              GetTr |    58.706 μs |  0.3821 μs |  0.3574 μs |    58.704 μs |
|            GetTrix |    59.182 μs |  0.5293 μs |  0.4951 μs |    59.020 μs |
|     GetTrixWithSma |    63.150 μs |  0.4490 μs |  0.3981 μs |    63.052 μs |
|             GetTsi |    62.175 μs |  0.3600 μs |  0.3191 μs |    62.157 μs |
|      GetUlcerIndex |   228.796 μs |  4.5048 μs |  5.0071 μs |   226.323 μs |
|        GetUltimate |    88.883 μs |  0.6861 μs |  0.6082 μs |    88.719 μs |
|  GetVolatilityStop |   103.314 μs |  0.6626 μs |  0.5874 μs |   103.379 μs |
|          GetVortex |    70.592 μs |  0.3965 μs |  0.3515 μs |    70.623 μs |
|            GetVwap |    58.599 μs |  0.3480 μs |  0.3085 μs |    58.550 μs |
|            GetVwma |    72.141 μs |  0.3449 μs |  0.3227 μs |    72.101 μs |
|       GetWilliamsR |   102.201 μs |  0.7367 μs |  0.6531 μs |   102.212 μs |
|             GetWma |    67.038 μs |  1.2970 μs |  1.7754 μs |    66.329 μs |
|          GetZigZag |   166.959 μs |  1.8257 μs |  1.7078 μs |   166.100 μs |
