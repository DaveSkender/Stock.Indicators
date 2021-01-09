# Performance benchmarks for v1.6.1

These are the execution times for the current indicators using two years of historical daily stock quotes (502 periods) with default or typical parameters.

``` bash
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19042
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=5.0.101
  [Host]     : .NET Core 5.0.1 (CoreCLR 5.0.120.57516, CoreFX 5.0.120.57516), X64 RyuJIT
  DefaultJob : .NET Core 5.0.1 (CoreCLR 5.0.120.57516, CoreFX 5.0.120.57516), X64 RyuJIT
```

## indicators

|            Method |        Mean |     Error |    StdDev |      Median |
|------------------ |------------:|----------:|----------:|------------:|
|            GetAdl |   146.28 μs |  2.480 μs |  1.936 μs |   145.65 μs |
|     GetAdlWithSma |   381.18 μs |  1.441 μs |  1.278 μs |   381.53 μs |
|            GetAdx |   744.01 μs |  3.101 μs |  2.749 μs |   742.98 μs |
|           GetAlma |   219.16 μs |  1.581 μs |  1.320 μs |   218.84 μs |
|          GetAroon |   347.06 μs |  1.232 μs |  1.028 μs |   346.69 μs |
|            GetAtr |   159.69 μs |  0.852 μs |  0.712 μs |   159.47 μs |
|        GetAwesome |   330.39 μs |  2.932 μs |  2.448 μs |   329.31 μs |
|           GetBeta |   973.89 μs |  6.855 μs |  6.077 μs |   972.45 μs |
| GetBollingerBands |   464.35 μs |  3.993 μs |  3.335 μs |   463.50 μs |
|            GetBop |   280.01 μs |  1.952 μs |  1.730 μs |   279.59 μs |
|            GetCci |   851.10 μs | 12.751 μs | 11.303 μs |   845.06 μs |
|     GetChaikinOsc |   271.94 μs |  3.575 μs |  3.169 μs |   270.80 μs |
|     GetChandelier |   367.06 μs |  2.543 μs |  2.379 μs |   366.87 μs |
|            GetCmf |   667.44 μs |  4.139 μs |  3.871 μs |   667.09 μs |
|     GetConnorsRsi | 1,187.86 μs |  3.134 μs |  2.617 μs | 1,187.95 μs |
|    GetCorrelation |   880.92 μs |  4.984 μs |  4.418 μs |   879.16 μs |
|       GetDonchian |   346.57 μs |  1.404 μs |  1.313 μs |   346.30 μs |
|      GetDoubleEma |   185.97 μs |  3.248 μs |  6.021 μs |   183.22 μs |
|            GetEma |   101.39 μs |  0.737 μs |  0.616 μs |   101.30 μs |
|        GetFractal |    73.19 μs |  0.779 μs |  0.728 μs |    73.21 μs |
|     GetHeikinAshi |   176.53 μs |  1.526 μs |  1.353 μs |   175.83 μs |
|            GetHma | 1,383.42 μs | 10.139 μs |  9.484 μs | 1,378.12 μs |
|       GetIchimoku |   986.09 μs |  7.399 μs |  6.921 μs |   982.20 μs |
|           GetKama |   326.86 μs |  2.038 μs |  1.906 μs |   325.90 μs |
|        GetKeltner |   468.76 μs |  2.032 μs |  1.697 μs |   468.16 μs |
|           GetMacd |   218.70 μs |  1.395 μs |  1.237 μs |   218.03 μs |
|    GetMaEnvelopes |   152.46 μs |  0.755 μs |  0.669 μs |   152.55 μs |
|           GetMama |   287.74 μs |  0.929 μs |  0.775 μs |   287.60 μs |
|            GetMfi |   489.89 μs |  1.358 μs |  1.204 μs |   490.04 μs |
|            GetObv |    63.15 μs |  0.514 μs |  0.481 μs |    63.19 μs |
|     GetObvWithSma |   138.62 μs |  0.959 μs |  0.801 μs |   138.67 μs |
|   GetParabolicSar |    94.73 μs |  0.483 μs |  0.428 μs |    94.62 μs |
|    GetPivotPoints |    99.50 μs |  1.636 μs |  3.379 μs |    98.20 μs |
|            GetPmo |   270.48 μs |  1.187 μs |  1.052 μs |   270.30 μs |
|            GetPrs |   133.94 μs |  0.724 μs |  0.642 μs |   133.86 μs |
|     GetPrsWithSma |   211.04 μs |  0.959 μs |  0.850 μs |   210.82 μs |
|            GetPvo |   344.97 μs |  2.116 μs |  1.979 μs |   344.13 μs |
|            GetRoc |    98.03 μs |  0.606 μs |  0.537 μs |    98.00 μs |
|     GetRocWithSma |   359.67 μs |  1.982 μs |  1.854 μs |   360.04 μs |
|            GetRsi |   345.55 μs |  1.925 μs |  1.706 μs |   345.18 μs |
|          GetSlope |   901.22 μs |  7.007 μs |  5.851 μs |   899.42 μs |
|            GetSma |   112.81 μs |  0.551 μs |  0.489 μs |   112.80 μs |
|    GetSmaExtended |   898.33 μs |  1.673 μs |  1.306 μs |   898.28 μs |
|         GetStdDev |   293.22 μs |  1.291 μs |  1.078 μs |   293.08 μs |
|     GetStarcBands |   431.60 μs |  2.227 μs |  1.974 μs |   430.42 μs |
|  GetStdDevWithSma |   385.41 μs |  2.127 μs |  1.885 μs |   385.14 μs |
|          GetStoch |   381.68 μs |  3.497 μs |  3.100 μs |   380.35 μs |
|       GetStochRsi |   683.39 μs |  3.848 μs |  3.412 μs |   681.70 μs |
|     GetSuperTrend |   306.21 μs |  1.391 μs |  1.161 μs |   305.90 μs |
|      GetTripleEma |   267.65 μs |  2.918 μs |  2.587 μs |   266.51 μs |
|           GetTrix |   323.65 μs |  1.162 μs |  0.971 μs |   323.70 μs |
|    GetTrixWithSma |   383.10 μs |  3.688 μs |  3.080 μs |   382.00 μs |
|            GetTsi |   376.00 μs |  3.302 μs |  3.089 μs |   374.48 μs |
|     GetUlcerIndex | 1,542.76 μs |  4.406 μs |  3.906 μs | 1,541.87 μs |
|       GetUltimate |   565.85 μs |  8.967 μs | 14.734 μs |   560.55 μs |
|         GetVolSma |   119.74 μs |  0.526 μs |  0.467 μs |   119.70 μs |
|           GetVwap |    98.73 μs |  0.289 μs |  0.241 μs |    98.68 μs |
|      GetWilliamsR |   292.99 μs |  0.825 μs |  0.644 μs |   293.03 μs |
|            GetWma |   752.19 μs |  4.519 μs |  4.006 μs |   751.02 μs |
|         GetZigZag |   146.94 μs |  0.555 μs |  0.464 μs |   146.71 μs |

## history functions (mostly internal)

|         Method |     Mean |    Error |   StdDev |
|--------------- |---------:|---------:|---------:|
|           Sort | 37.62 μs | 0.189 μs | 0.158 μs |
|       Validate | 39.76 μs | 0.259 μs | 0.242 μs |
| ConvertToBasic | 43.84 μs | 0.457 μs | 0.428 μs |

## math functions (internal)

| Method | Periods |        Mean |     Error |    StdDev |
|------- |-------- |------------:|----------:|----------:|
| StdDev |      20 |    36.78 ns |  0.641 ns |  0.569 ns |
| StdDev |      50 |    95.98 ns |  0.959 ns |  0.801 ns |
| StdDev |     250 |   531.67 ns |  1.224 ns |  1.022 ns |
| StdDev |    1000 | 2,158.29 ns | 19.727 ns | 17.488 ns |
