# Performance benchmarks for v1.4.3

These are the execution times for the current indicators using two years of historical daily stock quotes (502 periods) with default or typical parameters.

``` bash
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19042
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=5.0.101
  [Host]     : .NET Core 5.0.1 (CoreCLR 5.0.120.57516, CoreFX 5.0.120.57516), X64 RyuJIT
  DefaultJob : .NET Core 5.0.1 (CoreCLR 5.0.120.57516, CoreFX 5.0.120.57516), X64 RyuJIT
```

## indicators

|                   Method |        Mean |     Error |    StdDev |      Median |
|------------------------- |------------:|----------:|----------:|------------:|
|                   GetAdl |   145.34 μs |  1.085 μs |  0.962 μs |   145.23 μs |
|            GetAdlWithSma |   382.39 μs |  4.474 μs |  3.966 μs |   380.34 μs |
|                   GetAdx |   751.50 μs |  8.546 μs |  7.575 μs |   751.45 μs |
|                  GetAlma |   219.54 μs |  1.205 μs |  0.941 μs |   219.44 μs |
|                 GetAroon |   354.22 μs |  4.216 μs |  4.686 μs |   354.11 μs |
|                   GetAtr |   160.05 μs |  1.042 μs |  0.870 μs |   159.88 μs |
|                  GetBeta |   973.89 μs |  6.855 μs |  6.077 μs |   972.45 μs |
|        GetBollingerBands |   458.54 μs |  3.043 μs |  2.541 μs |   458.06 μs |
|                   GetCci |   851.10 μs | 12.751 μs | 11.303 μs |   845.06 μs |
|            GetChaikinOsc |   276.31 μs |  5.220 μs |  6.411 μs |   275.34 μs |
|            GetChandelier |   362.32 μs |  6.153 μs |  5.455 μs |   360.57 μs |
|                   GetCmf |   662.65 μs |  2.575 μs |  2.011 μs |   662.45 μs |
|            GetConnorsRsi | 1,188.69 μs |  4.364 μs |  3.407 μs | 1,188.58 μs |
|           GetCorrelation |   907.85 μs | 10.719 μs |  9.502 μs |   906.34 μs |
|              GetDonchian |   340.64 μs |  3.759 μs |  3.516 μs |   339.39 μs |
|             GetDoubleEma |   185.30 μs |  1.869 μs |  1.748 μs |   185.09 μs |
|                   GetEma |   100.89 μs |  1.160 μs |  0.968 μs |   100.52 μs |
|               GetFractal |    72.54 μs |  1.447 μs |  2.956 μs |    71.06 μs |
|            GetHeikinAshi |   181.92 μs |  3.604 μs |  9.683 μs |   178.01 μs |
|                   GetHma | 1,372.59 μs | 19.611 μs | 18.344 μs | 1,364.86 μs |
|              GetIchimoku | 1,006.12 μs |  2.826 μs |  2.360 μs | 1,005.65 μs |
|                  GetKama |   325.29 μs |  3.071 μs |  2.872 μs |   323.57 μs |
|               GetKeltner |   473.08 μs |  5.414 μs |  5.064 μs |   470.52 μs |
|                  GetMacd |   302.85 μs |  1.325 μs |  1.107 μs |   302.44 μs |
|                  GetMama |   286.94 μs |  0.894 μs |  0.747 μs |   286.97 μs |
|                   GetMfi |   488.15 μs |  2.177 μs |  1.930 μs |   488.17 μs |
|                   GetObv |    61.60 μs |  0.252 μs |  0.223 μs |    61.55 μs |
|            GetObvWithSma |   136.91 μs |  0.481 μs |  0.450 μs |   137.08 μs |
|          GetParabolicSar |    94.02 μs |  0.369 μs |  0.327 μs |    93.92 μs |
|           GetPivotPoints |   104.71 μs |  3.314 μs |  9.562 μs |    99.17 μs |
|                   GetPmo |   264.77 μs |  0.984 μs |  0.822 μs |   264.58 μs |
|                   GetPrs |   133.00 μs |  1.306 μs |  1.222 μs |   133.38 μs |
|            GetPrsWithSma |   205.64 μs |  1.980 μs |  1.852 μs |   204.60 μs |
|                   GetRoc |    97.49 μs |  0.967 μs |  0.857 μs |    97.17 μs |
|            GetRocWithSma |   356.66 μs |  3.232 μs |  2.865 μs |   355.30 μs |
|                   GetRsi |   345.28 μs |  4.420 μs |  4.134 μs |   343.68 μs |
|                 GetSlope |   883.97 μs |  4.062 μs |  3.800 μs |   882.74 μs |
|                   GetSma |   111.64 μs |  0.970 μs |  0.907 μs |   111.26 μs |
|           GetSmaExtended |   907.84 μs |  7.643 μs |  6.382 μs |   905.56 μs |
|                GetStdDev |   297.89 μs |  1.849 μs |  1.639 μs |   297.25 μs |
|         GetStdDevWithSma |   380.06 μs |  2.763 μs |  2.585 μs |   379.02 μs |
|                 GetStoch |   381.15 μs |  4.609 μs |  4.086 μs |   380.71 μs |
|              GetStochRsi |   674.39 μs |  1.770 μs |  1.382 μs |   674.06 μs |
|            GetSuperTrend |   302.77 μs |  0.795 μs |  0.664 μs |   302.83 μs |
|             GetTripleEma |   264.63 μs |  1.741 μs |  1.454 μs |   263.85 μs |
|                  GetTrix |   329.21 μs |  5.810 μs |  8.332 μs |   325.24 μs |
|           GetTrixWithSma |   380.27 μs |  2.427 μs |  2.270 μs |   379.61 μs |
|            GetUlcerIndex | 1,535.04 μs | 16.767 μs | 14.864 μs | 1,527.89 μs |
|              GetUltimate |   557.05 μs |  4.772 μs |  4.464 μs |   555.09 μs |
|                GetVolSma |   122.15 μs |  0.978 μs |  0.867 μs |   122.23 μs |
|             GetWilliamsR |   292.12 μs |  1.125 μs |  0.879 μs |   291.86 μs |
|                   GetWma |   746.80 μs |  9.188 μs |  8.145 μs |   743.65 μs |
|                GetZigZag |   148.56 μs |  0.734 μs |  0.650 μs |   148.46 μs |

## general functions (internal)

|             Method |     Mean |    Error |   StdDev |   Median |
|------------------- |---------:|---------:|---------:|---------:|
|        SortHistory | 37.66 μs | 0.148 μs | 0.124 μs | 37.65 μs |
|    ValidateHistory | 40.28 μs | 0.439 μs | 0.367 μs | 40.15 μs |
| ConvertToBasicData | 45.66 μs | 0.867 μs | 2.224 μs | 44.82 μs |

## math functions (internal)

| Method | Periods |        Mean |     Error |   StdDev |
|------- |-------- |------------:|----------:|---------:|
| StdDev |      20 |    37.45 ns |  0.291 ns | 0.258 ns |
| StdDev |      50 |    98.09 ns |  0.831 ns | 0.778 ns |
| StdDev |     250 |   534.89 ns |  1.562 ns | 1.220 ns |
| StdDev |    1000 | 2,167.24 ns | 10.419 ns | 9.236 ns |
