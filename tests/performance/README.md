# Performance benchmarks

Updated for v0.11.2

These are the execution times for the current indicators using two years of historical daily stock quotes (502 periods) with default or typical parameters.

``` bash
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19041.508 (2004/?/20H1)
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.1.402
  [Host]     : .NET Core 3.1.8 (CoreCLR 4.700.20.41105, CoreFX 4.700.20.41903), X64 RyuJIT
  DefaultJob : .NET Core 3.1.8 (CoreCLR 4.700.20.41105, CoreFX 4.700.20.41903), X64 RyuJIT
```

## indicators

|                   Method |        Mean |     Error |    StdDev |
|------------------------- |------------:|----------:|----------:|
|                   GetAdl |   164.60 μs |  1.043 μs |  0.976 μs |
|                   GetAdx |   845.75 μs |  3.485 μs |  2.910 μs |
|                 GetAroon |   359.33 μs |  1.815 μs |  1.515 μs |
|                   GetAtr |   205.75 μs |  1.710 μs |  1.600 μs |
|                  GetBeta | 1,265.96 μs | 25.119 μs | 42.653 μs |
|        GetBollingerBands |   525.91 μs |  2.474 μs |  2.065 μs |
|                   GetCci | 1,091.22 μs |  4.156 μs |  3.471 μs |
|            GetChaikinOsc |   442.40 μs |  6.910 μs |  8.486 μs |
|            GetChandelier |   413.63 μs |  2.783 μs |  2.324 μs |
|                   GetCmf |   728.49 μs |  3.405 μs |  2.843 μs |
|            GetConnorsRsi | 1,653.33 μs | 10.409 μs |  9.736 μs |
|           GetCorrelation | 1,103.21 μs | 14.299 μs | 12.676 μs |
|              GetDonchian |   343.38 μs |  3.042 μs |  2.540 μs |
|             GetDoubleEma |   293.87 μs |  0.767 μs |  0.641 μs |
|                   GetEma |   156.96 μs |  0.491 μs |  0.459 μs |
|            GetHeikinAshi |   209.11 μs |  0.508 μs |  0.475 μs |
|                   GetHma | 1,704.73 μs | 21.810 μs | 20.401 μs |
|              GetIchimoku |   900.98 μs |  2.829 μs |  2.363 μs |
|               GetKeltner |   618.38 μs |  4.727 μs |  3.691 μs |
|                  GetMacd |   469.48 μs |  3.423 μs |  3.202 μs |
|                   GetMfi |   533.09 μs |  1.822 μs |  1.615 μs |
|                   GetObv |    79.33 μs |  0.288 μs |  0.255 μs |
|          GetParabolicSar |   115.66 μs |  0.637 μs |  0.532 μs |
|                   GetPmo |   360.52 μs |  3.744 μs |  3.503 μs |
|                   GetPrs |   145.28 μs |  0.307 μs |  0.256 μs |
|                   GetRoc |   104.93 μs |  0.220 μs |  0.184 μs |
|                   GetRsi |   423.80 μs |  3.037 μs |  2.692 μs |
|                 GetSlope | 1,160.92 μs |  6.177 μs |  5.158 μs |
|                   GetSma |   139.72 μs |  0.630 μs |  0.589 μs |
|                GetStdDev |   372.50 μs |  3.567 μs |  3.337 μs |
|                 GetStoch |   417.40 μs |  4.008 μs |  3.749 μs |
|              GetStochRsi |   820.29 μs | 11.483 μs | 10.742 μs |
|             GetTripleEma |   432.54 μs |  4.856 μs |  4.542 μs |
|            GetUlcerIndex | 1,551.47 μs | 15.428 μs | 14.431 μs |
|              GetUltimate |   980.96 μs |  2.913 μs |  2.583 μs |
|                GetVolSma |   165.49 μs |  0.951 μs |  0.843 μs |
|              GetWilliamR |   321.39 μs |  3.045 μs |  2.849 μs |
|                   GetWma |   905.77 μs |  5.484 μs |  4.862 μs |
|                GetZigZag |   262.02 μs |  1.229 μs |  1.089 μs |

## internal cleaners

|           Method |     Mean |    Error |   StdDev |
|----------------- |---------:|---------:|---------:|
|   PrepareHistory | 49.63 μs | 0.838 μs | 0.654 μs |
| PrepareBasicData | 33.47 μs | 0.317 μs | 0.265 μs |

## internal math functions

| Method | Periods |        Mean |     Error |   StdDev |
|------- |-------- |------------:|----------:|---------:|
| StdDev |      20 |    37.45 ns |  0.291 ns | 0.258 ns |
| StdDev |      50 |    98.09 ns |  0.831 ns | 0.778 ns |
| StdDev |     250 |   534.89 ns |  1.562 ns | 1.220 ns |
| StdDev |    1000 | 2,167.24 ns | 10.419 ns | 9.236 ns |
