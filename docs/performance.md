---
title: Performance benchmarks
permalink: /performance/
relative_path: performance.md
layout: page
---

# {{ page.title }} for v2.0.0

These are the execution times for the current indicators using two years of historical daily stock quotes (502 periods) with default or typical parameters.

``` bash
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1706 (21H2)
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.100-preview.3.22179.4
```

## indicators

|             Method |         Mean |     Error |    StdDev |       Median |
|------------------- |-------------:|----------:|----------:|-------------:|
|             GetAdl |    60.347 μs | 0.3749 μs | 0.3324 μs |    60.295 μs |
|      GetAdlWithSma |    68.658 μs | 0.4739 μs | 0.3957 μs |    68.622 μs |
|             GetAdx |    80.970 μs | 0.3550 μs | 0.3147 μs |    80.919 μs |
|       GetAlligator |    63.904 μs | 0.3363 μs | 0.2981 μs |    63.846 μs |
|            GetAlma |    60.981 μs | 1.1402 μs | 2.9636 μs |    59.301 μs |
|           GetAroon |   115.751 μs | 0.2999 μs | 0.2341 μs |   115.730 μs |
|             GetAtr |    63.478 μs | 0.6206 μs | 0.4845 μs |    63.303 μs |
|         GetAwesome |    73.718 μs | 0.7548 μs | 0.6303 μs |    73.490 μs |
|            GetBeta |   208.923 μs | 1.2592 μs | 1.1162 μs |   208.800 μs |
|          GetBetaUp |   231.978 μs | 1.7312 μs | 1.5347 μs |   232.002 μs |
|        GetBetaDown |   226.277 μs | 0.4878 μs | 0.3809 μs |   226.344 μs |
|         GetBetaAll |   483.450 μs | 2.1287 μs | 1.7776 μs |   483.658 μs |
|  GetBollingerBands |   106.171 μs | 0.5238 μs | 0.4900 μs |   106.160 μs |
|             GetBop |    66.647 μs | 0.2390 μs | 0.1866 μs |    66.668 μs |
|             GetCci |    78.612 μs | 0.3705 μs | 0.3465 μs |    78.579 μs |
|      GetChaikinOsc |    85.282 μs | 0.5272 μs | 0.4402 μs |    85.233 μs |
|      GetChandelier |   112.554 μs | 0.4248 μs | 0.3974 μs |   112.621 μs |
|            GetChop |   115.419 μs | 0.3906 μs | 0.3653 μs |   115.305 μs |
|             GetCmf |   126.591 μs | 0.8285 μs | 0.7749 μs |   126.516 μs |
|      GetConnorsRsi |   178.875 μs | 0.8408 μs | 0.7865 μs |   178.516 μs |
|     GetCorrelation |   166.876 μs | 0.7236 μs | 0.6415 μs |   166.904 μs |
|            GetDema |    52.756 μs | 0.2324 μs | 0.2060 μs |    52.715 μs |
|            GetDoji |   110.817 μs | 1.1305 μs | 1.0022 μs |   110.642 μs |
|        GetDonchian |   300.250 μs | 1.8153 μs | 1.6981 μs |   299.926 μs |
|             GetDpo |    94.380 μs | 0.5367 μs | 0.5021 μs |    94.266 μs |
|        GetElderRay |    98.987 μs | 0.4124 μs | 0.3656 μs |    99.004 μs |
|             GetEma |    50.276 μs | 0.1659 μs | 0.1470 μs |    50.279 μs |
|       GetEmaStream |     9.967 μs | 0.0489 μs | 0.0433 μs |     9.963 μs |
|            GetEpma |    89.430 μs | 0.5174 μs | 0.4840 μs |    89.287 μs |
|             GetFcb |   351.736 μs | 1.0840 μs | 0.9052 μs |   351.648 μs |
| GetFisherTransform |    83.870 μs | 0.4313 μs | 0.3823 μs |    83.789 μs |
|      GetForceIndex |    61.722 μs | 0.9941 μs | 0.8812 μs |    61.391 μs |
|         GetFractal |    94.232 μs | 0.7794 μs | 0.6508 μs |    93.960 μs |
|           GetGator |   103.013 μs | 0.3922 μs | 0.3477 μs |   102.981 μs |
|      GetHeikinAshi |   156.856 μs | 1.5004 μs | 1.1714 μs |   156.931 μs |
|             GetHma |   222.908 μs | 1.1191 μs | 1.0468 μs |   222.525 μs |
|     GetHtTrendline |   118.440 μs | 0.3020 μs | 0.2677 μs |   118.454 μs |
|           GetHurst | 1,060.516 μs | 5.8367 μs | 5.1741 μs | 1,059.739 μs |
|        GetIchimoku |   853.281 μs | 3.6966 μs | 3.4578 μs |   852.415 μs |
|            GetKama |    65.390 μs | 0.3625 μs | 0.3390 μs |    65.251 μs |
|         GetKlinger |    73.821 μs | 0.4813 μs | 0.4266 μs |    73.787 μs |
|         GetKeltner |   114.742 μs | 0.6961 μs | 0.6512 μs |   114.582 μs |
|             GetKvo |    73.698 μs | 0.3450 μs | 0.3058 μs |    73.750 μs |
|            GetMacd |    76.204 μs | 0.3030 μs | 0.2531 μs |    76.272 μs |
|     GetMaEnvelopes |    91.821 μs | 0.2732 μs | 0.2282 μs |    91.790 μs |
|            GetMama |   109.541 μs | 0.6069 μs | 0.5677 μs |   109.476 μs |
|        GetMarubozu |   134.166 μs | 0.7934 μs | 0.6625 μs |   134.195 μs |
|             GetMfi |    93.860 μs | 0.4202 μs | 0.3725 μs |    93.806 μs |
|             GetObv |    63.904 μs | 0.4880 μs | 0.4565 μs |    63.811 μs |
|      GetObvWithSma |    74.643 μs | 0.5319 μs | 0.4715 μs |    74.684 μs |
|    GetParabolicSar |    67.939 μs | 0.3245 μs | 0.3036 μs |    67.859 μs |
|     GetPivotPoints |    85.710 μs | 0.5572 μs | 0.5212 μs |    85.727 μs |
|          GetPivots |   167.356 μs | 1.3519 μs | 1.2646 μs |   166.969 μs |
|             GetPmo |    65.503 μs | 0.3762 μs | 0.3519 μs |    65.423 μs |
|             GetPrs |    99.893 μs | 0.6047 μs | 0.4721 μs |   100.004 μs |
|      GetPrsWithSma |   104.606 μs | 0.4735 μs | 0.4198 μs |   104.569 μs |
|             GetPvo |    73.202 μs | 0.3475 μs | 0.3080 μs |    73.112 μs |
|           GetRenko |    91.701 μs | 0.4490 μs | 0.4200 μs |    91.663 μs |
|        GetRenkoAtr |    96.420 μs | 0.5378 μs | 0.5030 μs |    96.466 μs |
|             GetRoc |    52.888 μs | 0.1471 μs | 0.1304 μs |    52.898 μs |
|           GetRocWb |    77.267 μs | 0.6031 μs | 0.5346 μs |    77.386 μs |
|      GetRocWithSma |    61.435 μs | 0.1576 μs | 0.1397 μs |    61.391 μs |
|   GetRollingPivots |   355.222 μs | 1.9860 μs | 1.8577 μs |   354.601 μs |
|             GetRsi |    55.472 μs | 0.2606 μs | 0.2437 μs |    55.423 μs |
|           GetSlope |    94.764 μs | 1.8347 μs | 3.1648 μs |    93.183 μs |
|             GetSma |    59.556 μs | 0.2277 μs | 0.2130 μs |    59.509 μs |
|     GetSmaExtended |   125.479 μs | 0.5065 μs | 0.4230 μs |   125.351 μs |
|             GetSmi |    70.226 μs | 0.1995 μs | 0.1558 μs |    70.216 μs |
|            GetSmma |    51.753 μs | 0.1518 μs | 0.1268 μs |    51.795 μs |
|      GetStarcBands |   126.906 μs | 0.7488 μs | 0.7004 μs |   127.028 μs |
|             GetStc |   119.405 μs | 0.6199 μs | 0.5495 μs |   119.386 μs |
|          GetStdDev |   102.799 μs | 0.6859 μs | 0.6080 μs |   102.765 μs |
|   GetStdDevWithSma |   114.171 μs | 0.7715 μs | 0.6443 μs |   114.196 μs |
|  GetStdDevChannels |   105.034 μs | 0.5550 μs | 0.5192 μs |   104.903 μs |
|           GetStoch |   107.460 μs | 0.6055 μs | 0.5664 μs |   107.609 μs |
|       GetStochSMMA |    91.052 μs | 0.6008 μs | 0.5620 μs |    90.777 μs |
|        GetStochRsi |   118.601 μs | 0.5933 μs | 0.5550 μs |   118.657 μs |
|      GetSuperTrend |    93.913 μs | 1.1304 μs | 1.0574 μs |    93.895 μs |
|              GetT3 |    57.628 μs | 0.3523 μs | 0.3123 μs |    57.557 μs |
|            GetTema |    53.904 μs | 0.2251 μs | 0.2106 μs |    53.831 μs |
|            GetTrix |    55.696 μs | 0.1227 μs | 0.1087 μs |    55.709 μs |
|     GetTrixWithSma |    62.623 μs | 0.5520 μs | 0.5163 μs |    62.461 μs |
|             GetTsi |    64.066 μs | 1.2657 μs | 3.2216 μs |    64.982 μs |
|      GetUlcerIndex |   197.053 μs | 1.1115 μs | 0.9853 μs |   196.830 μs |
|        GetUltimate |    87.455 μs | 1.5153 μs | 1.4175 μs |    86.921 μs |
|  GetVolatilityStop |   108.783 μs | 0.9797 μs | 0.9164 μs |   108.518 μs |
|          GetVortex |    70.959 μs | 0.2840 μs | 0.2372 μs |    70.959 μs |
|            GetVwap |    61.362 μs | 0.4135 μs | 0.3868 μs |    61.271 μs |
|            GetVwma |    83.554 μs | 1.6520 μs | 3.8288 μs |    83.307 μs |
|       GetWilliamsR |   101.976 μs | 1.9972 μs | 4.3838 μs |    99.977 μs |
|             GetWma |    67.604 μs | 1.3510 μs | 2.1428 μs |    66.970 μs |
|          GetZigZag |   155.373 μs | 0.9241 μs | 0.8192 μs |   155.057 μs |
