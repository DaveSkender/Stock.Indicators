# Performance benchmarks for v1.13.0

These are the execution times for the current indicators using two years of historical daily stock quotes (502 periods) with default or typical parameters.

``` bash
BenchmarkDotNet=v0.13.0, OS=Windows 10.0.19043.1055 (21H1/May2021Update)
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK=5.0.301
  [Host]     : .NET 5.0.7 (5.0.721.25508), X64 RyuJIT
  DefaultJob : .NET 5.0.7 (5.0.721.25508), X64 RyuJIT
```

## indicators

|             Method |        Mean |     Error |    StdDev |
|------------------- |------------:|----------:|----------:|
|             GetAdl |   143.99 μs |  0.385 μs |  0.322 μs |
|      GetAdlWithSma |   378.45 μs |  1.024 μs |  0.958 μs |
|             GetAdx |   752.21 μs |  1.662 μs |  1.554 μs |
|       GetAlligator |   234.75 μs |  0.360 μs |  0.300 μs |
|            GetAlma |   219.23 μs |  2.133 μs |  1.665 μs |
|           GetAroon |   348.09 μs |  1.176 μs |  0.982 μs |
|             GetAtr |   159.34 μs |  0.266 μs |  0.236 μs |
|         GetAwesome |   329.91 μs |  1.614 μs |  1.431 μs |
|            GetBeta |   971.69 μs |  2.306 μs |  2.045 μs |
|  GetBollingerBands |   457.50 μs |  0.854 μs |  0.757 μs |
|             GetBop |   278.49 μs |  0.716 μs |  0.598 μs |
|             GetCci |   846.06 μs |  2.232 μs |  2.088 μs |
|      GetChaikinOsc |   268.37 μs |  0.774 μs |  0.724 μs |
|      GetChandelier |   361.67 μs |  0.939 μs |  0.833 μs |
|            GetChop |   305.60 μs |  0.906 μs |  0.847 μs |
|             GetCmf |   666.58 μs |  0.739 μs |  0.617 μs |
|      GetConnorsRsi | 1,175.92 μs |  4.322 μs |  4.043 μs |
|     GetCorrelation |   879.02 μs |  1.301 μs |  1.086 μs |
|        GetDonchian |   341.23 μs |  0.745 μs |  0.622 μs |
|       GetDoubleEma |   182.35 μs |  0.515 μs |  0.482 μs |
|        GetElderRay |   167.89 μs |  0.524 μs |  0.465 μs |
|             GetEma |    98.38 μs |  0.190 μs |  0.158 μs |
|            GetEpma | 1,377.38 μs |  3.667 μs |  3.062 μs |
|             GetFcb |   384.00 μs |  0.875 μs |  0.730 μs |
| GetFisherTransform |   285.59 μs |  0.974 μs |  0.911 μs |
|      GetForceIndex |   127.83 μs |  0.273 μs |  0.228 μs |
|         GetFractal |   105.90 μs |  0.237 μs |  0.210 μs |
|           GetGator |   285.49 μs |  0.722 μs |  0.675 μs |
|      GetHeikinAshi |   173.11 μs |  0.442 μs |  0.413 μs |
|             GetHma | 1,364.67 μs |  2.549 μs |  2.259 μs |
|     GetHtTrendline |   173.95 μs |  0.424 μs |  0.376 μs |
|        GetIchimoku |   990.62 μs |  3.245 μs |  2.877 μs |
|            GetKama |   328.36 μs |  0.513 μs |  0.401 μs |
|         GetKlinger |   495.51 μs |  1.240 μs |  1.099 μs |
|         GetKeltner |   479.02 μs |  0.630 μs |  0.526 μs |
|            GetMacd |   216.36 μs |  0.402 μs |  0.376 μs |
|     GetMaEnvelopes |   151.98 μs |  0.170 μs |  0.142 μs |
|            GetMama |   287.67 μs |  0.852 μs |  0.665 μs |
|             GetMfi |   486.01 μs |  1.798 μs |  1.681 μs |
|             GetObv |    62.47 μs |  0.394 μs |  0.369 μs |
|      GetObvWithSma |   138.48 μs |  0.961 μs |  0.944 μs |
|    GetParabolicSar |    94.35 μs |  0.359 μs |  0.318 μs |
|     GetPivotPoints |    95.21 μs |  0.335 μs |  0.313 μs |
|             GetPmo |   260.83 μs |  0.752 μs |  0.587 μs |
|             GetPrs |   132.90 μs |  0.854 μs |  0.798 μs |
|      GetPrsWithSma |   206.29 μs |  0.505 μs |  0.472 μs |
|             GetPvo |   341.99 μs |  4.889 μs |  8.563 μs |
|             GetRoc |    96.66 μs |  1.051 μs |  0.878 μs |
|           GetRocWb |   201.52 μs |  0.394 μs |  0.308 μs |
|      GetRocWithSma |   355.29 μs |  1.056 μs |  0.824 μs |
|             GetRsi |   342.66 μs |  1.079 μs |  0.956 μs |
|           GetSlope |   897.78 μs |  5.472 μs |  4.851 μs |
|             GetSma |   107.45 μs |  0.751 μs |  0.627 μs |
|     GetSmaExtended |   947.36 μs |  4.993 μs |  4.426 μs |
|            GetSmma |    98.28 μs |  0.514 μs |  0.455 μs |
|      GetStarcBands |   418.86 μs |  1.821 μs |  1.703 μs |
|          GetStdDev |   291.32 μs |  1.029 μs |  0.913 μs |
|   GetStdDevWithSma |   381.76 μs |  2.349 μs |  2.198 μs |
|  GetStdDevChannels |   988.70 μs |  3.511 μs |  2.932 μs |
|           GetStoch |   402.00 μs |  1.573 μs |  1.394 μs |
|        GetStochRsi |   709.01 μs |  5.271 μs |  4.673 μs |
|      GetSuperTrend |   304.45 μs |  1.213 μs |  1.075 μs |
|       GetTripleEma |   262.92 μs |  1.150 μs |  0.960 μs |
|            GetTrix |   322.77 μs |  0.740 μs |  0.618 μs |
|     GetTrixWithSma |   378.55 μs |  0.654 μs |  0.510 μs |
|             GetTsi |   368.83 μs |  0.921 μs |  0.769 μs |
|              GetT3 |   465.86 μs |  0.936 μs |  0.781 μs |
|      GetUlcerIndex | 1,508.19 μs |  5.792 μs |  5.134 μs |
|        GetUltimate |   557.18 μs |  3.958 μs |  3.702 μs |
|          GetVolSma |   122.34 μs |  0.649 μs |  0.542 μs |
|          GetVortex |   284.85 μs |  1.313 μs |  1.097 μs |
|            GetVwap |    99.16 μs |  0.344 μs |  0.305 μs |
|       GetWilliamsR |   284.47 μs |  1.061 μs |  0.828 μs |
|             GetWma |   749.35 μs | 13.237 μs | 16.741 μs |
|          GetZigZag |   146.51 μs |  0.453 μs |  0.378 μs |

## history functions (mostly internal)

|         Method |     Mean |    Error |   StdDev |
|--------------- |---------:|---------:|---------:|
|           Sort | 37.62 μs | 0.189 μs | 0.158 μs |
|       Validate | 39.76 μs | 0.259 μs | 0.242 μs |
| ConvertToBasic | 43.84 μs | 0.457 μs | 0.428 μs |

## math functions (internal)

| Method | Periods |        Mean |    Error |   StdDev |
|------- |-------- |------------:|---------:|---------:|
| StdDev |      20 |    36.59 ns | 0.139 ns | 0.130 ns |
| StdDev |      50 |    95.29 ns | 0.192 ns | 0.170 ns |
| StdDev |     250 |   530.01 ns | 1.333 ns | 1.181 ns |
| StdDev |    1000 | 2,142.70 ns | 4.781 ns | 4.239 ns |
