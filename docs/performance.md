---
title: Performance benchmarks
permalink: /performance/
relative_path: performance.md
layout: page
---

# {{ page.title }} for v2.0.3

These are the execution times for the current indicators using two years of historical daily stock quotes (502 periods) with default or typical parameters.

``` bash
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1766 (21H2)
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.100-preview.5.22307.18
```

## indicators

|             Method |         Mean |     Error |     StdDev |       Median |
|------------------- |-------------:|----------:|-----------:|-------------:|
|             GetAdl |    66.812 μs | 0.4395 μs |  0.4111 μs |    66.759 μs |
|      GetAdlWithSma |    75.831 μs | 0.6835 μs |  0.6393 μs |    75.604 μs |
|             GetAdx |    91.155 μs | 0.2863 μs |  0.2390 μs |    91.175 μs |
|       GetAlligator |    77.326 μs | 0.8087 μs |  0.6314 μs |    77.211 μs |
|            GetAlma |    59.722 μs | 0.2484 μs |  0.2202 μs |    59.658 μs |
|           GetAroon |   119.483 μs | 0.4080 μs |  0.3407 μs |   119.436 μs |
|             GetAtr |    70.681 μs | 0.6822 μs |  0.5696 μs |    70.658 μs |
|         GetAwesome |    74.538 μs | 1.3208 μs |  2.7569 μs |    73.327 μs |
|            GetBeta |   226.336 μs | 1.3649 μs |  2.9086 μs |   225.228 μs |
|          GetBetaUp |   349.185 μs | 2.4987 μs |  2.3373 μs |   348.771 μs |
|        GetBetaDown |   347.913 μs | 3.1108 μs |  2.7577 μs |   346.679 μs |
|         GetBetaAll |   609.262 μs | 9.0627 μs |  8.4773 μs |   610.426 μs |
|  GetBollingerBands |   130.424 μs | 0.6829 μs |  0.6054 μs |   130.449 μs |
|             GetBop |    69.678 μs | 0.4577 μs |  0.4058 μs |    69.647 μs |
|             GetCci |    90.022 μs | 0.2783 μs |  0.2467 μs |    90.032 μs |
|      GetChaikinOsc |    98.395 μs | 0.7230 μs |  0.6409 μs |    98.137 μs |
|      GetChandelier |   114.937 μs | 1.0775 μs |  0.9551 μs |   114.585 μs |
|            GetChop |   114.121 μs | 0.6047 μs |  0.5360 μs |   114.005 μs |
|             GetCmf |   138.138 μs | 0.6871 μs |  0.5737 μs |   138.118 μs |
|      GetConnorsRsi |   184.353 μs | 3.3833 μs |  2.8252 μs |   183.142 μs |
|     GetCorrelation |   222.384 μs | 1.5135 μs |  1.4157 μs |   221.827 μs |
|            GetDema |    56.572 μs | 0.3044 μs |  0.2847 μs |    56.481 μs |
|            GetDoji |   131.300 μs | 1.1346 μs |  1.0613 μs |   131.026 μs |
|        GetDonchian |   298.226 μs | 0.9564 μs |  0.7986 μs |   298.285 μs |
|             GetDpo |    94.204 μs | 0.3571 μs |  0.2982 μs |    94.156 μs |
|        GetElderRay |   106.071 μs | 0.6196 μs |  0.5492 μs |   105.873 μs |
|             GetEma |    54.967 μs | 0.3169 μs |  0.2646 μs |    54.850 μs |
|       GetEmaStream |     9.622 μs | 0.0325 μs |  0.0288 μs |     9.614 μs |
|            GetEpma |   107.232 μs | 0.5238 μs |  0.4644 μs |   107.042 μs |
|             GetFcb |   335.486 μs | 1.3661 μs |  1.2110 μs |   335.036 μs |
| GetFisherTransform |    90.430 μs | 0.5789 μs |  0.5131 μs |    90.264 μs |
|      GetForceIndex |    63.756 μs | 0.3559 μs |  0.2972 μs |    63.831 μs |
|         GetFractal |    96.159 μs | 0.5549 μs |  0.4919 μs |    96.116 μs |
|           GetGator |   116.700 μs | 0.6394 μs |  0.5981 μs |   116.521 μs |
|      GetHeikinAshi |   150.818 μs | 1.2129 μs |  2.7623 μs |   149.936 μs |
|             GetHma |   187.193 μs | 1.7505 μs |  1.4617 μs |   186.716 μs |
|     GetHtTrendline |   122.926 μs | 0.8247 μs |  0.7714 μs |   122.614 μs |
|           GetHurst | 1,105.037 μs | 7.1121 μs |  6.3047 μs | 1,104.669 μs |
|        GetIchimoku |   828.367 μs | 4.2532 μs |  3.9784 μs |   827.829 μs |
|            GetKama |    69.923 μs | 0.3374 μs |  0.2991 μs |    69.873 μs |
|         GetKlinger |    73.962 μs | 1.4503 μs |  2.7239 μs |    72.355 μs |
|         GetKeltner |   123.614 μs | 0.6187 μs |  0.5485 μs |   123.474 μs |
|             GetKvo |    77.753 μs | 0.4498 μs |  0.3987 μs |    77.640 μs |
|            GetMacd |    97.949 μs | 0.5975 μs |  0.5589 μs |    97.668 μs |
|     GetMaEnvelopes |    89.186 μs | 0.3849 μs |  0.3601 μs |    89.173 μs |
|            GetMama |   120.426 μs | 0.2541 μs |  0.1984 μs |   120.439 μs |
|        GetMarubozu |   153.157 μs | 0.8336 μs |  0.7390 μs |   153.045 μs |
|             GetMfi |    92.212 μs | 0.6365 μs |  0.5954 μs |    91.925 μs |
|             GetObv |    66.412 μs | 0.3990 μs |  0.3331 μs |    66.338 μs |
|      GetObvWithSma |    80.260 μs | 0.6102 μs |  0.5409 μs |    79.975 μs |
|    GetParabolicSar |    71.027 μs | 1.4169 μs |  2.5186 μs |    69.783 μs |
|     GetPivotPoints |    97.537 μs | 0.6642 μs |  0.6213 μs |    97.467 μs |
|          GetPivots |   170.757 μs | 0.9049 μs |  0.8022 μs |   170.791 μs |
|             GetPmo |    76.486 μs | 0.3457 μs |  0.2886 μs |    76.474 μs |
|             GetPrs |   109.505 μs | 0.3410 μs |  0.2848 μs |   109.501 μs |
|      GetPrsWithSma |   116.768 μs | 0.4675 μs |  0.3904 μs |   116.711 μs |
|             GetPvo |    91.335 μs | 0.6300 μs |  0.5893 μs |    91.277 μs |
|           GetRenko |    94.268 μs | 0.6079 μs |  0.5389 μs |    94.291 μs |
|        GetRenkoAtr |    98.405 μs | 0.4613 μs |  0.4315 μs |    98.385 μs |
|             GetRoc |    58.932 μs | 0.2134 μs |  0.1892 μs |    58.908 μs |
|           GetRocWb |    85.245 μs | 0.5565 μs |  0.5206 μs |    85.132 μs |
|      GetRocWithSma |    67.944 μs | 0.2361 μs |  0.1843 μs |    67.991 μs |
|   GetRollingPivots |   368.079 μs | 6.3832 μs | 12.8943 μs |   362.858 μs |
|             GetRsi |    58.625 μs | 0.2296 μs |  0.2148 μs |    58.575 μs |
|           GetSlope |   109.038 μs | 0.6798 μs |  0.5308 μs |   109.204 μs |
|             GetSma |    62.312 μs | 0.4249 μs |  0.3974 μs |    62.463 μs |
|     GetSmaAnalysis |    90.786 μs | 0.5836 μs |  0.5459 μs |    90.555 μs |
|             GetSmi |    73.510 μs | 0.4111 μs |  0.3846 μs |    73.440 μs |
|            GetSmma |    56.877 μs | 0.5259 μs |  0.4391 μs |    56.947 μs |
|      GetStarcBands |   134.388 μs | 0.8294 μs |  0.6475 μs |   134.473 μs |
|             GetStc |   147.019 μs | 1.6509 μs |  1.5442 μs |   146.949 μs |
|          GetStdDev |   131.800 μs | 1.1587 μs |  0.9676 μs |   131.789 μs |
|   GetStdDevWithSma |   146.078 μs | 0.9091 μs |  0.8059 μs |   145.801 μs |
|  GetStdDevChannels |   121.912 μs | 1.1015 μs |  1.0304 μs |   121.952 μs |
|           GetStoch |   116.371 μs | 0.5438 μs |  0.5087 μs |   116.289 μs |
|       GetStochSMMA |    92.859 μs | 1.8121 μs |  3.0276 μs |    91.040 μs |
|        GetStochRsi |   132.046 μs | 0.4697 μs |  0.3922 μs |   131.928 μs |
|      GetSuperTrend |    98.195 μs | 1.9382 μs |  3.5926 μs |    96.550 μs |
|              GetT3 |    67.083 μs | 1.8129 μs |  5.3455 μs |    66.132 μs |
|            GetTema |    58.943 μs | 0.3371 μs |  0.3154 μs |    58.915 μs |
|            GetTrix |    64.463 μs | 0.5608 μs |  0.5246 μs |    64.331 μs |
|     GetTrixWithSma |    70.809 μs | 0.3368 μs |  0.2985 μs |    70.768 μs |
|             GetTsi |    69.088 μs | 0.3275 μs |  0.3064 μs |    69.035 μs |
|      GetUlcerIndex |   227.698 μs | 1.0465 μs |  0.9277 μs |   227.358 μs |
|        GetUltimate |    99.336 μs | 0.4985 μs |  0.4419 μs |    99.284 μs |
|  GetVolatilityStop |   119.069 μs | 2.1555 μs |  1.9108 μs |   118.283 μs |
|          GetVortex |    77.599 μs | 0.2764 μs |  0.2450 μs |    77.575 μs |
|            GetVwap |    64.629 μs | 0.2275 μs |  0.2016 μs |    64.677 μs |
|            GetVwma |    82.783 μs | 0.5888 μs |  0.5219 μs |    82.546 μs |
|       GetWilliamsR |   116.290 μs | 0.8125 μs |  0.7600 μs |   116.008 μs |
|             GetWma |    70.137 μs | 0.1948 μs |  0.1521 μs |    70.108 μs |
|          GetZigZag |   166.534 μs | 0.6495 μs |  0.5424 μs |   166.452 μs |
