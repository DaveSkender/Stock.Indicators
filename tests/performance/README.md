# Performance benchmarks for v1.14.0

These are the execution times for the current indicators using two years of historical daily stock quotes (502 periods) with default or typical parameters.

``` bash
BenchmarkDotNet=v0.13.0, OS=Windows 10.0.19043.1110 (21H1/May2021Update)
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK=5.0.302
  [Host]     : .NET 5.0.8 (5.0.821.31504), X64 RyuJIT
  DefaultJob : .NET 5.0.8 (5.0.821.31504), X64 RyuJIT
```

## indicators

|             Method |        Mean |     Error |    StdDev |
|------------------- |------------:|----------:|----------:|
|             GetAdl |   143.94 μs |  0.500 μs |  0.417 μs |
|      GetAdlWithSma |   380.24 μs |  1.982 μs |  1.757 μs |
|             GetAdx |   750.81 μs |  4.000 μs |  3.546 μs |
|       GetAlligator |   235.81 μs |  1.900 μs |  1.778 μs |
|            GetAlma |   214.68 μs |  0.908 μs |  0.758 μs |
|           GetAroon |   350.25 μs |  2.118 μs |  1.877 μs |
|             GetAtr |   159.39 μs |  0.319 μs |  0.249 μs |
|         GetAwesome |   328.41 μs |  0.769 μs |  0.642 μs |
|            GetBeta |   962.03 μs |  5.898 μs |  5.228 μs |
|  GetBollingerBands |   454.37 μs |  2.971 μs |  2.634 μs |
|             GetBop |   282.41 μs |  1.202 μs |  1.066 μs |
|             GetCci |   841.90 μs |  2.642 μs |  2.342 μs |
|      GetChaikinOsc |   267.75 μs |  0.938 μs |  0.783 μs |
|      GetChandelier |   357.70 μs |  1.257 μs |  1.050 μs |
|            GetChop |   335.21 μs |  7.914 μs | 22.706 μs |
|             GetCmf |   669.17 μs |  4.940 μs |  3.857 μs |
|      GetConnorsRsi | 1,217.74 μs | 10.196 μs |  9.039 μs |
|     GetCorrelation |   879.93 μs |  9.628 μs | 14.990 μs |
|        GetDonchian |   338.10 μs |  1.241 μs |  0.969 μs |
|       GetDoubleEma |   181.31 μs |  0.978 μs |  0.867 μs |
|        GetElderRay |   165.60 μs |  0.492 μs |  0.411 μs |
|             GetEma |   100.18 μs |  0.728 μs |  0.608 μs |
|            GetEpma | 1,373.85 μs |  3.511 μs |  3.113 μs |
|             GetFcb |   396.01 μs |  2.954 μs |  2.619 μs |
| GetFisherTransform |   278.79 μs |  1.040 μs |  0.868 μs |
|      GetForceIndex |   128.30 μs |  0.373 μs |  0.331 μs |
|         GetFractal |   106.81 μs |  0.931 μs |  0.871 μs |
|           GetGator |   287.06 μs |  1.198 μs |  1.001 μs |
|      GetHeikinAshi |   186.53 μs |  1.272 μs |  1.190 μs |
|             GetHma | 1,376.77 μs |  3.739 μs |  2.919 μs |
|     GetHtTrendline |   173.79 μs |  1.396 μs |  1.238 μs |
|        GetIchimoku |   977.56 μs |  5.653 μs |  5.288 μs |
|            GetKama |   329.32 μs |  2.116 μs |  1.875 μs |
|         GetKlinger |   497.29 μs |  2.628 μs |  2.195 μs |
|         GetKeltner |   469.75 μs |  3.236 μs |  2.868 μs |
|            GetMacd |   218.43 μs |  1.295 μs |  1.211 μs |
|     GetMaEnvelopes |   152.04 μs |  0.569 μs |  0.475 μs |
|            GetMama |   289.02 μs |  1.238 μs |  1.098 μs |
|             GetMfi |   486.28 μs |  1.702 μs |  1.592 μs |
|             GetObv |    63.14 μs |  0.214 μs |  0.178 μs |
|      GetObvWithSma |   139.55 μs |  0.768 μs |  0.681 μs |
|    GetParabolicSar |    94.78 μs |  0.666 μs |  0.623 μs |
|     GetPivotPoints |    96.77 μs |  1.029 μs |  0.963 μs |
|             GetPmo |   264.38 μs |  0.503 μs |  0.393 μs |
|             GetPrs |   133.24 μs |  1.900 μs |  1.586 μs |
|      GetPrsWithSma |   205.19 μs |  1.452 μs |  1.287 μs |
|             GetPvo |   342.31 μs |  2.006 μs |  1.778 μs |
|           GetRenko |    95.32 μs |  0.797 μs |  0.707 μs |
|        GetRenkoAtr |   103.30 μs |  0.361 μs |  0.301 μs |
|             GetRoc |    96.19 μs |  0.446 μs |  0.373 μs |
|           GetRocWb |   200.18 μs |  1.256 μs |  1.113 μs |
|      GetRocWithSma |   360.51 μs |  1.064 μs |  0.831 μs |
|             GetRsi |   340.59 μs |  1.187 μs |  0.991 μs |
|           GetSlope |   899.18 μs |  5.010 μs |  4.441 μs |
|             GetSma |   106.71 μs |  0.248 μs |  0.207 μs |
|     GetSmaExtended |   945.83 μs |  4.934 μs |  4.615 μs |
|            GetSmma |    98.14 μs |  0.619 μs |  0.579 μs |
|      GetStarcBands |   425.59 μs |  2.025 μs |  1.691 μs |
|          GetStdDev |   291.52 μs |  1.170 μs |  0.977 μs |
|   GetStdDevWithSma |   379.61 μs |  3.100 μs |  2.748 μs |
|  GetStdDevChannels |   948.98 μs |  2.475 μs |  2.067 μs |
|           GetStoch |   399.00 μs |  1.155 μs |  0.964 μs |
|        GetStochRsi |   704.34 μs |  1.421 μs |  1.186 μs |
|      GetSuperTrend |   305.46 μs |  0.732 μs |  0.611 μs |
|       GetTripleEma |   262.27 μs |  0.935 μs |  0.781 μs |
|            GetTrix |   320.13 μs |  1.061 μs |  0.886 μs |
|     GetTrixWithSma |   377.03 μs |  1.789 μs |  1.494 μs |
|             GetTsi |   370.99 μs |  1.126 μs |  0.879 μs |
|              GetT3 |   466.95 μs |  2.113 μs |  1.873 μs |
|      GetUlcerIndex | 1,510.79 μs |  3.978 μs |  3.322 μs |
|        GetUltimate |   552.20 μs |  1.155 μs |  0.964 μs |
|          GetVolSma |   121.47 μs |  0.676 μs |  0.599 μs |
|          GetVortex |   286.72 μs |  1.142 μs |  1.012 μs |
|            GetVwap |    99.28 μs |  0.653 μs |  0.510 μs |
|       GetWilliamsR |   289.59 μs |  1.653 μs |  1.465 μs |
|             GetWma |   736.31 μs |  1.567 μs |  1.389 μs |
|          GetZigZag |   145.62 μs |  0.431 μs |  0.360 μs |

## quotes functions (mostly internal)

|         Method |     Mean |    Error |   StdDev |
|--------------- |---------:|---------:|---------:|
|           Sort | 37.62 μs | 0.189 μs | 0.158 μs |
|       Validate | 39.76 μs | 0.259 μs | 0.242 μs |
| ConvertToBasic | 43.84 μs | 0.457 μs | 0.428 μs |

## math functions (internal)

| Method | Periods |        Mean |    Error |   StdDev |
|------- |-------- |------------:|---------:|---------:|
| StdDev |      20 |    38.30 ns | 0.346 ns | 0.289 ns |
| StdDev |      50 |    96.44 ns | 0.505 ns | 0.448 ns |
| StdDev |     250 |   535.44 ns | 5.344 ns | 4.999 ns |
| StdDev |    1000 | 2,145.99 ns | 4.168 ns | 3.254 ns |
