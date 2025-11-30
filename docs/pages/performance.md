---
title: Performance benchmarks
description: The Stock Indicators for .NET library is built for speed and production workloads.  Compare our execution times with other options.
permalink: /performance/
relative_path: pages/performance.md
layout: page
---

# {{ page.title }} for v2.6.0

These are the execution times for the current indicators using two years of historical daily stock quotes (502 periods) with default or typical parameters.

``` bash
BenchmarkDotNet v0.14.0, Ubuntu 22.04.5 LTS (Jammy Jellyfish)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.100
  [Host]   : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
```

| Method             | Mean      | Error      | StdDev   |
|------------------- |----------:|-----------:|---------:|
| GetAdl             |  34.89 μs |   2.409 μs | 0.132 μs |
| GetAdx             |  42.48 μs |   2.417 μs | 0.132 μs |
| GetAlligator       |  32.84 μs |   0.380 μs | 0.021 μs |
| GetAlma            |  27.08 μs |   0.524 μs | 0.029 μs |
| GetAroon           |  54.64 μs |   0.722 μs | 0.040 μs |
| GetAtr             |  36.40 μs |   4.916 μs | 0.269 μs |
| GetAtrStop         |  52.84 μs |   5.843 μs | 0.320 μs |
| GetAwesome         |  48.42 μs |   0.809 μs | 0.044 μs |
| GetBeta            | 164.01 μs |  23.671 μs | 1.297 μs |
| GetBetaUp          | 128.36 μs |   8.187 μs | 0.449 μs |
| GetBetaDown        | 133.49 μs |   5.559 μs | 0.305 μs |
| GetBetaAll         | 357.86 μs |  10.892 μs | 0.597 μs |
| GetBollingerBands  |  69.14 μs |   2.992 μs | 0.164 μs |
| GetBop             |  37.33 μs |   3.893 μs | 0.213 μs |
| GetCci             |  46.97 μs |   3.619 μs | 0.198 μs |
| GetChaikinOsc      |  55.48 μs |   2.268 μs | 0.124 μs |
| GetChandelier      |  53.86 μs |   6.396 μs | 0.351 μs |
| GetChop            |  59.92 μs |   0.953 μs | 0.052 μs |
| GetCmf             |  70.13 μs |   7.516 μs | 0.412 μs |
| GetCmo             |  33.04 μs |   2.522 μs | 0.138 μs |
| GetConnorsRsi      | 109.98 μs |   2.155 μs | 0.118 μs |
| GetCorrelation     |  96.95 μs |   7.741 μs | 0.424 μs |
| GetDema            |  22.99 μs |   1.052 μs | 0.058 μs |
| GetDoji            |  69.18 μs |   5.349 μs | 0.293 μs |
| GetDonchian        | 204.30 μs |  10.312 μs | 0.565 μs |
| GetDpo             |  48.21 μs |   1.477 μs | 0.081 μs |
| GetDynamic         |  39.30 μs |   1.816 μs | 0.100 μs |
| GetElderRay        |  53.86 μs |   4.422 μs | 0.242 μs |
| GetEma             |  22.31 μs |   1.314 μs | 0.072 μs |
| GetEpma            |  55.52 μs |   3.782 μs | 0.207 μs |
| GetFcb             | 217.82 μs |   6.294 μs | 0.345 μs |
| GetFisherTransform |  52.84 μs |   1.077 μs | 0.059 μs |
| GetForceIndex      |  32.86 μs |   2.649 μs | 0.145 μs |
| GetFractal         |  48.06 μs |   5.058 μs | 0.277 μs |
| GetGator           |  59.64 μs |   9.488 μs | 0.520 μs |
| GetHeikinAshi      | 109.37 μs |   1.923 μs | 0.105 μs |
| GetHma             |  99.47 μs |   5.452 μs | 0.299 μs |
| GetHtTrendline     |  84.69 μs |  18.501 μs | 1.014 μs |
| GetHurst           | 838.65 μs | 127.955 μs | 7.014 μs |
| GetIchimoku        | 640.07 μs |  44.858 μs | 2.459 μs |
| GetKama            |  30.06 μs |   4.795 μs | 0.263 μs |
| GetKlinger         |  43.21 μs |   2.307 μs | 0.126 μs |
| GetKeltner         |  64.88 μs |  21.930 μs | 1.202 μs |
| GetKvo             |  43.35 μs |   3.056 μs | 0.168 μs |
| GetMacd            |  46.94 μs |   4.013 μs | 0.220 μs |
| GetMaEnvelopes     |  43.99 μs |   1.648 μs | 0.090 μs |
| GetMama            |  75.49 μs |   1.308 μs | 0.072 μs |
| GetMarubozu        |  78.81 μs |   3.513 μs | 0.193 μs |
| GetMfi             |  43.31 μs |   1.323 μs | 0.073 μs |
| GetObv             |  33.07 μs |   1.152 μs | 0.063 μs |
| GetObvWithSma      |  42.54 μs |   1.500 μs | 0.082 μs |
| GetParabolicSar    |  36.18 μs |   0.569 μs | 0.031 μs |
| GetPivotPoints     |  45.84 μs |   2.456 μs | 0.135 μs |
| GetPivots          |  87.39 μs |   1.025 μs | 0.056 μs |
| GetPmo             |  39.11 μs |   1.240 μs | 0.068 μs |
| GetPrs             |  40.60 μs |   2.618 μs | 0.144 μs |
| GetPrsWithSma      |  43.07 μs |   1.737 μs | 0.095 μs |
| GetPvo             |  44.81 μs |   2.604 μs | 0.143 μs |
| GetRenko           |  59.82 μs |   3.289 μs | 0.180 μs |
| GetRenkoAtr        |  67.58 μs |  24.816 μs | 1.360 μs |
| GetRoc             |  24.04 μs |   1.218 μs | 0.067 μs |
| GetRocWb           |  40.13 μs |   3.526 μs | 0.193 μs |
| GetRocWithSma      |  30.52 μs |   2.540 μs | 0.139 μs |
| GetRollingPivots   | 243.31 μs |  37.635 μs | 2.063 μs |
| GetRsi             |  25.91 μs |   0.812 μs | 0.044 μs |
| GetSlope           |  56.35 μs |   9.701 μs | 0.532 μs |
| GetSma             |  24.90 μs |   2.631 μs | 0.144 μs |
| GetSmaAnalysis     |  42.43 μs |   0.780 μs | 0.043 μs |
| GetSmi             |  37.50 μs |   1.633 μs | 0.089 μs |
| GetSmma            |  23.26 μs |   2.726 μs | 0.149 μs |
| GetStarcBands      |  71.22 μs |  27.014 μs | 1.481 μs |
| GetStc             |  82.32 μs |   4.596 μs | 0.252 μs |
| GetStdDev          |  71.29 μs |   1.644 μs | 0.090 μs |
| GetStdDevWithSma   |  76.73 μs |   1.728 μs | 0.095 μs |
| GetStdDevChannels  |  66.38 μs |   8.307 μs | 0.455 μs |
| GetStoch           |  51.89 μs |   3.650 μs | 0.200 μs |
| GetStochSMMA       |  50.08 μs |   3.252 μs | 0.178 μs |
| GetStochRsi        |  61.82 μs |   3.847 μs | 0.211 μs |
| GetSuperTrend      |  54.21 μs |  11.806 μs | 0.647 μs |
| GetT3              |  26.51 μs |   0.176 μs | 0.010 μs |
| GetTema            |  23.74 μs |   1.415 μs | 0.078 μs |
| GetTr              |  33.63 μs |   5.232 μs | 0.287 μs |
| GetTrix            |  26.56 μs |   1.217 μs | 0.067 μs |
| GetTrixWithSma     |  30.39 μs |   1.462 μs | 0.080 μs |
| GetTsi             |  29.67 μs |   6.213 μs | 0.341 μs |
| GetUlcerIndex      |  95.21 μs |   2.543 μs | 0.139 μs |
| GetUltimate        |  58.52 μs |   2.435 μs | 0.133 μs |
| GetVolatilityStop  |  61.17 μs |  22.487 μs | 1.233 μs |
| GetVortex          |  43.12 μs |   1.817 μs | 0.100 μs |
| GetVwap            |  33.56 μs |   1.656 μs | 0.091 μs |
| GetVwma            |  42.38 μs |   1.098 μs | 0.060 μs |
| GetWilliamsR       |  52.27 μs |   0.135 μs | 0.007 μs |
| GetWma             |  31.98 μs |   2.411 μs | 0.132 μs |
| GetZigZag          |  99.35 μs |   5.101 μs | 0.280 μs |
