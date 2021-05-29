# Performance benchmarks for v1.11.1

These are the execution times for the current indicators using two years of historical daily stock quotes (502 periods) with default or typical parameters.

``` bash
BenchmarkDotNet=v0.13.0, OS=Windows 10.0.19042.985 (20H2/October2020Update)
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK=5.0.203
  [Host]     : .NET 5.0.6 (5.0.621.22011), X64 RyuJIT
  DefaultJob : .NET 5.0.6 (5.0.621.22011), X64 RyuJIT
```

## indicators

|             Method |        Mean |    Error |   StdDev |
|------------------- |------------:|---------:|---------:|
|             GetAdl |   144.93 μs | 0.426 μs | 0.356 μs |
|      GetAdlWithSma |   379.24 μs | 0.999 μs | 0.935 μs |
|             GetAdx |   752.20 μs | 2.060 μs | 1.927 μs |
|       GetAlligator |   235.47 μs | 0.843 μs | 0.789 μs |
|            GetAlma |   217.43 μs | 0.538 μs | 0.503 μs |
|           GetAroon |   337.66 μs | 0.732 μs | 0.684 μs |
|             GetAtr |   159.75 μs | 0.731 μs | 0.648 μs |
|         GetAwesome |   331.46 μs | 1.751 μs | 1.367 μs |
|            GetBeta |   979.92 μs | 3.162 μs | 2.803 μs |
|  GetBollingerBands |   462.40 μs | 2.117 μs | 1.877 μs |
|             GetBop |   283.49 μs | 2.585 μs | 2.158 μs |
|             GetCci |   845.06 μs | 1.262 μs | 1.119 μs |
|      GetChaikinOsc |   266.55 μs | 0.563 μs | 0.499 μs |
|      GetChandelier |   355.75 μs | 1.098 μs | 0.974 μs |
|            GetChop |   306.13 μs | 1.367 μs | 1.212 μs |
|             GetCmf |   666.08 μs | 1.856 μs | 1.550 μs |
|      GetConnorsRsi | 1,181.32 μs | 3.130 μs | 2.614 μs |
|     GetCorrelation |   873.22 μs | 1.914 μs | 1.697 μs |
|        GetDonchian |   345.41 μs | 0.955 μs | 0.798 μs |
|       GetDoubleEma |   184.03 μs | 0.930 μs | 0.824 μs |
|        GetElderRay |   166.58 μs | 0.700 μs | 0.655 μs |
|             GetEma |    99.82 μs | 0.296 μs | 0.276 μs |
|            GetEpma | 1,388.29 μs | 3.948 μs | 3.500 μs |
|             GetFcb |   392.89 μs | 1.242 μs | 1.162 μs |
| GetFisherTransform |   283.96 μs | 1.102 μs | 0.977 μs |
|      GetForceIndex |   128.03 μs | 0.580 μs | 0.514 μs |
|         GetFractal |   108.10 μs | 0.624 μs | 0.584 μs |
|           GetGator |   288.83 μs | 0.830 μs | 0.735 μs |
|      GetHeikinAshi |   176.00 μs | 0.486 μs | 0.431 μs |
|             GetHma | 1,368.52 μs | 4.075 μs | 3.403 μs |
|     GetHtTrendline |   175.90 μs | 1.924 μs | 1.799 μs |
|        GetIchimoku | 1,003.14 μs | 2.958 μs | 2.622 μs |
|            GetKama |   326.21 μs | 0.830 μs | 0.777 μs |
|         GetKeltner |   468.85 μs | 0.792 μs | 0.702 μs |
|            GetMacd |   216.78 μs | 0.625 μs | 0.522 μs |
|     GetMaEnvelopes |   151.92 μs | 0.416 μs | 0.347 μs |
|            GetMama |   286.52 μs | 0.627 μs | 0.555 μs |
|             GetMfi |   484.78 μs | 0.973 μs | 0.910 μs |
|             GetObv |    62.79 μs | 0.218 μs | 0.204 μs |
|      GetObvWithSma |   138.33 μs | 0.326 μs | 0.289 μs |
|    GetParabolicSar |    93.76 μs | 0.325 μs | 0.304 μs |
|     GetPivotPoints |    97.71 μs | 0.351 μs | 0.311 μs |
|             GetPmo |   261.43 μs | 0.672 μs | 0.596 μs |
|             GetPrs |   132.97 μs | 0.573 μs | 0.508 μs |
|      GetPrsWithSma |   204.46 μs | 0.308 μs | 0.273 μs |
|             GetPvo |   341.19 μs | 0.906 μs | 0.804 μs |
|             GetRoc |    96.64 μs | 0.401 μs | 0.375 μs |
|           GetRocWb |   201.06 μs | 0.341 μs | 0.302 μs |
|      GetRocWithSma |   353.39 μs | 0.411 μs | 0.364 μs |
|             GetRsi |   340.56 μs | 0.782 μs | 0.653 μs |
|           GetSlope |   880.20 μs | 0.799 μs | 0.668 μs |
|             GetSma |   105.35 μs | 0.244 μs | 0.190 μs |
|     GetSmaExtended |   945.91 μs | 1.511 μs | 1.261 μs |
|            GetSmma |    97.21 μs | 0.467 μs | 0.437 μs |
|      GetStarcBands |   420.69 μs | 0.779 μs | 0.728 μs |
|          GetStdDev |   295.64 μs | 0.688 μs | 0.538 μs |
|   GetStdDevWithSma |   436.54 μs | 4.411 μs | 3.911 μs |
|  GetStdDevChannels |   945.04 μs | 1.729 μs | 1.444 μs |
|           GetStoch |   403.55 μs | 1.305 μs | 1.221 μs |
|        GetStochRsi |   704.54 μs | 1.785 μs | 1.670 μs |
|      GetSuperTrend |   303.18 μs | 0.804 μs | 0.752 μs |
|       GetTripleEma |   263.62 μs | 1.432 μs | 1.339 μs |
|            GetTrix |   325.12 μs | 2.111 μs | 1.871 μs |
|     GetTrixWithSma |   378.41 μs | 0.966 μs | 0.903 μs |
|             GetTsi |   369.60 μs | 0.457 μs | 0.357 μs |
|              GetT3 |   465.37 μs | 0.731 μs | 0.648 μs |
|      GetUlcerIndex | 1,531.77 μs | 1.826 μs | 1.618 μs |
|        GetUltimate |   556.71 μs | 1.488 μs | 1.243 μs |
|          GetVolSma |   121.32 μs | 0.487 μs | 0.456 μs |
|          GetVortex |   279.29 μs | 0.519 μs | 0.405 μs |
|            GetVwap |    98.09 μs | 0.297 μs | 0.248 μs |
|       GetWilliamsR |   291.26 μs | 1.229 μs | 1.149 μs |
|             GetWma |   735.78 μs | 1.319 μs | 1.101 μs |
|          GetZigZag |   148.00 μs | 0.369 μs | 0.345 μs |

## history functions (mostly internal)

|         Method |     Mean |    Error |   StdDev |
|--------------- |---------:|---------:|---------:|
|           Sort | 37.62 μs | 0.189 μs | 0.158 μs |
|       Validate | 39.76 μs | 0.259 μs | 0.242 μs |
| ConvertToBasic | 43.84 μs | 0.457 μs | 0.428 μs |

## math functions (internal)

| Method | Periods |        Mean |    Error |   StdDev |
|------- |-------- |------------:|---------:|---------:|
| StdDev |      20 |    36.89 ns | 0.109 ns | 0.091 ns |
| StdDev |      50 |    95.57 ns | 0.269 ns | 0.224 ns |
| StdDev |     250 |   530.10 ns | 1.183 ns | 0.988 ns |
| StdDev |    1000 | 2,142.99 ns | 4.975 ns | 4.653 ns |
