---
title: Performance benchmarks
description: The Stock Indicators for .NET library is built for speed and production workloads.  Compare our execution times with other options.
permalink: /performance/
relative_path: performance.md
layout: page
---

# {{ page.title }} for v2.5.0

These are the execution times for the current indicators using two years of historical daily stock quotes (502 periods) with default or typical parameters.

``` bash
BenchmarkDotNet=v0.13.2, OS=Windows 10 (10.0.19045.2364)
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.101
```

## indicators

|             Method |        Mean |    Error |   StdDev |      Median |
|------------------- |------------:|---------:|---------:|------------:|
|             GetAdl |    71.31 μs | 0.340 μs | 0.284 μs |    71.32 μs |
|      GetAdlWithSma |    92.81 μs | 0.746 μs | 0.698 μs |    92.69 μs |
|             GetAdx |    85.96 μs | 0.396 μs | 0.331 μs |    86.08 μs |
|       GetAlligator |    84.08 μs | 0.455 μs | 0.380 μs |    84.15 μs |
|            GetAlma |    75.46 μs | 0.633 μs | 0.561 μs |    75.32 μs |
|           GetAroon |   157.81 μs | 1.204 μs | 1.067 μs |   157.79 μs |
|             GetAtr |    78.59 μs | 1.989 μs | 5.802 μs |    75.47 μs |
|         GetAtrStop |   111.57 μs | 0.603 μs | 0.564 μs |   111.59 μs |
|         GetAwesome |    79.86 μs | 0.708 μs | 0.627 μs |    79.54 μs |
|            GetBeta |   234.33 μs | 2.006 μs | 1.566 μs |   233.97 μs |
|          GetBetaUp |   234.20 μs | 1.203 μs | 1.126 μs |   234.33 μs |
|        GetBetaDown |   236.89 μs | 1.867 μs | 1.655 μs |   236.62 μs |
|         GetBetaAll |   496.71 μs | 3.277 μs | 2.905 μs |   496.23 μs |
|  GetBollingerBands |   138.28 μs | 1.167 μs | 1.091 μs |   137.89 μs |
|             GetBop |    80.39 μs | 0.937 μs | 0.782 μs |    80.55 μs |
|             GetCci |    88.74 μs | 0.618 μs | 0.516 μs |    88.66 μs |
|      GetChaikinOsc |   119.45 μs | 0.618 μs | 0.578 μs |   119.43 μs |
|      GetChandelier |   139.75 μs | 1.187 μs | 1.052 μs |   139.61 μs |
|            GetChop |   126.76 μs | 0.893 μs | 0.792 μs |   126.77 μs |
|             GetCmf |   190.71 μs | 1.695 μs | 1.585 μs |   190.28 μs |
|             GetCmo |    97.05 μs | 0.679 μs | 0.635 μs |    96.85 μs |
|      GetConnorsRsi |   193.13 μs | 0.950 μs | 0.793 μs |   193.18 μs |
|     GetCorrelation |   223.44 μs | 1.263 μs | 1.181 μs |   223.19 μs |
|            GetDema |    60.45 μs | 0.514 μs | 0.481 μs |    60.39 μs |
|            GetDoji |   115.66 μs | 0.344 μs | 0.321 μs |   115.61 μs |
|        GetDonchian |   337.16 μs | 6.321 μs | 5.913 μs |   334.10 μs |
|             GetDpo |   117.38 μs | 0.692 μs | 0.614 μs |   117.30 μs |
|        GetElderRay |   117.26 μs | 0.605 μs | 0.537 μs |   117.16 μs |
|             GetEma |    56.36 μs | 0.329 μs | 0.308 μs |    56.37 μs |
|       GetEmaStream |    11.42 μs | 0.080 μs | 0.071 μs |    11.41 μs |
|            GetEpma |   125.02 μs | 0.894 μs | 0.793 μs |   125.06 μs |
|             GetFcb |   377.92 μs | 1.868 μs | 1.656 μs |   377.66 μs |
| GetFisherTransform |   104.60 μs | 0.457 μs | 0.405 μs |   104.67 μs |
|      GetForceIndex |    70.02 μs | 1.386 μs | 2.390 μs |    68.87 μs |
|         GetFractal |   113.98 μs | 0.661 μs | 0.516 μs |   113.89 μs |
|           GetGator |   126.02 μs | 0.579 μs | 0.514 μs |   125.93 μs |
|      GetHeikinAshi |   167.10 μs | 2.546 μs | 2.126 μs |   166.23 μs |
|             GetHma |   193.00 μs | 1.341 μs | 1.189 μs |   192.68 μs |
|     GetHtTrendline |   128.03 μs | 1.073 μs | 0.951 μs |   127.61 μs |
|           GetHurst | 1,129.99 μs | 3.490 μs | 3.094 μs | 1,129.89 μs |
|        GetIchimoku |   978.81 μs | 3.982 μs | 3.724 μs |   978.47 μs |
|            GetKama |    87.92 μs | 0.422 μs | 0.374 μs |    87.76 μs |
|         GetKlinger |    80.20 μs | 0.421 μs | 0.373 μs |    80.15 μs |
|         GetKeltner |   136.52 μs | 1.082 μs | 0.959 μs |   136.28 μs |
|             GetKvo |    80.14 μs | 0.596 μs | 0.498 μs |    80.21 μs |
|            GetMacd |   112.67 μs | 0.580 μs | 0.543 μs |   112.54 μs |
|     GetMaEnvelopes |   110.52 μs | 0.440 μs | 0.411 μs |   110.48 μs |
|            GetMama |   118.71 μs | 1.098 μs | 0.857 μs |   118.47 μs |
|        GetMarubozu |   142.40 μs | 1.167 μs | 1.091 μs |   142.00 μs |
|             GetMfi |   100.38 μs | 0.297 μs | 0.232 μs |   100.34 μs |
|             GetObv |    71.37 μs | 0.630 μs | 0.590 μs |    71.18 μs |
|      GetObvWithSma |    90.54 μs | 0.588 μs | 0.550 μs |    90.31 μs |
|    GetParabolicSar |    81.71 μs | 0.566 μs | 0.529 μs |    81.50 μs |
|     GetPivotPoints |    91.31 μs | 0.467 μs | 0.437 μs |    91.30 μs |
|          GetPivots |   197.74 μs | 3.951 μs | 4.550 μs |   194.83 μs |
|             GetPmo |    83.37 μs | 0.391 μs | 0.327 μs |    83.28 μs |
|             GetPrs |   108.45 μs | 0.595 μs | 0.556 μs |   108.29 μs |
|      GetPrsWithSma |   120.35 μs | 2.374 μs | 2.221 μs |   119.11 μs |
|             GetPvo |    99.88 μs | 0.363 μs | 0.340 μs |    99.90 μs |
|           GetRenko |   104.46 μs | 0.411 μs | 0.385 μs |   104.36 μs |
|        GetRenkoAtr |   106.94 μs | 0.577 μs | 0.450 μs |   106.78 μs |
|             GetRoc |    67.28 μs | 0.308 μs | 0.257 μs |    67.37 μs |
|           GetRocWb |    97.02 μs | 0.430 μs | 0.381 μs |    96.90 μs |
|      GetRocWithSma |    86.22 μs | 0.927 μs | 1.808 μs |    85.70 μs |
|   GetRollingPivots |   377.42 μs | 1.956 μs | 1.734 μs |   377.41 μs |
|             GetRsi |    62.53 μs | 1.245 μs | 1.745 μs |    61.69 μs |
|           GetSlope |   131.88 μs | 0.788 μs | 0.698 μs |   131.84 μs |
|             GetSma |    70.49 μs | 0.242 μs | 0.202 μs |    70.44 μs |
|     GetSmaAnalysis |   116.45 μs | 0.373 μs | 0.312 μs |   116.49 μs |
|             GetSmi |    85.63 μs | 1.640 μs | 1.952 μs |    84.77 μs |
|            GetSmma |    58.93 μs | 0.579 μs | 0.753 μs |    58.74 μs |
|      GetStarcBands |   159.57 μs | 1.360 μs | 1.272 μs |   159.27 μs |
|             GetStc |   181.72 μs | 3.611 μs | 4.435 μs |   180.35 μs |
|          GetStdDev |   131.51 μs | 1.409 μs | 1.176 μs |   130.97 μs |
|   GetStdDevWithSma |   150.15 μs | 0.465 μs | 0.388 μs |   150.11 μs |
|  GetStdDevChannels |   155.41 μs | 0.575 μs | 0.480 μs |   155.47 μs |
|           GetStoch |   139.41 μs | 2.604 μs | 2.674 μs |   138.49 μs |
|       GetStochSMMA |   116.20 μs | 0.612 μs | 0.542 μs |   116.05 μs |
|        GetStochRsi |   156.69 μs | 0.458 μs | 0.406 μs |   156.75 μs |
|      GetSuperTrend |   112.96 μs | 2.186 μs | 2.045 μs |   112.01 μs |
|              GetT3 |    66.26 μs | 0.363 μs | 0.321 μs |    66.25 μs |
|            GetTema |    69.20 μs | 1.344 μs | 1.700 μs |    69.80 μs |
|              GetTr |    76.78 μs | 1.504 μs | 2.788 μs |    77.01 μs |
|            GetTrix |    75.37 μs | 1.447 μs | 1.486 μs |    75.02 μs |
|     GetTrixWithSma |    70.90 μs | 0.365 μs | 0.406 μs |    70.71 μs |
|             GetTsi |    71.61 μs | 1.370 μs | 1.876 μs |    70.80 μs |
|      GetUlcerIndex |   373.75 μs | 1.966 μs | 1.839 μs |   373.42 μs |
|        GetUltimate |    99.74 μs | 0.438 μs | 0.410 μs |    99.61 μs |
|  GetVolatilityStop |   130.68 μs | 0.847 μs | 0.661 μs |   130.71 μs |
|          GetVortex |    80.20 μs | 0.232 μs | 0.194 μs |    80.15 μs |
|            GetVwap |    69.36 μs | 0.399 μs | 0.354 μs |    69.31 μs |
|            GetVwma |    99.52 μs | 0.415 μs | 0.368 μs |    99.52 μs |
|       GetWilliamsR |   139.32 μs | 1.410 μs | 1.101 μs |   139.13 μs |
|             GetWma |    82.92 μs | 0.353 μs | 0.295 μs |    82.90 μs |
|          GetZigZag |   188.82 μs | 1.485 μs | 1.389 μs |   188.54 μs |
