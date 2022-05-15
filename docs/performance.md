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

|             Method |        Mean |    Error |   StdDev |      Median |
|------------------- |------------:|---------:|---------:|------------:|
|             GetAdl |    60.55 μs | 1.157 μs | 1.505 μs |    59.86 μs |
|      GetAdlWithSma |    72.88 μs | 0.364 μs | 0.341 μs |    72.81 μs |
|             GetAdx |   231.89 μs | 2.586 μs | 2.419 μs |   231.17 μs |
|       GetAlligator |   169.47 μs | 0.614 μs | 0.512 μs |   169.34 μs |
|            GetAlma |    65.99 μs | 0.214 μs | 0.167 μs |    66.02 μs |
|           GetAroon |   141.47 μs | 0.626 μs | 0.555 μs |   141.28 μs |
|             GetAtr |   151.59 μs | 0.500 μs | 0.418 μs |   151.69 μs |
|         GetAwesome |    68.58 μs | 0.426 μs | 0.398 μs |    68.44 μs |
|            GetBeta |   303.89 μs | 2.240 μs | 1.986 μs |   303.50 μs |
|          GetBetaUp |   325.56 μs | 1.116 μs | 0.932 μs |   325.66 μs |
|        GetBetaDown |   318.71 μs | 0.965 μs | 0.806 μs |   318.93 μs |
|         GetBetaAll |   566.07 μs | 4.453 μs | 4.165 μs |   564.85 μs |
|  GetBollingerBands |   219.29 μs | 1.258 μs | 1.176 μs |   219.17 μs |
|             GetBop |    69.35 μs | 0.275 μs | 0.230 μs |    69.34 μs |
|             GetCci |    75.87 μs | 0.244 μs | 0.204 μs |    75.94 μs |
|      GetChaikinOsc |   109.68 μs | 0.560 μs | 0.496 μs |   109.54 μs |
|      GetChandelier |   262.82 μs | 1.205 μs | 1.006 μs |   262.84 μs |
|            GetChop |   110.33 μs | 1.358 μs | 1.334 μs |   109.92 μs |
|             GetCmf |   134.63 μs | 0.851 μs | 0.754 μs |   134.47 μs |
|      GetConnorsRsi |   220.51 μs | 1.471 μs | 1.376 μs |   220.22 μs |
|     GetCorrelation |   156.54 μs | 0.561 μs | 0.438 μs |   156.42 μs |
|            GetDema |    58.19 μs | 0.332 μs | 0.311 μs |    58.06 μs |
|            GetDoji |   107.66 μs | 0.742 μs | 0.658 μs |   107.43 μs |
|        GetDonchian |   307.88 μs | 1.080 μs | 0.902 μs |   307.52 μs |
|             GetDpo |   148.76 μs | 1.580 μs | 1.319 μs |   148.12 μs |
|        GetElderRay |   117.20 μs | 0.782 μs | 0.694 μs |   116.93 μs |
|             GetEma |    55.14 μs | 0.103 μs | 0.081 μs |    55.16 μs |
|            GetEpma |    90.30 μs | 0.778 μs | 0.689 μs |    89.99 μs |
|             GetFcb |   338.45 μs | 0.969 μs | 0.809 μs |   338.41 μs |
| GetFisherTransform |    81.64 μs | 0.338 μs | 0.282 μs |    81.65 μs |
|      GetForceIndex |    59.14 μs | 0.176 μs | 0.147 μs |    59.08 μs |
|         GetFractal |    93.61 μs | 0.771 μs | 0.683 μs |    93.51 μs |
|           GetGator |   225.34 μs | 0.656 μs | 0.512 μs |   225.14 μs |
|      GetHeikinAshi |   166.60 μs | 1.082 μs | 0.959 μs |   166.17 μs |
|             GetHma |   290.99 μs | 4.927 μs | 7.671 μs |   287.24 μs |
|     GetHtTrendline |   169.31 μs | 0.418 μs | 0.326 μs |   169.33 μs |
|           GetHurst | 1,054.72 μs | 5.358 μs | 4.474 μs | 1,053.72 μs |
|        GetIchimoku |   830.03 μs | 4.357 μs | 3.639 μs |   828.42 μs |
|            GetKama |    74.88 μs | 0.254 μs | 0.199 μs |    74.84 μs |
|         GetKlinger |    71.95 μs | 0.256 μs | 0.227 μs |    71.88 μs |
|         GetKeltner |   381.09 μs | 1.993 μs | 1.767 μs |   380.63 μs |
|            GetMacd |   120.49 μs | 0.817 μs | 0.764 μs |   120.29 μs |
|     GetMaEnvelopes |    86.72 μs | 0.183 μs | 0.153 μs |    86.70 μs |
|            GetMama |   136.63 μs | 0.810 μs | 0.757 μs |   136.38 μs |
|        GetMarubozu |   129.75 μs | 1.351 μs | 1.197 μs |   129.34 μs |
|             GetMfi |   173.05 μs | 0.878 μs | 0.821 μs |   172.73 μs |
|             GetObv |    61.36 μs | 0.308 μs | 0.273 μs |    61.31 μs |
|      GetObvWithSma |    73.56 μs | 0.391 μs | 0.365 μs |    73.49 μs |
|    GetParabolicSar |    85.20 μs | 0.451 μs | 0.400 μs |    84.98 μs |
|          GetPivots |   161.53 μs | 0.778 μs | 0.650 μs |   161.49 μs |
|     GetPivotPoints |    83.65 μs | 0.327 μs | 0.273 μs |    83.68 μs |
|             GetPmo |    61.31 μs | 0.202 μs | 0.157 μs |    61.31 μs |
|             GetPrs |    93.96 μs | 0.557 μs | 0.493 μs |    93.77 μs |
|      GetPrsWithSma |    99.85 μs | 0.427 μs | 0.378 μs |    99.67 μs |
|             GetPvo |   165.56 μs | 0.549 μs | 0.458 μs |   165.47 μs |
|           GetRenko |    88.14 μs | 0.378 μs | 0.316 μs |    88.07 μs |
|        GetRenkoAtr |    97.66 μs | 0.707 μs | 0.661 μs |    97.46 μs |
|             GetRoc |    50.98 μs | 0.231 μs | 0.205 μs |    50.91 μs |
|           GetRocWb |    74.53 μs | 0.261 μs | 0.218 μs |    74.53 μs |
|      GetRocWithSma |    58.95 μs | 0.309 μs | 0.289 μs |    58.90 μs |
|   GetRollingPivots |   357.63 μs | 1.130 μs | 1.057 μs |   357.67 μs |
|             GetRsi |    49.03 μs | 0.245 μs | 0.217 μs |    48.97 μs |
|           GetSlope |    89.43 μs | 0.461 μs | 0.409 μs |    89.33 μs |
|             GetSma |    81.62 μs | 0.348 μs | 0.309 μs |    81.53 μs |
|     GetSmaExtended |   152.90 μs | 2.723 μs | 4.841 μs |   150.83 μs |
|             GetSmi |    98.65 μs | 0.259 μs | 0.216 μs |    98.68 μs |
|            GetSmma |    58.77 μs | 0.301 μs | 0.267 μs |    58.69 μs |
|      GetStarcBands |   323.33 μs | 2.445 μs | 2.041 μs |   322.72 μs |
|             GetStc |   284.15 μs | 3.146 μs | 2.789 μs |   284.35 μs |
|          GetStdDev |   101.68 μs | 0.704 μs | 0.624 μs |   101.63 μs |
|   GetStdDevWithSma |   113.81 μs | 0.363 μs | 0.303 μs |   113.88 μs |
|  GetStdDevChannels |   126.99 μs | 1.612 μs | 1.507 μs |   126.82 μs |
|           GetStoch |   110.04 μs | 0.503 μs | 0.446 μs |   109.93 μs |
|       GetStochSMMA |    89.92 μs | 0.543 μs | 0.508 μs |    89.69 μs |
|        GetStochRsi |   234.81 μs | 1.397 μs | 1.307 μs |   235.36 μs |
|      GetSuperTrend |   250.16 μs | 1.804 μs | 1.688 μs |   249.70 μs |
|            GetTema |    64.34 μs | 0.835 μs | 0.740 μs |    64.24 μs |
|            GetTrix |    54.76 μs | 0.264 μs | 0.234 μs |    54.74 μs |
|     GetTrixWithSma |    59.59 μs | 0.304 μs | 0.270 μs |    59.64 μs |
|             GetTsi |    57.40 μs | 0.316 μs | 0.296 μs |    57.37 μs |
|              GetT3 |    70.10 μs | 0.340 μs | 0.301 μs |    70.02 μs |
|      GetUlcerIndex |   246.06 μs | 1.741 μs | 1.543 μs |   245.62 μs |
|        GetUltimate |    98.98 μs | 2.441 μs | 7.198 μs |    94.30 μs |
|  GetVolatilityStop |   240.56 μs | 1.303 μs | 1.155 μs |   240.11 μs |
|          GetVortex |    70.87 μs | 0.419 μs | 0.371 μs |    70.81 μs |
|            GetVwap |    70.73 μs | 0.337 μs | 0.282 μs |    70.77 μs |
|            GetVwma |    81.84 μs | 0.411 μs | 0.365 μs |    81.79 μs |
|       GetWilliamsR |   122.04 μs | 0.277 μs | 0.216 μs |   122.01 μs |
|             GetWma |    71.78 μs | 0.149 μs | 0.125 μs |    71.75 μs |
|          GetZigZag |   153.60 μs | 0.723 μs | 0.676 μs |   153.21 μs |
