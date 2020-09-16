# Performance benchmarks

Updated for v0.10.30

These are the execution times for the current indicators using two years of historical daily stock quotes (502 periods) with default or typical parameters.

``` bash
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19041.508 (2004/?/20H1)
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.1.402
  [Host]     : .NET Core 3.1.8 (CoreCLR 4.700.20.41105, CoreFX 4.700.20.41903), X64 RyuJIT
  DefaultJob : .NET Core 3.1.8 (CoreCLR 4.700.20.41105, CoreFX 4.700.20.41903), X64 RyuJIT
```

## indicators

|            Method |       Mean |    Error |   StdDev |     Median |
|------------------ |-----------:|---------:|---------:|-----------:|
|            GetAdl |   227.6 us |  1.33 us |  1.18 us |   227.3 us |
|            GetAdx |   939.4 us |  3.88 us |  3.03 us |   938.6 us |
|          GetAroon |   384.0 us |  7.12 us |  6.66 us |   382.7 us |
|            GetAtr |   264.3 us |  2.02 us |  1.68 us |   263.6 us |
|           GetBeta | 1,412.8 us | 27.66 us | 27.17 us | 1,397.8 us |
| GetBollingerBands |   467.1 us |  8.79 us |  9.41 us |   463.1 us |
|            GetCci | 1,119.5 us | 13.29 us | 11.10 us | 1,116.4 us |
|     GetChaikinOsc |   547.4 us |  2.84 us |  2.66 us |   547.3 us |
|     GetChandelier |   505.2 us |  3.40 us |  3.01 us |   504.1 us |
|            GetCmf |   816.6 us |  8.15 us |  7.63 us |   812.7 us |
|     GetConnorsRsi | 1,683.3 us | 12.81 us | 11.98 us | 1,677.7 us |
|    GetCorrelation | 1,228.5 us | 18.65 us | 16.53 us | 1,220.7 us |
|       GetDonchian |   376.0 us |  3.15 us |  2.95 us |   374.8 us |
|      GetDoubleEma |   319.4 us |  1.66 us |  1.30 us |   319.2 us |
|            GetEma |   181.7 us |  1.91 us |  1.78 us |   181.5 us |
|     GetHeikinAshi |   292.0 us |  4.23 us |  3.96 us |   291.0 us |
|            GetHma | 1,780.2 us | 16.52 us | 15.46 us | 1,772.2 us |
|       GetIchimoku |   935.7 us | 10.13 us |  9.48 us |   929.7 us |
|        GetKeltner |   754.7 us | 14.70 us | 33.48 us |   742.5 us |
|           GetMacd |   509.7 us |  9.38 us | 18.51 us |   500.6 us |
|            GetMfi |   560.7 us |  2.74 us |  2.43 us |   560.4 us |
|            GetObv |   143.2 us |  0.43 us |  0.34 us |   143.2 us |
|   GetParabolicSar |   147.1 us |  2.03 us |  1.70 us |   146.5 us |
|            GetPmo |   422.8 us |  2.39 us |  2.00 us |   422.2 us |
|            GetRoc |   129.5 us |  0.34 us |  0.28 us |   129.4 us |
|            GetRsi |   444.5 us |  4.49 us |  4.20 us |   442.9 us |
|            GetSma |   165.6 us |  0.53 us |  0.42 us |   165.7 us |
|         GetStdDev |   405.9 us |  4.32 us |  4.04 us |   404.8 us |
|          GetStoch |   442.8 us |  1.10 us |  0.86 us |   442.7 us |
|       GetStochRsi |   889.9 us |  7.94 us |  7.04 us |   887.2 us |
|     GetUlcerIndex | 1,588.7 us | 29.36 us | 24.52 us | 1,577.7 us |
|       GetWilliamR |   398.0 us |  7.72 us | 10.82 us |   393.7 us |
|            GetWma |   935.6 us |  9.47 us |  8.86 us |   931.2 us |
|         GetZigZag |   308.0 us |  3.76 us |  3.34 us |   307.6 us |

## cleaners

|           Method |     Mean |    Error |   StdDev |
|----------------- |---------:|---------:|---------:|
|   PrepareHistory | 73.00 us | 0.467 us | 0.414 us |
| PrepareBasicData | 32.71 us | 0.375 us | 0.351 us |

## shared functions

| Method | Periods |        Mean |     Error |    StdDev |
|------- |-------- |------------:|----------:|----------:|
| StdDev |      20 |    38.13 ns |  0.775 ns |  0.687 ns |
| StdDev |      50 |    98.43 ns |  1.998 ns |  2.991 ns |
| StdDev |     250 |   532.63 ns |  1.947 ns |  1.626 ns |
| StdDev |    1000 | 2,158.71 ns | 14.582 ns | 12.927 ns |
