# Performance benchmarks

## for v0.10.27

These are the execution times for the current indicators using two years of historical daily stock quotes (502 periods) with default or typical parameters.

``` bash
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19041.508 (2004/?/20H1)
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.1.402
  [Host]     : .NET Core 3.1.8 (CoreCLR 4.700.20.41105, CoreFX 4.700.20.41903), X64 RyuJIT
  DefaultJob : .NET Core 3.1.8 (CoreCLR 4.700.20.41105, CoreFX 4.700.20.41903), X64 RyuJIT
```

|            Method |       Mean |    Error |    StdDev |     Median |
|------------------ |-----------:|---------:|----------:|-----------:|
|            GetAdl |   338.2 μs |  6.73 μs |  15.61 μs |   334.9 μs |
|            GetAdx | 1,224.2 μs | 14.83 μs |  13.14 μs | 1,226.3 μs |
|          GetAroon | 2,928.1 μs | 37.89 μs |  35.44 μs | 2,908.6 μs |
|            GetAtr |   374.8 μs |  5.15 μs |   4.82 μs |   374.2 μs |
|           GetBeta | 3,935.8 μs | 34.91 μs |  32.65 μs | 3,936.8 μs |
|    GetCorrelation | 3,723.0 μs | 64.69 μs |  60.51 μs | 3,726.1 μs |
| GetBollingerBands | 2,147.5 μs | 18.51 μs |  15.46 μs | 2,147.0 μs |
|            GetCci | 2,431.7 μs | 26.14 μs |  23.17 μs | 2,432.3 μs |
|            GetCmf | 3,570.9 μs | 71.05 μs | 198.07 μs | 3,526.1 μs |
|     GetChaikinOsc |   757.1 μs | 16.19 μs |  46.72 μs |   731.9 μs |
|     GetChandelier | 2,217.9 μs | 31.25 μs |  27.70 μs | 2,209.5 μs |
|     GetConnorsRsi | 4,079.3 μs | 38.89 μs |  36.38 μs | 4,076.3 μs |
|       GetDonchian | 2,152.2 μs | 37.63 μs |  31.43 μs | 2,138.2 μs |
|      GetDoubleEma |   538.9 μs |  3.48 μs |   3.09 μs |   538.2 μs |
|            GetEma |   358.7 μs |  2.32 μs |   2.17 μs |   358.6 μs |
|     GetHeikinAshi |   348.5 μs |  1.46 μs |   1.22 μs |   348.3 μs |
|            GetHma | 6,740.9 μs | 52.67 μs |  46.69 μs | 6,754.0 μs |
|       GetIchimoku | 6,407.0 μs | 52.49 μs |  49.10 μs | 6,384.7 μs |
|        GetKeltner | 1,034.1 μs |  8.06 μs |   7.54 μs | 1,035.1 μs |
|           GetMacd |   573.1 μs |  6.11 μs |   5.42 μs |   572.5 μs |
|            GetMfi | 2,070.1 μs | 14.38 μs |  12.01 μs | 2,069.6 μs |
|            GetObv |   217.7 μs |  1.85 μs |   1.73 μs |   217.4 μs |
|   GetParabolicSar |   256.7 μs |  1.23 μs |   1.09 μs |   256.5 μs |
|            GetPmo |   729.8 μs | 31.25 μs |  92.15 μs |   710.8 μs |
|            GetRoc |   342.2 μs |  4.37 μs |   3.87 μs |   341.4 μs |
|            GetRsi |   798.5 μs |  6.06 μs |   5.67 μs |   797.8 μs |
|            GetSma | 2,570.3 μs | 40.71 μs |  36.09 μs | 2,570.8 μs |
|         GetStdDev | 2,812.9 μs | 32.96 μs |  48.32 μs | 2,807.9 μs |
|          GetStoch | 5,928.9 μs | 57.62 μs |  53.89 μs | 5,932.3 μs |
|       GetStochRsi | 5,006.4 μs | 60.66 μs |  56.74 μs | 5,007.8 μs |
|     GetUlcerIndex | 7,355.8 μs | 54.86 μs |  51.31 μs | 7,378.9 μs |
|       GetWilliamR | 3,292.1 μs | 46.88 μs |  43.85 μs | 3,284.2 μs |
|            GetWma | 5,901.4 μs | 95.95 μs |  80.12 μs | 5,867.6 μs |
|         GetZigZag |   589.5 μs |  8.82 μs |   8.25 μs |   590.9 μs |
