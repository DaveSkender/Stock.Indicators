---
title: Performance benchmarks for v1.20.0
permalink: /performance/
layout: default
redirect_from:
 - /tests/performance
 - /tests/performance/
---

# {{ page.title }}

These are the execution times for the current indicators using two years of historical daily stock quotes (502 periods) with default or typical parameters.

``` bash
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1348 (21H2)
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.100
```

## indicators

|             Method |        Mean |     Error |    StdDev |
|------------------- |------------:|----------:|----------:|
|             GetAdl |   124.29 μs |  2.414 μs |  3.687 μs |
|      GetAdlWithSma |   115.54 μs |  1.839 μs |  2.753 μs |
|             GetAdx |   237.15 μs |  1.069 μs |  0.948 μs |
|       GetAlligator |   155.55 μs |  0.993 μs |  0.880 μs |
|            GetAlma |    83.36 μs |  0.531 μs |  0.444 μs |
|           GetAroon |   309.18 μs |  1.777 μs |  1.576 μs |
|             GetAtr |   156.35 μs |  0.484 μs |  0.404 μs |
|         GetAwesome |    66.29 μs |  0.259 μs |  0.230 μs |
|            GetBeta |   286.29 μs |  0.851 μs |  0.665 μs |
|          GetBetaUp |   377.75 μs |  1.433 μs |  1.270 μs |
|        GetBetaDown |   369.43 μs |  1.257 μs |  1.050 μs |
|         GetBetaAll |   878.41 μs | 16.218 μs | 13.543 μs |
|  GetBollingerBands |   259.73 μs |  1.392 μs |  1.234 μs |
|             GetBop |   108.02 μs |  0.246 μs |  0.205 μs |
|             GetCci |    81.89 μs |  0.852 μs |  0.797 μs |
|      GetChaikinOsc |   162.76 μs |  1.113 μs |  0.987 μs |
|      GetChandelier |   346.43 μs |  1.250 μs |  1.044 μs |
|            GetChop |   119.33 μs |  0.454 μs |  0.402 μs |
|             GetCmf |   212.59 μs |  1.732 μs |  1.446 μs |
|      GetConnorsRsi |   249.64 μs |  1.196 μs |  0.999 μs |
|     GetCorrelation |   229.14 μs |  0.633 μs |  0.494 μs |
|        GetDonchian |   312.26 μs |  2.677 μs |  2.236 μs |
|       GetDoubleEma |   100.06 μs |  0.361 μs |  0.302 μs |
|             GetDpo |   149.42 μs |  0.430 μs |  0.402 μs |
|        GetElderRay |   117.93 μs |  0.895 μs |  0.793 μs |
|             GetEma |    57.01 μs |  0.494 μs |  0.386 μs |
|            GetEpma |   154.44 μs |  0.546 μs |  0.456 μs |
|             GetFcb |   346.71 μs |  3.460 μs |  3.067 μs |
| GetFisherTransform |    80.28 μs |  0.213 μs |  0.178 μs |
|      GetForceIndex |    48.03 μs |  0.178 μs |  0.149 μs |
|         GetFractal |    95.96 μs |  0.528 μs |  0.441 μs |
|           GetGator |   201.69 μs |  0.528 μs |  0.412 μs |
|      GetHeikinAshi |   173.05 μs |  1.270 μs |  1.060 μs |
|             GetHma |   326.51 μs |  2.386 μs |  2.116 μs |
|     GetHtTrendline |   165.08 μs |  0.916 μs |  0.812 μs |
|           GetHurst |   996.05 μs |  2.921 μs |  2.589 μs |
|        GetIchimoku |   872.99 μs |  6.311 μs |  5.903 μs |
|            GetKama |   218.28 μs |  1.158 μs |  1.026 μs |
|         GetKlinger |    75.66 μs |  0.409 μs |  0.342 μs |
|         GetKeltner |   381.80 μs |  1.465 μs |  1.299 μs |
|            GetMacd |   143.19 μs |  0.550 μs |  0.429 μs |
|     GetMaEnvelopes |    86.82 μs |  0.665 μs |  0.622 μs |
|            GetMama |   132.06 μs |  1.773 μs |  1.571 μs |
|        GetMarubozu |   125.23 μs |  2.474 μs |  2.540 μs |
|             GetMfi |   172.48 μs |  1.895 μs |  1.773 μs |
|             GetObv |    60.26 μs |  0.430 μs |  0.381 μs |
|      GetObvWithSma |    65.82 μs |  0.340 μs |  0.302 μs |
|    GetParabolicSar |    94.03 μs |  0.178 μs |  0.158 μs |
|          GetPivots |   159.44 μs |  0.464 μs |  0.387 μs |
|     GetPivotPoints |    93.03 μs |  0.871 μs |  0.772 μs |
|             GetPmo |   113.49 μs |  2.236 μs |  2.196 μs |
|             GetPrs |   133.71 μs |  1.256 μs |  1.175 μs |
|      GetPrsWithSma |   138.15 μs |  0.610 μs |  0.541 μs |
|             GetPvo |   203.95 μs |  0.467 μs |  0.390 μs |
|           GetRenko |    94.77 μs |  0.744 μs |  0.659 μs |
|        GetRenkoAtr |    99.85 μs |  0.271 μs |  0.240 μs |
|             GetRoc |    95.43 μs |  0.331 μs |  0.310 μs |
|           GetRocWb |   115.57 μs |  0.942 μs |  0.835 μs |
|      GetRocWithSma |   106.40 μs |  0.837 μs |  0.742 μs |
|   GetRollingPivots |   326.90 μs |  1.383 μs |  1.226 μs |
|             GetRsi |    53.08 μs |  0.098 μs |  0.087 μs |
|           GetSlope |   175.21 μs |  0.348 μs |  0.309 μs |
|             GetSma |    82.92 μs |  0.180 μs |  0.159 μs |
|     GetSmaExtended |   174.34 μs |  0.276 μs |  0.244 μs |
|             GetSmi |   151.84 μs |  1.414 μs |  1.253 μs |
|            GetSmma |    80.26 μs |  0.302 μs |  0.283 μs |
|      GetStarcBands |   366.64 μs |  0.567 μs |  0.531 μs |
|             GetStc |   395.16 μs |  3.675 μs |  3.257 μs |
|          GetStdDev |   169.55 μs |  1.396 μs |  1.237 μs |
|   GetStdDevWithSma |   178.82 μs |  0.296 μs |  0.247 μs |
|  GetStdDevChannels |   209.56 μs |  0.692 μs |  0.613 μs |
|           GetStoch |   290.82 μs |  0.536 μs |  0.418 μs |
|       GetStochSMMA |   238.85 μs |  2.398 μs |  2.003 μs |
|        GetStochRsi |   345.09 μs |  4.706 μs |  3.929 μs |
|      GetSuperTrend |   247.28 μs |  1.870 μs |  1.658 μs |
|       GetTripleEma |   143.98 μs |  2.074 μs |  1.619 μs |
|            GetTrix |   185.84 μs |  1.499 μs |  1.402 μs |
|     GetTrixWithSma |   243.63 μs |  1.639 μs |  1.533 μs |
|             GetTsi |    61.29 μs |  0.603 μs |  0.564 μs |
|              GetT3 |    66.47 μs |  0.776 μs |  0.726 μs |
|      GetUlcerIndex | 1,102.98 μs |  3.249 μs |  3.039 μs |
|        GetUltimate |   110.62 μs |  0.974 μs |  0.863 μs |
|  GetVolatilityStop |   243.24 μs |  0.560 μs |  0.497 μs |
|          GetVolSma |   159.04 μs |  1.197 μs |  1.120 μs |
|          GetVortex |    90.71 μs |  1.141 μs |  0.953 μs |
|            GetVwap |    67.75 μs |  0.362 μs |  0.338 μs |
|       GetWilliamsR |   254.60 μs |  1.243 μs |  1.038 μs |
|             GetWma |   101.64 μs |  0.579 μs |  0.541 μs |
|          GetZigZag |   163.98 μs |  3.266 μs |  5.458 μs |
