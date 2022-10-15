---
title: Performance benchmarks
description: The Stock Indicators for .NET library is built for speed and production workloads.  Compare our execution times with other options.
permalink: /performance/
relative_path: performance.md
layout: page
noindex: true
sitemap: false
---

# {{ page.title }} for v2.3.1

These are the execution times for the current indicators using two years of historical daily stock quotes (502 periods) with default or typical parameters.

``` bash
BenchmarkDotNet=v0.13.2, OS=Windows 10 (10.0.19044.2006/21H2/November2021Update)
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.100-preview.5.22307.18
```

## indicators

|             Method |        Mean |     Error |    StdDev |      Median |
|------------------- |------------:|----------:|----------:|------------:|
|             GetAdl |    63.94 μs |  1.260 μs |  1.724 μs |    63.77 μs |
|      GetAdlWithSma |    71.49 μs |  1.412 μs |  1.511 μs |    71.59 μs |
|             GetAdx |    74.40 μs |  1.474 μs |  1.447 μs |    74.18 μs |
|       GetAlligator |    74.27 μs |  1.384 μs |  1.295 μs |    74.90 μs |
|            GetAlma |    59.78 μs |  1.189 μs |  1.168 μs |    60.22 μs |
|           GetAroon |   117.09 μs |  2.320 μs |  2.279 μs |   117.87 μs |
|             GetAtr |    65.60 μs |  0.676 μs |  0.632 μs |    65.81 μs |
|         GetAtrStop |    95.16 μs |  1.902 μs |  2.264 μs |    95.79 μs |
|         GetAwesome |    75.30 μs |  1.272 μs |  1.465 μs |    75.56 μs |
|            GetBeta |   260.85 μs |  9.754 μs | 28.452 μs |   259.90 μs |
|          GetBetaUp |   306.25 μs |  5.979 μs |  7.774 μs |   305.27 μs |
|        GetBetaDown |   284.79 μs | 13.465 μs | 39.702 μs |   299.98 μs |
|         GetBetaAll |   534.27 μs |  9.217 μs |  7.196 μs |   531.84 μs |
|  GetBollingerBands |   111.81 μs |  1.571 μs |  1.470 μs |   112.42 μs |
|             GetBop |    67.86 μs |  1.306 μs |  1.283 μs |    68.23 μs |
|             GetCci |    81.58 μs |  1.079 μs |  0.956 μs |    81.88 μs |
|      GetChaikinOsc |    91.11 μs |  1.690 μs |  2.369 μs |    91.13 μs |
|      GetChandelier |   111.89 μs |  2.150 μs |  2.011 μs |   112.83 μs |
|            GetChop |   113.16 μs |  2.024 μs |  1.893 μs |   114.20 μs |
|             GetCmf |   128.85 μs |  2.362 μs |  2.094 μs |   129.15 μs |
|             GetCmo |    99.23 μs |  1.098 μs |  1.027 μs |    99.59 μs |
|      GetConnorsRsi |   177.56 μs |  0.630 μs |  0.492 μs |   177.65 μs |
|     GetCorrelation |   174.42 μs |  3.056 μs |  3.002 μs |   174.71 μs |
|            GetDema |    68.69 μs |  1.314 μs |  1.291 μs |    68.67 μs |
|            GetDoji |   140.52 μs |  2.760 μs |  4.131 μs |   140.43 μs |
|        GetDonchian |   329.84 μs |  5.609 μs |  4.684 μs |   329.83 μs |
|             GetDpo |   120.60 μs |  2.243 μs |  2.098 μs |   120.47 μs |
|        GetElderRay |   127.71 μs |  2.519 μs |  2.800 μs |   128.25 μs |
|             GetEma |    54.27 μs |  1.217 μs |  3.290 μs |    53.19 μs |
|       GetEmaStream |    10.11 μs |  0.121 μs |  0.113 μs |    10.14 μs |
|            GetEpma |    97.92 μs |  1.710 μs |  1.600 μs |    98.33 μs |
|             GetFcb |   339.94 μs |  5.965 μs |  5.580 μs |   336.95 μs |
| GetFisherTransform |    89.63 μs |  1.538 μs |  1.363 μs |    90.19 μs |
|      GetForceIndex |    61.32 μs |  1.161 μs |  1.030 μs |    61.83 μs |
|         GetFractal |    94.70 μs |  1.738 μs |  1.540 μs |    95.29 μs |
|           GetGator |   112.04 μs |  1.717 μs |  1.606 μs |   112.76 μs |
|      GetHeikinAshi |   155.26 μs |  2.518 μs |  2.355 μs |   156.53 μs |
|             GetHma |   182.47 μs |  3.257 μs |  2.720 μs |   182.50 μs |
|     GetHtTrendline |   120.58 μs |  2.242 μs |  2.097 μs |   121.96 μs |
|           GetHurst | 1,059.49 μs | 15.787 μs | 14.767 μs | 1,055.62 μs |
|        GetIchimoku |   937.68 μs | 19.803 μs | 33.627 μs |   935.65 μs |
|            GetKama |    82.95 μs |  1.354 μs |  1.131 μs |    83.08 μs |
|         GetKlinger |    91.46 μs |  1.799 μs |  1.767 μs |    91.38 μs |
|         GetKeltner |   146.83 μs |  2.807 μs |  5.409 μs |   146.07 μs |
|             GetKvo |    92.53 μs |  1.624 μs |  2.223 μs |    92.22 μs |
|            GetMacd |   109.94 μs |  2.170 μs |  3.379 μs |   109.82 μs |
|     GetMaEnvelopes |   104.93 μs |  1.955 μs |  1.920 μs |   105.23 μs |
|            GetMama |   134.48 μs |  2.676 μs |  3.923 μs |   134.23 μs |
|        GetMarubozu |   160.86 μs |  2.926 μs |  2.444 μs |   161.21 μs |
|             GetMfi |   114.60 μs |  2.238 μs |  1.869 μs |   114.43 μs |
|             GetObv |    69.17 μs |  1.349 μs |  3.483 μs |    68.21 μs |
|      GetObvWithSma |    82.99 μs |  1.160 μs |  1.028 μs |    83.48 μs |
|    GetParabolicSar |    72.03 μs |  0.897 μs |  0.795 μs |    72.13 μs |
|     GetPivotPoints |    95.77 μs |  1.352 μs |  1.199 μs |    95.91 μs |
|          GetPivots |   181.77 μs |  3.623 μs |  5.851 μs |   181.14 μs |
|             GetPmo |    73.39 μs |  1.046 μs |  0.927 μs |    73.07 μs |
|             GetPrs |   111.88 μs |  1.424 μs |  1.332 μs |   111.94 μs |
|      GetPrsWithSma |   118.47 μs |  2.349 μs |  2.307 μs |   118.61 μs |
|             GetPvo |    85.70 μs |  1.318 μs |  1.233 μs |    86.29 μs |
|           GetRenko |   103.79 μs |  1.545 μs |  1.445 μs |   103.83 μs |
|        GetRenkoAtr |   108.03 μs |  1.955 μs |  1.633 μs |   107.32 μs |
|             GetRoc |    59.57 μs |  0.745 μs |  0.697 μs |    59.73 μs |
|           GetRocWb |    87.55 μs |  1.632 μs |  1.527 μs |    87.79 μs |
|      GetRocWithSma |    69.53 μs |  1.032 μs |  0.965 μs |    69.66 μs |
|   GetRollingPivots |   385.49 μs |  7.325 μs |  7.194 μs |   387.24 μs |
|             GetRsi |    60.07 μs |  1.120 μs |  0.993 μs |    59.89 μs |
|           GetSlope |   107.77 μs |  1.539 μs |  1.364 μs |   107.38 μs |
|             GetSma |    62.62 μs |  1.011 μs |  0.896 μs |    62.84 μs |
|     GetSmaAnalysis |    88.05 μs |  1.304 μs |  1.220 μs |    87.88 μs |
|             GetSmi |    72.37 μs |  0.922 μs |  0.817 μs |    72.10 μs |
|            GetSmma |    59.93 μs |  0.710 μs |  0.664 μs |    59.59 μs |
|      GetStarcBands |   138.87 μs |  3.217 μs |  9.486 μs |   136.78 μs |
|             GetStc |   144.57 μs |  2.138 μs |  2.196 μs |   145.18 μs |
|          GetStdDev |   115.51 μs |  1.281 μs |  1.069 μs |   115.11 μs |
|   GetStdDevWithSma |   131.93 μs |  2.030 μs |  1.899 μs |   131.65 μs |
|  GetStdDevChannels |   114.24 μs |  2.239 μs |  3.211 μs |   114.15 μs |
|           GetStoch |   111.87 μs |  1.955 μs |  1.733 μs |   111.84 μs |
|       GetStochSMMA |    91.32 μs |  1.429 μs |  1.337 μs |    90.51 μs |
|        GetStochRsi |   124.41 μs |  2.094 μs |  2.866 μs |   124.70 μs |
|      GetSuperTrend |    97.89 μs |  1.941 μs |  1.721 μs |    97.34 μs |
|              GetT3 |    62.27 μs |  1.159 μs |  1.084 μs |    62.59 μs |
|            GetTema |    57.72 μs |  0.976 μs |  0.913 μs |    57.23 μs |
|              GetTr |    64.57 μs |  0.991 μs |  0.927 μs |    65.17 μs |
|            GetTrix |    62.11 μs |  0.860 μs |  0.804 μs |    62.07 μs |
|     GetTrixWithSma |    67.12 μs |  0.953 μs |  0.845 μs |    66.59 μs |
|             GetTsi |    66.02 μs |  1.219 μs |  1.542 μs |    66.18 μs |
|      GetUlcerIndex |   231.95 μs |  3.831 μs |  3.583 μs |   234.01 μs |
|        GetUltimate |    95.34 μs |  1.422 μs |  1.330 μs |    94.58 μs |
|  GetVolatilityStop |   112.81 μs |  1.305 μs |  1.450 μs |   112.24 μs |
|          GetVortex |    74.96 μs |  1.127 μs |  1.054 μs |    74.69 μs |
|            GetVwap |    63.66 μs |  0.943 μs |  0.882 μs |    64.16 μs |
|            GetVwma |    83.63 μs |  1.664 μs |  2.333 μs |    83.65 μs |
|       GetWilliamsR |   109.32 μs |  1.808 μs |  1.691 μs |   109.78 μs |
|             GetWma |    71.69 μs |  1.218 μs |  1.140 μs |    72.11 μs |
|          GetZigZag |   167.22 μs |  2.449 μs |  2.290 μs |   168.41 μs |
