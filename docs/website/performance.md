# Performance benchmarks for v2.6.0

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
| GetFcb             |  45.71 μs |  12.963 μs | 0.710 μs |
| GetFisherTransform |  53.18 μs |   3.841 μs | 0.211 μs |
| GetForceIndex      |  25.03 μs |   1.745 μs | 0.096 μs |
| GetFractal         |  99.68 μs |  10.595 μs | 0.581 μs |
| GetGator           |  70.81 μs |   0.904 μs | 0.050 μs |
| GetHeikinAshi      |  41.39 μs |   3.918 μs | 0.215 μs |
| GetHma             |  37.52 μs |   2.722 μs | 0.149 μs |
| GetHtLine          |  20.89 μs |   1.219 μs | 0.067 μs |
| GetHurst           | 349.59 μs |  11.290 μs | 0.619 μs |
| GetIchimoku        |  85.47 μs |   1.151 μs | 0.063 μs |
| GetKama            |  41.39 μs |   1.635 μs | 0.090 μs |
| GetKeltner         |  73.59 μs |   5.832 μs | 0.320 μs |
| GetKvo             |  77.42 μs |   2.892 μs | 0.159 μs |
| GetMacd            |  56.25 μs |   3.757 μs | 0.206 μs |
| GetMaEnvelopes     |  44.38 μs |   3.449 μs | 0.189 μs |
| GetMama            |  62.25 μs |   3.131 μs | 0.172 μs |
| GetMarubozu        | 101.88 μs |   3.873 μs | 0.212 μs |
| GetMfi             |  58.29 μs |   4.621 μs | 0.253 μs |
| GetObv             |  25.03 μs |   0.613 μs | 0.034 μs |
| GetParabolicSar    |  68.37 μs |   1.158 μs | 0.063 μs |
| GetPivotPoints     |  36.97 μs |   3.995 μs | 0.219 μs |
| GetPivots          |  41.71 μs |   1.889 μs | 0.104 μs |
| GetPmo             |  62.99 μs |   5.925 μs | 0.325 μs |
| GetPrs             |  27.71 μs |   0.753 μs | 0.041 μs |
| GetPvo             |  51.17 μs |   2.759 μs | 0.151 μs |
| GetRenko           | 174.15 μs |   7.333 μs | 0.402 μs |
| GetRoc             |  23.71 μs |   0.851 μs | 0.047 μs |
| GetRocWb           |  27.54 μs |   2.481 μs | 0.136 μs |
| GetRollingPivots   |  58.57 μs |   7.797 μs | 0.427 μs |
| GetRsi             |  33.30 μs |   0.977 μs | 0.054 μs |
| GetSlope           | 260.87 μs |  11.064 μs | 0.606 μs |
| GetSma             |  38.71 μs |   1.749 μs | 0.096 μs |
| GetSmi             |  80.05 μs |   4.315 μs | 0.237 μs |
| GetSmma            |  22.62 μs |   1.126 μs | 0.062 μs |
| GetStarcBands      |  81.82 μs |   8.578 μs | 0.470 μs |
| GetStc             |  76.54 μs |   4.196 μs | 0.230 μs |
| GetStdDev          |  43.47 μs |   2.566 μs | 0.141 μs |
| GetStdDevChannels  |  58.38 μs |   2.372 μs | 0.130 μs |
| GetStoch           |  47.62 μs |   0.958 μs | 0.053 μs |
| GetStochRsi        |  52.77 μs |   0.892 μs | 0.049 μs |
| GetSuperTrend      |  67.84 μs |   2.647 μs | 0.145 μs |
| GetT3              |  93.29 μs |   5.127 μs | 0.281 μs |
| GetTema            |  51.68 μs |   2.669 μs | 0.146 μs |
| GetTrix            |  61.59 μs |   3.180 μs | 0.174 μs |
| GetTsi             |  61.79 μs |   2.172 μs | 0.119 μs |
| GetUlcerIndex      |  62.76 μs |   0.861 μs | 0.047 μs |
| GetUltimate        |  57.12 μs |   0.569 μs | 0.031 μs |
| GetVolatilityStop  |  44.57 μs |   0.972 μs | 0.053 μs |
| GetVortex          |  42.05 μs |   2.749 μs | 0.151 μs |
| GetVwap            |  46.84 μs |   5.293 μs | 0.290 μs |
| GetVwma            |  29.26 μs |   2.160 μs | 0.118 μs |
| GetWilliamsR       |  39.89 μs |   0.981 μs | 0.054 μs |
| GetWma             |  40.61 μs |   2.610 μs | 0.143 μs |
| GetZigZag          |  48.02 μs |   3.111 μs | 0.171 μs |
