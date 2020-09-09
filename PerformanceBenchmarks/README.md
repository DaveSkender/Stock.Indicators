# Performance benchmarks

## for v0.10.10

These are the execution times for the current indicators using two years of historical daily stock quotes (502 periods) with default or typical parameters.

``` bash
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19041.508 (2004/?/20H1)
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.1.402
  [Host]     : .NET Core 3.1.8 (CoreCLR 4.700.20.41105, CoreFX 4.700.20.41903), X64 RyuJIT
  DefaultJob : .NET Core 3.1.8 (CoreCLR 4.700.20.41105, CoreFX 4.700.20.41903), X64 RyuJIT
```

|            Method |       Mean |    Error |   StdDev |
|------------------ |-----------:|---------:|---------:|
|            GetAdl |   335.7 μs |  5.91 μs |  7.04 μs |
|            GetAdx | 1,236.6 μs | 10.20 μs |  9.54 μs |
|          GetAroon | 2,918.4 μs | 25.19 μs | 23.57 μs |
|            GetAtr |   366.0 μs |  2.69 μs |  2.51 μs |
|           GetBeta | 3,969.0 μs | 30.95 μs | 27.44 μs |
|    GetCorrelation | 3,816.4 μs | 36.16 μs | 33.83 μs |
| GetBollingerBands | 2,205.7 μs |  8.39 μs |  7.44 μs |
|            GetCci | 2,428.5 μs | 24.58 μs | 23.00 μs |
|            GetCmf | 2,017.6 μs | 20.33 μs | 19.02 μs |
|     GetChaikinOsc |   797.1 μs | 15.75 μs | 19.92 μs |
|     GetChandelier | 2,393.8 μs | 19.44 μs | 18.18 μs |
|     GetConnorsRsi | 4,260.2 μs | 69.90 μs | 65.38 μs |
|       GetDonchian | 2,328.0 μs | 12.51 μs | 11.70 μs |
|            GetEma |   402.8 μs |  4.39 μs |  3.66 μs |
|     GetHeikinAshi |   380.9 μs |  2.53 μs |  2.24 μs |
|            GetHma | 7,214.2 μs | 76.14 μs | 63.58 μs |
|       GetIchimoku | 6,844.1 μs | 44.26 μs | 34.56 μs |
|        GetKeltner | 1,106.9 μs | 10.24 μs |  9.58 μs |
|           GetMacd |   630.9 μs |  3.67 μs |  3.25 μs |
|            GetMfi | 2,191.4 μs | 13.34 μs | 12.48 μs |
|            GetObv |   238.1 μs |  2.15 μs |  1.90 μs |
|   GetParabolicSar |   290.5 μs |  2.50 μs |  2.21 μs |
|            GetPmo |   706.9 μs |  5.97 μs |  4.99 μs |
|            GetRoc |   278.7 μs |  2.86 μs |  2.68 μs |
|            GetRsi |   785.0 μs | 19.90 μs | 58.04 μs |
|            GetSma | 1,855.3 μs | 12.91 μs | 11.44 μs |
|         GetStdDev | 2,204.5 μs | 28.53 μs | 25.29 μs |
|          GetStoch | 4,297.7 μs | 45.28 μs | 42.36 μs |
|       GetStochRsi | 3,675.8 μs | 32.09 μs | 28.45 μs |
|     GetUlcerIndex | 5,257.1 μs | 58.48 μs | 54.70 μs |
|       GetWilliamR | 2,340.7 μs | 18.09 μs | 16.93 μs |
|            GetWma | 4,519.3 μs | 31.39 μs | 29.36 μs |
|         GetZigZag |   444.0 μs |  2.38 μs |  2.23 μs |
