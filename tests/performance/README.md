# Performance benchmarks for v1.2.0

These are the execution times for the current indicators using two years of historical daily stock quotes (502 periods) with default or typical parameters.

``` bash
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19042
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=5.0.100
  [Host]     : .NET Core 5.0.0 (CoreCLR 5.0.20.51904, CoreFX 5.0.20.51904), X64 RyuJIT
  DefaultJob : .NET Core 5.0.0 (CoreCLR 5.0.20.51904, CoreFX 5.0.20.51904), X64 RyuJIT
```

## indicators

|             Method |        Mean |     Error |    StdDev |      Median |
|------------------- |------------:|----------:|----------:|------------:|
|             GetAdl |   145.86 μs |  1.167 μs |  0.975 μs |   145.81 μs |
|      GetAdlWithSma |   386.60 μs |  3.934 μs |  3.680 μs |   385.25 μs |
|             GetAdx |   748.81 μs |  5.807 μs |  5.148 μs |   746.60 μs |
|           GetAroon |   348.75 μs |  6.633 μs |  6.204 μs |   346.48 μs |
|             GetAtr |   158.31 μs |  0.361 μs |  0.282 μs |   158.26 μs |
|            GetBeta |   991.38 μs |  7.127 μs |  6.667 μs |   988.38 μs |
|  GetBollingerBands |   470.79 μs |  8.007 μs | 13.155 μs |   464.10 μs |
|             GetCci |   901.68 μs | 14.738 μs | 13.786 μs |   899.12 μs |
|      GetChaikinOsc |   271.32 μs |  3.455 μs |  3.063 μs |   270.74 μs |
|      GetChandelier |   369.23 μs |  4.169 μs |  3.899 μs |   367.52 μs |
|             GetCmf |   676.63 μs |  6.819 μs |  6.378 μs |   676.22 μs |
|      GetConnorsRsi | 1,228.35 μs | 13.353 μs | 11.837 μs | 1,222.89 μs |
|     GetCorrelation |   905.19 μs |  8.713 μs |  8.557 μs |   901.41 μs |
|        GetDonchian |   380.03 μs |  9.039 μs | 24.127 μs |   375.37 μs |
|       GetDoubleEma |   193.67 μs |  3.806 μs |  7.512 μs |   192.17 μs |
|             GetEma |   107.79 μs |  1.051 μs |  0.983 μs |   107.62 μs |
|      GetHeikinAshi |   187.63 μs |  3.678 μs |  5.505 μs |   185.66 μs |
|             GetHma | 1,473.86 μs | 18.819 μs | 15.714 μs | 1,467.52 μs |
|        GetIchimoku | 1,005.75 μs |  1.881 μs |  1.468 μs | 1,006.13 μs |
|         GetKeltner |   475.64 μs |  6.446 μs |  6.619 μs |   473.79 μs |
|            GetMacd |   318.17 μs |  6.602 μs | 18.941 μs |   307.08 μs |
|             GetMfi |   499.35 μs |  9.119 μs | 17.786 μs |   489.52 μs |
|             GetObv |    61.98 μs |  0.419 μs |  0.372 μs |    61.89 μs |
|      GetObvWithSma |   137.18 μs |  0.336 μs |  0.281 μs |   137.24 μs |
|    GetParabolicSar |    95.59 μs |  0.738 μs |  0.616 μs |    95.41 μs |
|             GetPmo |   264.77 μs |  0.737 μs |  0.616 μs |   264.56 μs |
|             GetPrs |   133.95 μs |  0.880 μs |  0.780 μs |   133.74 μs |
|      GetPrsWithSma |   221.04 μs |  4.194 μs |  9.635 μs |   219.82 μs |
|             GetRoc |   101.99 μs |  0.850 μs |  0.710 μs |   101.88 μs |
|      GetRocWithSma |   381.61 μs |  7.324 μs |  9.523 μs |   378.57 μs |
|             GetRsi |   366.58 μs |  6.997 μs |  9.578 μs |   363.40 μs |
|           GetSlope |   907.30 μs | 12.244 μs | 10.225 μs |   903.66 μs |
|             GetSma |   111.67 μs |  1.745 μs |  2.077 μs |   110.69 μs |
|     GetSmaExtended |   909.31 μs | 10.283 μs |  9.115 μs |   905.78 μs |
|          GetStdDev |   294.74 μs |  1.719 μs |  1.435 μs |   294.28 μs |
|   GetStdDevWithSma |   384.92 μs |  3.834 μs |  3.586 μs |   383.55 μs |
|           GetStoch |   381.52 μs |  1.546 μs |  1.291 μs |   381.54 μs |
|        GetStochRsi |   692.48 μs | 10.243 μs |  9.581 μs |   686.51 μs |
|       GetTripleEma |   267.39 μs |  1.703 μs |  1.509 μs |   266.97 μs |
|            GetTrix |   340.33 μs |  6.789 μs |  9.737 μs |   344.31 μs |
|     GetTrixWithSma |   388.53 μs |  3.513 μs |  3.286 μs |   386.77 μs |
|      GetUlcerIndex | 1,551.25 μs | 20.299 μs | 18.988 μs | 1,542.66 μs |
|        GetUltimate |   602.90 μs |  7.759 μs |  6.879 μs |   600.57 μs |
|          GetVolSma |   119.89 μs |  0.693 μs |  0.579 μs |   119.70 μs |
|       GetWilliamsR |   287.91 μs |  0.556 μs |  0.464 μs |   287.93 μs |
|             GetWma |   747.89 μs |  8.326 μs |  7.381 μs |   745.02 μs |
|          GetZigZag |   146.63 μs |  0.746 μs |  0.623 μs |   146.49 μs |

## internal cleaners

|             Method |     Mean |    Error |   StdDev |   Median |
|------------------- |---------:|---------:|---------:|---------:|
|        SortHistory | 37.69 μs | 0.614 μs | 0.544 μs | 37.50 μs |
|    ValidateHistory | 40.63 μs | 0.769 μs | 1.306 μs | 40.04 μs |
| ConvertToBasicData | 44.88 μs | 0.862 μs | 0.806 μs | 44.56 μs |

## internal math functions

| Method | Periods |        Mean |     Error |   StdDev |
|------- |-------- |------------:|----------:|---------:|
| StdDev |      20 |    37.45 ns |  0.291 ns | 0.258 ns |
| StdDev |      50 |    98.09 ns |  0.831 ns | 0.778 ns |
| StdDev |     250 |   534.89 ns |  1.562 ns | 1.220 ns |
| StdDev |    1000 | 2,167.24 ns | 10.419 ns | 9.236 ns |
