# Performance benchmarks

Updated for v1.0.0

These are the execution times for the current indicators using two years of historical daily stock quotes (502 periods) with default or typical parameters.

``` bash
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19041.572 (2004/?/20H1)
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.1.403
  [Host]     : .NET Core 3.1.9 (CoreCLR 4.700.20.47201, CoreFX 4.700.20.47203), X64 RyuJIT
  DefaultJob : .NET Core 3.1.9 (CoreCLR 4.700.20.47201, CoreFX 4.700.20.47203), X64 RyuJIT
```

## indicators

|             Method |        Mean |     Error |    StdDev |      Median |
|------------------- |------------:|----------:|----------:|------------:|
|             GetAdl |   167.24 μs |  1.787 μs |  1.492 μs |   167.20 μs |
|      GetAdlWithSma |   455.23 μs |  7.843 μs |  6.953 μs |   452.40 μs |
|             GetAdx |   855.60 μs |  8.082 μs |  7.560 μs |   857.01 μs |
|           GetAroon |   355.76 μs |  4.947 μs |  4.859 μs |   355.15 μs |
|             GetAtr |   200.16 μs |  2.224 μs |  1.857 μs |   199.26 μs |
|            GetBeta | 1,239.95 μs |  3.319 μs |  2.771 μs | 1,240.02 μs |
|  GetBollingerBands |   524.79 μs |  3.694 μs |  3.085 μs |   524.77 μs |
|             GetCci | 1,106.68 μs | 11.180 μs |  9.335 μs | 1,102.47 μs |
|      GetChaikinOsc |   431.92 μs |  2.981 μs |  2.489 μs |   432.03 μs |
|      GetChandelier |   409.24 μs |  3.950 μs |  3.298 μs |   408.91 μs |
|             GetCmf |   733.20 μs |  8.408 μs |  7.865 μs |   730.07 μs |
|      GetConnorsRsi | 1,652.26 μs | 18.406 μs | 17.217 μs | 1,642.92 μs |
|     GetCorrelation | 1,094.46 μs |  2.129 μs |  1.887 μs | 1,094.06 μs |
|        GetDonchian |   347.98 μs |  3.658 μs |  3.242 μs |   346.65 μs |
|       GetDoubleEma |   291.68 μs |  3.956 μs |  3.507 μs |   290.22 μs |
|             GetEma |   151.47 μs |  0.752 μs |  0.667 μs |   151.33 μs |
|      GetHeikinAshi |   220.23 μs |  5.598 μs | 16.239 μs |   209.94 μs |
|             GetHma | 1,695.43 μs |  4.669 μs |  3.645 μs | 1,694.16 μs |
|        GetIchimoku |   959.77 μs |  5.060 μs |  4.485 μs |   959.35 μs |
|         GetKeltner |   616.94 μs |  8.850 μs |  7.390 μs |   615.32 μs |
|            GetMacd |   464.52 μs |  3.691 μs |  3.453 μs |   462.54 μs |
|             GetMfi |   544.27 μs | 10.676 μs | 17.540 μs |   534.62 μs |
|             GetObv |    82.03 μs |  0.345 μs |  0.306 μs |    81.93 μs |
|      GetObvWithSma |   198.84 μs |  2.002 μs |  1.873 μs |   197.91 μs |
|    GetParabolicSar |   113.55 μs |  0.709 μs |  0.592 μs |   113.33 μs |
|             GetPmo |   363.58 μs |  5.214 μs |  4.878 μs |   360.75 μs |
|             GetPrs |   146.59 μs |  1.937 μs |  1.717 μs |   145.79 μs |
|      GetPrsWithSma |   237.48 μs |  3.367 μs |  3.877 μs |   236.03 μs |
|             GetRoc |   106.25 μs |  0.843 μs |  0.747 μs |   106.20 μs |
|      GetRocWithSma |   387.87 μs |  5.278 μs |  4.937 μs |   385.15 μs |
|             GetRsi |   413.49 μs |  3.968 μs |  3.313 μs |   412.55 μs |
|           GetSlope | 1,180.92 μs | 11.240 μs |  9.964 μs | 1,176.32 μs |
|             GetSma |   140.17 μs |  0.570 μs |  0.476 μs |   139.98 μs |
|     GetSmaExtended | 1,166.91 μs |  4.267 μs |  3.332 μs | 1,165.93 μs |
|          GetStdDev |   374.02 μs |  3.497 μs |  3.100 μs |   373.65 μs |
|   GetStdDevWithSma |   533.23 μs |  1.620 μs |  1.353 μs |   533.39 μs |
|           GetStoch |   404.15 μs |  3.478 μs |  3.083 μs |   402.51 μs |
|        GetStochRsi |   820.39 μs |  7.537 μs |  6.682 μs |   817.24 μs |
|       GetTripleEma |   429.46 μs |  3.667 μs |  3.062 μs |   428.15 μs |
|            GetTrix |   489.86 μs |  2.185 μs |  1.937 μs |   489.31 μs |
|     GetTrixWithSma |   597.82 μs | 11.434 μs | 21.475 μs |   588.12 μs |
|      GetUlcerIndex | 1,553.59 μs | 11.438 μs |  9.551 μs | 1,549.02 μs |
|        GetUltimate |   982.79 μs |  4.019 μs |  3.563 μs |   981.74 μs |
|          GetVolSma |   166.87 μs |  1.230 μs |  1.150 μs |   166.56 μs |
|        GetWilliamR |   323.22 μs |  1.461 μs |  1.295 μs |   322.73 μs |
|             GetWma |   908.22 μs | 11.714 μs |  9.145 μs |   906.89 μs |
|          GetZigZag |   264.63 μs |  4.154 μs |  7.165 μs |   261.03 μs |

## internal cleaners

|           Method |     Mean |    Error |   StdDev |   Median |
|----------------- |---------:|---------:|---------:|---------:|
|   PrepareHistory | 47.90 μs | 0.875 μs | 2.337 μs | 46.95 μs |
| PrepareBasicData | 33.88 μs | 0.595 μs | 1.058 μs | 33.42 μs |

## internal math functions

| Method | Periods |        Mean |     Error |   StdDev |
|------- |-------- |------------:|----------:|---------:|
| StdDev |      20 |    37.45 ns |  0.291 ns | 0.258 ns |
| StdDev |      50 |    98.09 ns |  0.831 ns | 0.778 ns |
| StdDev |     250 |   534.89 ns |  1.562 ns | 1.220 ns |
| StdDev |    1000 | 2,167.24 ns | 10.419 ns | 9.236 ns |
