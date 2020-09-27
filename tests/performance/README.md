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
|                   GetAdl |   164.60 us |  1.043 us |  0.976 us |
|                   GetAdx |   845.75 us |  3.485 us |  2.910 us |
|                 GetAroon |   359.33 us |  1.815 us |  1.515 us |
|                   GetAtr |   205.75 us |  1.710 us |  1.600 us |
|                  GetBeta | 1,265.96 us | 25.119 us | 42.653 us |
|        GetBollingerBands |   525.91 us |  2.474 us |  2.065 us |
|                   GetCci | 1,091.22 us |  4.156 us |  3.471 us |
|            GetChaikinOsc |   442.40 us |  6.910 us |  8.486 us |
|            GetChandelier |   413.63 us |  2.783 us |  2.324 us |
|                   GetCmf |   728.49 us |  3.405 us |  2.843 us |
|            GetConnorsRsi | 1,653.33 us | 10.409 us |  9.736 us |
|           GetCorrelation | 1,103.21 us | 14.299 us | 12.676 us |
|              GetDonchian |   343.38 us |  3.042 us |  2.540 us |
|             GetDoubleEma |   293.87 us |  0.767 us |  0.641 us |
|                   GetEma |   156.96 us |  0.491 us |  0.459 us |
|            GetHeikinAshi |   209.11 us |  0.508 us |  0.475 us |
|                   GetHma | 1,704.73 us | 21.810 us | 20.401 us |
|              GetIchimoku |   900.98 us |  2.829 us |  2.363 us |
|               GetKeltner |   618.38 us |  4.727 us |  3.691 us |
|                  GetMacd |   469.48 us |  3.423 us |  3.202 us |
|                   GetMfi |   533.09 us |  1.822 us |  1.615 us |
|                   GetObv |    79.33 us |  0.288 us |  0.255 us |
|          GetParabolicSar |   115.66 us |  0.637 us |  0.532 us |
|                   GetPmo |   360.52 us |  3.744 us |  3.503 us |
|                   GetPrs |   145.28 us |  0.307 us |  0.256 us |
|                   GetRoc |   104.93 us |  0.220 us |  0.184 us |
|                   GetRsi |   423.80 us |  3.037 us |  2.692 us |
|                 GetSlope | 1,160.92 us |  6.177 us |  5.158 us |
|                   GetSma |   139.72 us |  0.630 us |  0.589 us |
|                GetStdDev |   372.50 us |  3.567 us |  3.337 us |
|                 GetStoch |   417.40 us |  4.008 us |  3.749 us |
|              GetStochRsi |   820.29 us | 11.483 us | 10.742 us |
|             GetTripleEma |   432.54 us |  4.856 us |  4.542 us |
|            GetUlcerIndex | 1,551.47 us | 15.428 us | 14.431 us |
|              GetUltimate |   980.96 us |  2.913 us |  2.583 us |
|                GetVolSma |   165.49 us |  0.951 us |  0.843 us |
|              GetWilliamR |   321.39 us |  3.045 us |  2.849 us |
|                   GetWma |   905.77 us |  5.484 us |  4.862 us |
|                GetZigZag |   262.02 us |  1.229 us |  1.089 us |

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
