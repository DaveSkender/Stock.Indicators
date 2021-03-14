# Performance benchmarks for v1.10.0

These are the execution times for the current indicators using two years of historical daily stock quotes (502 periods) with default or typical parameters.

``` bash
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19042
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=5.0.201
  [Host]     : .NET Core 5.0.4 (CoreCLR 5.0.421.11614, CoreFX 5.0.421.11614), X64 RyuJIT
  DefaultJob : .NET Core 5.0.4 (CoreCLR 5.0.421.11614, CoreFX 5.0.421.11614), X64 RyuJIT
```

## indicators

|            Method |        Mean |     Error |    StdDev |      Median |
|------------------ |------------:|----------:|----------:|------------:|
|            GetAdl |   144.86 μs |  1.048 μs |  0.875 μs |   144.58 μs |
|     GetAdlWithSma |   382.27 μs |  2.757 μs |  2.444 μs |   381.91 μs |
|            GetAdx |   749.17 μs | 10.428 μs |  9.244 μs |   745.71 μs |
|      GetAlligator |   246.65 μs |  1.025 μs |  0.909 μs |   246.38 μs |
|           GetAlma |   217.38 μs |  1.205 μs |  1.127 μs |   217.18 μs |
|          GetAroon |   352.73 μs |  6.688 μs | 12.396 μs |   347.75 μs |
|            GetAtr |   159.76 μs |  0.494 μs |  0.385 μs |   159.87 μs |
|        GetAwesome |   329.78 μs |  4.923 μs |  5.669 μs |   327.18 μs |
|           GetBeta |   972.82 μs |  5.823 μs |  5.162 μs |   972.64 μs |
| GetBollingerBands |   452.60 μs |  2.950 μs |  2.463 μs |   451.73 μs |
|            GetBop |   281.24 μs |  1.317 μs |  1.232 μs |   280.78 μs |
|            GetCci |   846.26 μs |  2.555 μs |  2.133 μs |   846.58 μs |
|     GetChaikinOsc |   269.59 μs |  1.695 μs |  1.586 μs |   269.02 μs |
|     GetChandelier |   363.52 μs |  1.124 μs |  0.878 μs |   363.41 μs |
|           GetChop |   302.77 μs |  1.299 μs |  1.014 μs |   302.84 μs |
|            GetCmf |   669.83 μs |  3.413 μs |  3.026 μs |   669.25 μs |
|     GetConnorsRsi | 1,228.11 μs | 21.669 μs | 32.433 μs | 1,214.79 μs |
|    GetCorrelation |   885.73 μs |  4.027 μs |  3.570 μs |   886.12 μs |
|       GetDonchian |   342.20 μs |  2.241 μs |  2.096 μs |   341.30 μs |
|      GetDoubleEma |   184.09 μs |  0.944 μs |  0.883 μs |   184.04 μs |
|       GetElderRay |   166.20 μs |  1.060 μs |  0.992 μs |   165.88 μs |
|            GetEma |   100.43 μs |  0.472 μs |  0.394 μs |   100.37 μs |
|           GetEpma | 1,397.26 μs | 11.565 μs |  9.657 μs | 1,394.87 μs |
|            GetFcb |   397.88 μs |  1.035 μs |  0.864 μs |   397.64 μs |
|     GetForceIndex |   128.92 μs |  0.360 μs |  0.300 μs |   129.00 μs |
|        GetFractal |   104.26 μs |  0.318 μs |  0.266 μs |   104.36 μs |
|          GetGator |   301.83 μs |  1.337 μs |  1.116 μs |   301.83 μs |
|     GetHeikinAshi |   176.10 μs |  1.028 μs |  0.961 μs |   175.88 μs |
|            GetHma | 1,383.44 μs |  5.883 μs |  5.215 μs | 1,381.70 μs |
|    GetHtTrendline |   178.74 μs |  0.301 μs |  0.235 μs |   178.71 μs |
|       GetIchimoku |   982.20 μs |  5.026 μs |  4.197 μs |   982.29 μs |
|           GetKama |   325.44 μs |  1.024 μs |  0.800 μs |   325.31 μs |
|        GetKeltner |   474.66 μs |  3.797 μs |  3.171 μs |   473.48 μs |
|           GetMacd |   219.34 μs |  0.734 μs |  0.651 μs |   219.36 μs |
|    GetMaEnvelopes |   150.43 μs |  0.987 μs |  0.824 μs |   150.09 μs |
|           GetMama |   291.04 μs |  1.036 μs |  0.865 μs |   291.40 μs |
|            GetMfi |   485.82 μs |  1.696 μs |  1.417 μs |   485.58 μs |
|            GetObv |    62.63 μs |  0.525 μs |  0.491 μs |    62.66 μs |
|     GetObvWithSma |   139.26 μs |  0.745 μs |  0.582 μs |   139.26 μs |
|   GetParabolicSar |    93.59 μs |  0.735 μs |  0.688 μs |    93.47 μs |
|    GetPivotPoints |    97.50 μs |  0.781 μs |  0.731 μs |    97.82 μs |
|            GetPmo |   269.41 μs |  2.110 μs |  1.762 μs |   268.97 μs |
|            GetPrs |   134.67 μs |  1.141 μs |  1.012 μs |   134.75 μs |
|     GetPrsWithSma |   209.84 μs |  0.756 μs |  0.631 μs |   209.72 μs |
|            GetPvo |   346.99 μs |  1.991 μs |  1.862 μs |   347.10 μs |
|            GetRoc |    97.02 μs |  0.601 μs |  0.533 μs |    96.96 μs |
|     GetRocWithSma |   365.92 μs |  0.762 μs |  0.637 μs |   366.20 μs |
|            GetRsi |   346.99 μs |  1.888 μs |  1.674 μs |   346.70 μs |
|          GetSlope |   905.65 μs | 17.410 μs | 23.242 μs |   894.74 μs |
|            GetSma |   106.84 μs |  0.591 μs |  0.524 μs |   106.73 μs |
|    GetSmaExtended |   957.40 μs |  3.893 μs |  3.251 μs |   956.90 μs |
|           GetSmma |    98.01 μs |  0.422 μs |  0.395 μs |    97.82 μs |
|     GetStarcBands |   431.63 μs |  2.096 μs |  1.960 μs |   430.88 μs |
|         GetStdDev |   308.12 μs |  5.704 μs | 10.285 μs |   302.93 μs |
|  GetStdDevWithSma |   385.34 μs |  1.875 μs |  1.662 μs |   385.66 μs |
| GetStdDevChannels |   970.08 μs |  5.036 μs |  4.710 μs |   968.45 μs |
|          GetStoch |   408.54 μs |  2.380 μs |  2.227 μs |   407.52 μs |
|       GetStochRsi |   717.37 μs |  2.456 μs |  2.051 μs |   717.38 μs |
|     GetSuperTrend |   303.79 μs |  0.889 μs |  0.743 μs |   303.80 μs |
|      GetTripleEma |   266.70 μs |  0.659 μs |  0.515 μs |   266.72 μs |
|           GetTrix |   326.69 μs |  0.946 μs |  0.790 μs |   326.72 μs |
|    GetTrixWithSma |   385.88 μs |  1.999 μs |  1.669 μs |   385.50 μs |
|            GetTsi |   378.30 μs |  2.406 μs |  2.132 μs |   377.49 μs |
|             GetT3 |   476.28 μs |  2.313 μs |  2.164 μs |   475.29 μs |
|     GetUlcerIndex | 1,541.53 μs |  2.150 μs |  1.795 μs | 1,541.80 μs |
|       GetUltimate |   556.54 μs |  1.141 μs |  0.891 μs |   556.49 μs |
|         GetVolSma |   120.54 μs |  0.525 μs |  0.465 μs |   120.50 μs |
|         GetVortex |   286.03 μs |  1.183 μs |  0.988 μs |   285.88 μs |
|           GetVwap |    98.80 μs |  0.607 μs |  0.567 μs |    98.54 μs |
|      GetWilliamsR |   289.04 μs |  1.556 μs |  1.299 μs |   289.15 μs |
|            GetWma |   744.70 μs |  2.482 μs |  2.072 μs |   743.91 μs |
|         GetZigZag |   148.77 μs |  0.753 μs |  0.588 μs |   148.89 μs |

## history functions (mostly internal)

|         Method |     Mean |    Error |   StdDev |
|--------------- |---------:|---------:|---------:|
|           Sort | 37.62 μs | 0.189 μs | 0.158 μs |
|       Validate | 39.76 μs | 0.259 μs | 0.242 μs |
| ConvertToBasic | 43.84 μs | 0.457 μs | 0.428 μs |

## math functions (internal)

| Method | Periods |        Mean |    Error |   StdDev |
|------- |-------- |------------:|---------:|---------:|
| StdDev |      20 |    37.87 ns | 0.437 ns | 0.387 ns |
| StdDev |      50 |    96.39 ns | 0.648 ns | 0.606 ns |
| StdDev |     250 |   536.20 ns | 6.724 ns | 6.290 ns |
| StdDev |    1000 | 2,143.34 ns | 6.789 ns | 5.669 ns |
