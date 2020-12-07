# Performance benchmarks for v1.3.0

These are the execution times for the current indicators using two years of historical daily stock quotes (502 periods) with default or typical parameters.

``` bash
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19042
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=5.0.100
  [Host]     : .NET Core 5.0.0 (CoreCLR 5.0.20.51904, CoreFX 5.0.20.51904), X64 RyuJIT
  DefaultJob : .NET Core 5.0.0 (CoreCLR 5.0.20.51904, CoreFX 5.0.20.51904), X64 RyuJIT
```

## indicators

|            Method |        Mean |     Error |    StdDev |      Median |
|------------------ |------------:|----------:|----------:|------------:|
|            GetAdl |   147.37 μs |  2.835 μs |  3.375 μs |   146.46 μs |
|     GetAdlWithSma |   389.71 μs |  7.464 μs |  6.617 μs |   388.85 μs |
|            GetAdx |   746.59 μs |  5.873 μs |  4.904 μs |   743.81 μs |
|          GetAroon |   360.96 μs |  4.385 μs |  4.101 μs |   358.70 μs |
|            GetAtr |   161.96 μs |  1.701 μs |  1.591 μs |   162.13 μs |
|           GetBeta |   968.08 μs |  5.535 μs |  5.178 μs |   966.15 μs |
| GetBollingerBands |   466.91 μs |  8.937 μs |  8.777 μs |   463.49 μs |
|            GetCci |   853.54 μs |  6.780 μs |  6.342 μs |   850.87 μs |
|     GetChaikinOsc |   269.78 μs |  3.388 μs |  3.169 μs |   268.52 μs |
|     GetChandelier |   364.92 μs |  5.723 μs |  5.353 μs |   363.33 μs |
|            GetCmf |   674.32 μs |  5.351 μs |  5.005 μs |   673.48 μs |
|     GetConnorsRsi | 1,198.88 μs |  3.201 μs |  2.673 μs | 1,198.51 μs |
|    GetCorrelation |   882.43 μs |  6.782 μs |  5.663 μs |   883.75 μs |
|       GetDonchian |   398.05 μs |  8.422 μs | 23.196 μs |   394.36 μs |
|      GetDoubleEma |   186.70 μs |  2.520 μs |  2.902 μs |   185.41 μs |
|            GetEma |   100.86 μs |  1.002 μs |  0.938 μs |   101.00 μs |
|        GetFractal |    72.93 μs |  0.423 μs |  0.353 μs |    72.94 μs |
|     GetHeikinAshi |   175.29 μs |  1.826 μs |  1.708 μs |   175.15 μs |
|            GetHma | 1,375.60 μs | 12.810 μs | 11.356 μs | 1,371.08 μs |
|       GetIchimoku |   989.24 μs |  2.008 μs |  1.676 μs |   988.77 μs |
|        GetKeltner |   474.41 μs |  3.986 μs |  3.729 μs |   473.60 μs |
|           GetMacd |   302.67 μs |  2.318 μs |  2.168 μs |   301.61 μs |
|            GetMfi |   486.84 μs |  3.830 μs |  3.582 μs |   486.94 μs |
|            GetObv |    62.74 μs |  0.142 μs |  0.110 μs |    62.73 μs |
|     GetObvWithSma |   137.32 μs |  1.115 μs |  1.043 μs |   137.12 μs |
|   GetParabolicSar |    95.17 μs |  1.782 μs |  3.168 μs |    93.95 μs |
|            GetPmo |   261.27 μs |  1.196 μs |  0.934 μs |   260.98 μs |
|            GetPrs |   132.97 μs |  0.447 μs |  0.397 μs |   133.07 μs |
|     GetPrsWithSma |   208.16 μs |  2.699 μs |  2.524 μs |   207.28 μs |
|            GetRoc |    97.49 μs |  0.917 μs |  0.858 μs |    97.21 μs |
|     GetRocWithSma |   363.17 μs |  3.777 μs |  3.533 μs |   361.60 μs |
|            GetRsi |   341.76 μs |  2.744 μs |  2.433 μs |   341.52 μs |
|          GetSlope |   889.07 μs |  7.237 μs |  6.416 μs |   886.98 μs |
|            GetSma |   109.54 μs |  0.966 μs |  0.904 μs |   109.59 μs |
|    GetSmaExtended |   903.95 μs |  9.341 μs |  8.281 μs |   899.21 μs |
|         GetStdDev |   295.16 μs |  1.357 μs |  1.203 μs |   294.64 μs |
|  GetStdDevWithSma |   379.00 μs |  1.183 μs |  1.049 μs |   379.09 μs |
|          GetStoch |   371.34 μs |  3.662 μs |  3.425 μs |   369.67 μs |
|       GetStochRsi |   692.28 μs | 13.587 μs | 30.108 μs |   678.54 μs |
|     GetSuperTrend |   300.35 μs |  1.424 μs |  1.189 μs |   299.98 μs |
|      GetTripleEma |   269.44 μs |  5.305 μs | 10.222 μs |   263.51 μs |
|           GetTrix |   320.76 μs |  2.795 μs |  2.478 μs |   319.58 μs |
|    GetTrixWithSma |   383.26 μs |  7.164 μs | 11.153 μs |   376.97 μs |
|     GetUlcerIndex | 1,507.04 μs |  3.684 μs |  3.077 μs | 1,506.10 μs |
|       GetUltimate |   600.45 μs |  3.795 μs |  3.169 μs |   601.38 μs |
|         GetVolSma |   120.02 μs |  0.974 μs |  0.911 μs |   119.89 μs |
|      GetWilliamsR |   285.24 μs |  2.403 μs |  2.248 μs |   284.62 μs |
|            GetWma |   738.75 μs |  6.561 μs |  6.138 μs |   737.18 μs |
|         GetZigZag |   146.58 μs |  0.392 μs |  0.327 μs |   146.63 μs |

## internal cleaners

|             Method |     Mean |    Error |   StdDev |
|------------------- |---------:|---------:|---------:|
|        SortHistory | 38.12 μs | 0.384 μs | 0.320 μs |
|    ValidateHistory | 40.28 μs | 0.624 μs | 1.076 μs |
| ConvertToBasicData | 45.13 μs | 0.741 μs | 0.793 μs |

## internal math functions

| Method | Periods |        Mean |     Error |   StdDev |
|------- |-------- |------------:|----------:|---------:|
| StdDev |      20 |    37.45 ns |  0.291 ns | 0.258 ns |
| StdDev |      50 |    98.09 ns |  0.831 ns | 0.778 ns |
| StdDev |     250 |   534.89 ns |  1.562 ns | 1.220 ns |
| StdDev |    1000 | 2,167.24 ns | 10.419 ns | 9.236 ns |
