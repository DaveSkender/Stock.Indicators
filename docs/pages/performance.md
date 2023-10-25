---
title: Performance benchmarks
description: The Stock Indicators for .NET library is built for speed and production workloads.  Compare our execution times with other options.
permalink: /performance/
relative_path: pages/performance.md
layout: page
---

# {{ page.title }} for v2.4.11

These are the execution times for the current indicators using two years of historical daily stock quotes (502 periods) with default or typical parameters.

``` bash
BenchmarkDotNet v0.13.9, Windows 10
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK 7.0.402
  [Host]     : .NET 7.0.12 (7.0.1223.47720), X64 RyuJIT AVX2
```

## indicators

| Method             | Mean         | Error     | StdDev    | Median       |
|------------------- |-------------:|----------:|----------:|-------------:|
| GetAdl             |    59.039 μs | 0.5747 μs | 0.5094 μs |    58.952 μs |
| GetAdlWithSma      |    69.657 μs | 0.6687 μs | 0.5584 μs |    69.452 μs |
| GetAdx             |    71.841 μs | 1.2287 μs | 2.5099 μs |    70.816 μs |
| GetAlligator       |    69.392 μs | 0.3080 μs | 0.2572 μs |    69.406 μs |
| GetAlma            |    58.394 μs | 1.1506 μs | 2.4769 μs |    57.207 μs |
| GetAroon           |   118.119 μs | 0.2383 μs | 0.2112 μs |   118.115 μs |
| GetAtr             |    61.479 μs | 0.2266 μs | 0.2120 μs |    61.404 μs |
| GetAtrStop         |    88.143 μs | 0.2664 μs | 0.2362 μs |    88.111 μs |
| GetAwesome         |    71.855 μs | 0.2753 μs | 0.2440 μs |    71.754 μs |
| GetBeta            |   200.507 μs | 0.7199 μs | 0.6382 μs |   200.542 μs |
| GetBetaUp          |   212.544 μs | 2.6352 μs | 2.2005 μs |   211.601 μs |
| GetBetaDown        |   208.804 μs | 0.4363 μs | 0.4081 μs |   208.922 μs |
| GetBetaAll         |   442.614 μs | 4.1229 μs | 3.4428 μs |   441.238 μs |
| GetBollingerBands  |   103.036 μs | 0.3631 μs | 0.3396 μs |   102.982 μs |
| GetBop             |    63.915 μs | 0.1023 μs | 0.0798 μs |    63.937 μs |
| GetCci             |    76.254 μs | 0.3576 μs | 0.3170 μs |    76.169 μs |
| GetChaikinOsc      |    83.022 μs | 0.2467 μs | 0.2060 μs |    83.036 μs |
| GetChandelier      |    99.088 μs | 0.1846 μs | 0.1636 μs |    99.079 μs |
| GetChop            |   108.540 μs | 0.3124 μs | 0.2769 μs |   108.419 μs |
| GetCmf             |   123.722 μs | 0.2383 μs | 0.2113 μs |   123.686 μs |
| GetCmo             |    89.577 μs | 0.2785 μs | 0.2326 μs |    89.559 μs |
| GetConnorsRsi      |   183.793 μs | 0.2919 μs | 0.2588 μs |   183.831 μs |
| GetCorrelation     |   159.692 μs | 0.3176 μs | 0.2815 μs |   159.742 μs |
| GetDema            |    50.569 μs | 0.1348 μs | 0.1195 μs |    50.529 μs |
| GetDoji            |   102.564 μs | 0.2502 μs | 0.2089 μs |   102.621 μs |
| GetDonchian        |   296.274 μs | 1.7686 μs | 1.6543 μs |   296.023 μs |
| GetDpo             |    87.126 μs | 0.4292 μs | 0.3805 μs |    87.049 μs |
| GetElderRay        |    97.718 μs | 0.3713 μs | 0.3101 μs |    97.690 μs |
| GetEma             |    52.313 μs | 0.0998 μs | 0.0885 μs |    52.286 μs |
| GetEmaStream       |     9.583 μs | 0.0318 μs | 0.0297 μs |     9.582 μs |
| GetEpma            |    90.369 μs | 0.1802 μs | 0.1504 μs |    90.345 μs |
| GetFcb             |   323.945 μs | 1.1806 μs | 1.0466 μs |   323.639 μs |
| GetFisherTransform |    86.202 μs | 1.0737 μs | 1.6071 μs |    85.535 μs |
| GetForceIndex      |    58.920 μs | 0.0919 μs | 0.0815 μs |    58.917 μs |
| GetFractal         |    88.772 μs | 0.2520 μs | 0.2104 μs |    88.747 μs |
| GetGator           |   105.737 μs | 0.2060 μs | 0.1927 μs |   105.707 μs |
| GetHeikinAshi      |   149.540 μs | 0.2668 μs | 0.2495 μs |   149.501 μs |
| GetHma             |   169.999 μs | 0.4300 μs | 0.3590 μs |   169.946 μs |
| GetHtTrendline     |   117.775 μs | 0.1738 μs | 0.1451 μs |   117.750 μs |
| GetHurst           | 1,022.251 μs | 3.0030 μs | 2.8090 μs | 1,021.373 μs |
| GetIchimoku        |   825.739 μs | 3.0491 μs | 2.7030 μs |   825.078 μs |
| GetKama            |    63.739 μs | 0.1591 μs | 0.1410 μs |    63.773 μs |
| GetKlinger         |    69.183 μs | 0.3546 μs | 0.3317 μs |    69.165 μs |
| GetKeltner         |   108.516 μs | 0.6956 μs | 0.6506 μs |   108.764 μs |
| GetKvo             |    69.480 μs | 0.2219 μs | 0.1967 μs |    69.388 μs |
| GetMacd            |    86.834 μs | 0.4236 μs | 0.3962 μs |    86.724 μs |
| GetMaEnvelopes     |    81.267 μs | 0.4835 μs | 0.4038 μs |    81.160 μs |
| GetMama            |   107.137 μs | 0.2352 μs | 0.2200 μs |   107.119 μs |
| GetMarubozu        |   130.650 μs | 0.4685 μs | 0.4383 μs |   130.462 μs |
| GetMfi             |    85.911 μs | 0.1860 μs | 0.1649 μs |    85.939 μs |
| GetObv             |    59.423 μs | 0.1679 μs | 0.1488 μs |    59.375 μs |
| GetObvWithSma      |    71.693 μs | 0.1535 μs | 0.1436 μs |    71.684 μs |
| GetParabolicSar    |    63.203 μs | 0.2489 μs | 0.2328 μs |    63.175 μs |
| GetPivotPoints     |    73.940 μs | 0.2168 μs | 0.2028 μs |    73.889 μs |
| GetPivots          |   162.539 μs | 0.4111 μs | 0.3845 μs |   162.302 μs |
| GetPmo             |    69.433 μs | 0.3262 μs | 0.3051 μs |    69.365 μs |
| GetPrs             |    93.780 μs | 0.1971 μs | 0.1844 μs |    93.757 μs |
| GetPrsWithSma      |   103.931 μs | 0.7102 μs | 0.6296 μs |   103.713 μs |
| GetPvo             |    79.227 μs | 1.3457 μs | 2.0951 μs |    78.449 μs |
| GetRenko           |    92.727 μs | 0.2168 μs | 0.1922 μs |    92.715 μs |
| GetRenkoAtr        |    97.115 μs | 0.2640 μs | 0.2340 μs |    97.109 μs |
| GetRoc             |    58.376 μs | 0.0813 μs | 0.0721 μs |    58.378 μs |
| GetRocWb           |    77.738 μs | 0.1986 μs | 0.1857 μs |    77.695 μs |
| GetRocWithSma      |    68.037 μs | 0.2058 μs | 0.1719 μs |    67.987 μs |
| GetRollingPivots   |   337.463 μs | 0.6625 μs | 0.5873 μs |   337.428 μs |
| GetRsi             |    51.189 μs | 0.1106 μs | 0.1035 μs |    51.233 μs |
| GetSlope           |    90.432 μs | 0.1067 μs | 0.0891 μs |    90.453 μs |
| GetSma             |    55.180 μs | 0.0809 μs | 0.0717 μs |    55.166 μs |
| GetSmaAnalysis     |    78.865 μs | 0.2377 μs | 0.2224 μs |    78.806 μs |
| GetSmi             |    63.542 μs | 0.2992 μs | 0.2799 μs |    63.493 μs |
| GetSmma            |    49.599 μs | 0.1641 μs | 0.1370 μs |    49.628 μs |
| GetStarcBands      |   109.124 μs | 0.2264 μs | 0.1767 μs |   109.061 μs |
| GetStc             |   123.629 μs | 0.4272 μs | 0.3787 μs |   123.626 μs |
| GetStdDev          |   102.272 μs | 0.2176 μs | 0.2035 μs |   102.199 μs |
| GetStdDevWithSma   |   117.003 μs | 0.2509 μs | 0.2225 μs |   116.955 μs |
| GetStdDevChannels  |   107.457 μs | 3.1051 μs | 9.1555 μs |   100.293 μs |
| GetStoch           |    99.240 μs | 0.1936 μs | 0.1716 μs |    99.205 μs |
| GetStochSMMA       |    83.248 μs | 0.2030 μs | 0.1585 μs |    83.232 μs |
| GetStochRsi        |   112.795 μs | 0.2191 μs | 0.2049 μs |   112.787 μs |
| GetSuperTrend      |    88.710 μs | 0.2540 μs | 0.2251 μs |    88.710 μs |
| GetT3              |    56.609 μs | 0.0885 μs | 0.0785 μs |    56.584 μs |
| GetTema            |    51.801 μs | 0.1635 μs | 0.1530 μs |    51.750 μs |
| GetTr              |    58.327 μs | 0.1389 μs | 0.1160 μs |    58.332 μs |
| GetTrix            |    57.605 μs | 0.0856 μs | 0.0801 μs |    57.636 μs |
| GetTrixWithSma     |    60.952 μs | 0.1461 μs | 0.1295 μs |    60.969 μs |
| GetTsi             |    57.475 μs | 0.1275 μs | 0.1065 μs |    57.464 μs |
| GetUlcerIndex      |   223.441 μs | 0.9974 μs | 0.9796 μs |   223.031 μs |
| GetUltimate        |    85.577 μs | 0.2859 μs | 0.2232 μs |    85.546 μs |
| GetVolatilityStop  |   104.920 μs | 1.6186 μs | 1.2637 μs |   104.525 μs |
| GetVortex          |    69.805 μs | 0.1676 μs | 0.1485 μs |    69.825 μs |
| GetVwap            |    59.109 μs | 0.1576 μs | 0.1397 μs |    59.096 μs |
| GetVwma            |    71.345 μs | 0.2088 μs | 0.1954 μs |    71.267 μs |
| GetWilliamsR       |   100.541 μs | 0.1775 μs | 0.1386 μs |   100.536 μs |
| GetWma             |    62.707 μs | 0.1868 μs | 0.1656 μs |    62.660 μs |
| GetZigZag          |   165.914 μs | 0.3932 μs | 0.3678 μs |   165.698 μs |
