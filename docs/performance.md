---
title: Performance benchmarks
permalink: /performance/
relative_path: performance.md
layout: page
---

# {{ page.title }} for v2.0.1

These are the execution times for the current indicators using two years of historical daily stock quotes (502 periods) with default or typical parameters.

``` bash
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1766 (21H2)
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.100-preview.3.22179.4
```

## indicators

|             Method |         Mean |     Error |    StdDev |       Median |
|------------------- |-------------:|----------:|----------:|-------------:|
|             GetAdl |    60.401 μs | 0.1959 μs | 0.1737 μs |    60.401 μs |
|      GetAdlWithSma |    68.466 μs | 0.5235 μs | 0.4641 μs |    68.438 μs |
|             GetAdx |    80.929 μs | 0.3495 μs | 0.2919 μs |    80.886 μs |
|       GetAlligator |    63.722 μs | 0.2399 μs | 0.2003 μs |    63.697 μs |
|            GetAlma |    59.039 μs | 0.2792 μs | 0.2475 μs |    58.978 μs |
|           GetAroon |   115.987 μs | 0.8159 μs | 0.6813 μs |   115.802 μs |
|             GetAtr |    64.065 μs | 1.1651 μs | 2.0710 μs |    63.254 μs |
|         GetAwesome |    74.882 μs | 0.6478 μs | 0.6059 μs |    74.894 μs |
|            GetBeta |   217.582 μs | 4.1445 μs | 6.6926 μs |   215.369 μs |
|          GetBetaUp |   228.262 μs | 4.0399 μs | 8.8676 μs |   224.475 μs |
|        GetBetaDown |   224.283 μs | 1.7050 μs | 1.4237 μs |   223.839 μs |
|         GetBetaAll |   469.895 μs | 4.2181 μs | 3.9457 μs |   469.415 μs |
|  GetBollingerBands |   104.302 μs | 0.7697 μs | 0.7200 μs |   104.151 μs |
|             GetBop |    64.928 μs | 0.4819 μs | 0.4272 μs |    64.955 μs |
|             GetCci |    76.659 μs | 0.3135 μs | 0.2618 μs |    76.615 μs |
|      GetChaikinOsc |    83.184 μs | 0.4868 μs | 0.4554 μs |    83.065 μs |
|      GetChandelier |   110.127 μs | 2.1114 μs | 2.1682 μs |   108.960 μs |
|            GetChop |   111.753 μs | 0.4332 μs | 0.3617 μs |   111.595 μs |
|             GetCmf |   123.505 μs | 1.5849 μs | 1.3234 μs |   123.127 μs |
|      GetConnorsRsi |   180.640 μs | 2.0833 μs | 1.9487 μs |   180.132 μs |
|     GetCorrelation |   169.848 μs | 1.3102 μs | 1.2255 μs |   169.936 μs |
|            GetDema |    52.860 μs | 0.1701 μs | 0.1508 μs |    52.850 μs |
|            GetDoji |   111.176 μs | 0.7611 μs | 0.6747 μs |   110.931 μs |
|        GetDonchian |   302.248 μs | 3.2866 μs | 2.9135 μs |   301.033 μs |
|             GetDpo |    94.287 μs | 0.4761 μs | 0.4453 μs |    94.156 μs |
|        GetElderRay |   100.479 μs | 0.6941 μs | 0.6492 μs |   100.254 μs |
|             GetEma |    50.153 μs | 0.1380 μs | 0.1153 μs |    50.134 μs |
|       GetEmaStream |     9.969 μs | 0.0451 μs | 0.0400 μs |     9.964 μs |
|            GetEpma |    90.063 μs | 0.5556 μs | 0.5197 μs |    89.963 μs |
|             GetFcb |   353.782 μs | 1.8686 μs | 1.5604 μs |   354.253 μs |
| GetFisherTransform |    85.597 μs | 0.2858 μs | 0.2533 μs |    85.563 μs |
|      GetForceIndex |    61.787 μs | 0.5898 μs | 0.5517 μs |    61.810 μs |
|         GetFractal |    95.368 μs | 1.7979 μs | 1.5938 μs |    94.717 μs |
|           GetGator |   105.632 μs | 0.5642 μs | 0.5278 μs |   105.561 μs |
|      GetHeikinAshi |   160.004 μs | 2.5597 μs | 3.5884 μs |   158.735 μs |
|             GetHma |   223.092 μs | 1.5513 μs | 1.3751 μs |   222.806 μs |
|     GetHtTrendline |   118.754 μs | 0.5026 μs | 0.4702 μs |   118.830 μs |
|           GetHurst | 1,061.926 μs | 5.4270 μs | 4.8109 μs | 1,062.514 μs |
|        GetIchimoku |   852.482 μs | 3.2533 μs | 3.0431 μs |   852.061 μs |
|            GetKama |    65.343 μs | 0.3154 μs | 0.2634 μs |    65.327 μs |
|         GetKlinger |    73.408 μs | 0.3397 μs | 0.2837 μs |    73.441 μs |
|         GetKeltner |   113.475 μs | 0.9271 μs | 0.8219 μs |   113.144 μs |
|             GetKvo |    73.810 μs | 0.3192 μs | 0.2830 μs |    73.819 μs |
|            GetMacd |    75.029 μs | 0.3637 μs | 0.3402 μs |    75.011 μs |
|     GetMaEnvelopes |    92.618 μs | 0.5254 μs | 0.4915 μs |    92.591 μs |
|            GetMama |   109.790 μs | 0.6556 μs | 0.6133 μs |   109.699 μs |
|        GetMarubozu |   134.154 μs | 0.9513 μs | 0.8433 μs |   134.258 μs |
|             GetMfi |    94.140 μs | 0.5471 μs | 0.4850 μs |    94.089 μs |
|             GetObv |    63.282 μs | 0.4854 μs | 0.4303 μs |    63.221 μs |
|      GetObvWithSma |    74.727 μs | 0.4527 μs | 0.4013 μs |    74.714 μs |
|    GetParabolicSar |    67.773 μs | 0.3893 μs | 0.3642 μs |    67.799 μs |
|     GetPivotPoints |    85.761 μs | 0.4942 μs | 0.4623 μs |    85.653 μs |
|          GetPivots |   167.801 μs | 0.8489 μs | 0.7526 μs |   167.589 μs |
|             GetPmo |    67.160 μs | 0.3315 μs | 0.2938 μs |    67.190 μs |
|             GetPrs |   100.005 μs | 0.4441 μs | 0.3937 μs |   100.081 μs |
|      GetPrsWithSma |   104.659 μs | 0.5368 μs | 0.5021 μs |   104.532 μs |
|             GetPvo |    75.112 μs | 0.5299 μs | 0.4956 μs |    75.059 μs |
|           GetRenko |    91.823 μs | 0.5937 μs | 0.5553 μs |    91.622 μs |
|        GetRenkoAtr |    96.930 μs | 0.5960 μs | 0.5575 μs |    96.818 μs |
|             GetRoc |    52.916 μs | 0.3180 μs | 0.2819 μs |    52.831 μs |
|           GetRocWb |    77.336 μs | 0.5365 μs | 0.4756 μs |    77.320 μs |
|      GetRocWithSma |    62.133 μs | 0.4011 μs | 0.3556 μs |    62.091 μs |
|   GetRollingPivots |   353.444 μs | 1.9217 μs | 1.7035 μs |   353.427 μs |
|             GetRsi |    55.279 μs | 0.3249 μs | 0.2880 μs |    55.142 μs |
|           GetSlope |    95.059 μs | 1.6530 μs | 2.7160 μs |    94.369 μs |
|             GetSma |    59.746 μs | 0.2861 μs | 0.2389 μs |    59.698 μs |
|     GetSmaAnalysis |    78.802 μs | 0.4904 μs | 0.4347 μs |    78.869 μs |
|             GetSmi |    69.983 μs | 0.5190 μs | 0.4601 μs |    69.830 μs |
|            GetSmma |    51.918 μs | 0.1073 μs | 0.0952 μs |    51.906 μs |
|      GetStarcBands |   128.279 μs | 1.0295 μs | 0.9630 μs |   128.181 μs |
|             GetStc |   119.185 μs | 0.8572 μs | 0.8018 μs |   119.144 μs |
|          GetStdDev |   103.578 μs | 0.6917 μs | 0.5776 μs |   103.456 μs |
|   GetStdDevWithSma |   113.721 μs | 0.7027 μs | 0.6573 μs |   113.890 μs |
|  GetStdDevChannels |   104.527 μs | 0.5476 μs | 0.4573 μs |   104.396 μs |
|           GetStoch |   106.815 μs | 0.7737 μs | 0.6858 μs |   106.769 μs |
|       GetStochSMMA |    91.534 μs | 0.7453 μs | 0.6971 μs |    91.777 μs |
|        GetStochRsi |   118.031 μs | 0.7115 μs | 0.6656 μs |   117.903 μs |
|      GetSuperTrend |    93.278 μs | 0.8014 μs | 0.7496 μs |    93.452 μs |
|              GetT3 |    57.669 μs | 0.2464 μs | 0.1923 μs |    57.668 μs |
|            GetTema |    53.633 μs | 0.2364 μs | 0.2096 μs |    53.608 μs |
|            GetTrix |    55.807 μs | 0.3254 μs | 0.3044 μs |    55.732 μs |
|     GetTrixWithSma |    66.804 μs | 1.3132 μs | 1.7975 μs |    67.382 μs |
|             GetTsi |    59.860 μs | 0.9879 μs | 1.0571 μs |    59.737 μs |
|      GetUlcerIndex |   197.894 μs | 3.0568 μs | 2.7098 μs |   197.095 μs |
|        GetUltimate |    86.138 μs | 0.7006 μs | 0.6211 μs |    85.929 μs |
|  GetVolatilityStop |   107.804 μs | 0.5877 μs | 0.5209 μs |   107.716 μs |
|          GetVortex |    70.626 μs | 0.3349 μs | 0.2797 μs |    70.576 μs |
|            GetVwap |    70.844 μs | 1.1411 μs | 1.0116 μs |    70.744 μs |
|            GetVwma |    83.530 μs | 1.6607 μs | 1.7054 μs |    83.173 μs |
|       GetWilliamsR |    99.584 μs | 1.0061 μs | 1.9384 μs |    99.050 μs |
|             GetWma |    65.439 μs | 0.5449 μs | 0.4830 μs |    65.296 μs |
|          GetZigZag |   154.676 μs | 0.4790 μs | 0.4480 μs |   154.837 μs |
