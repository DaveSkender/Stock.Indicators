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
|            GetAdl |   229.4 us |  2.40 us |  2.25 us |   227.9 us |
|            GetAdx |   942.2 us |  6.35 us |  5.94 us |   940.9 us |
|          GetAroon |   390.0 us |  3.49 us |  3.09 us |   388.8 us |
|            GetAtr |   267.6 us |  2.05 us |  1.81 us |   267.1 us |
|           GetBeta | 1,435.9 us | 14.76 us | 13.80 us | 1,427.9 us |
| GetBollingerBands |   464.7 us |  3.04 us |  2.84 us |   464.8 us |
|            GetCci | 1,134.0 us |  3.16 us |  2.64 us | 1,134.1 us |
|     GetChaikinOsc |   533.2 us |  2.76 us |  2.30 us |   533.6 us |
|     GetChandelier |   499.2 us |  1.92 us |  1.71 us |   499.5 us |
|            GetCmf |   826.4 us |  4.48 us |  4.19 us |   825.7 us |
|     GetConnorsRsi | 1,684.4 us |  9.09 us |  8.50 us | 1,681.9 us |
|    GetCorrelation | 1,226.4 us |  8.60 us |  7.62 us | 1,224.3 us |
|       GetDonchian |   382.4 us |  5.14 us |  4.81 us |   380.4 us |
|      GetDoubleEma |   323.6 us |  1.94 us |  1.62 us |   323.2 us |
|            GetEma |   190.3 us |  3.78 us |  8.77 us |   186.5 us |
|     GetHeikinAshi |   270.8 us |  1.91 us |  1.78 us |   270.2 us |
|            GetHma | 1,790.9 us | 28.42 us | 35.94 us | 1,785.2 us |
|       GetIchimoku |   970.1 us |  5.43 us |  5.08 us |   969.3 us |
|        GetKeltner |   742.3 us |  3.33 us |  2.78 us |   742.7 us |
|           GetMacd |   500.3 us |  2.35 us |  2.08 us |   499.7 us |
|            GetMfi |   564.6 us |  2.15 us |  1.91 us |   563.9 us |
|            GetObv |   145.3 us |  0.50 us |  0.44 us |   145.2 us |
|   GetParabolicSar |   144.1 us |  0.45 us |  0.40 us |   144.1 us |
|            GetPmo |   461.2 us |  4.40 us |  3.68 us |   460.7 us |
|            GetRoc |   130.1 us |  0.60 us |  0.50 us |   130.0 us |
|            GetRsi |   447.9 us |  3.18 us |  2.82 us |   447.5 us |
|            GetSma |   165.3 us |  0.57 us |  0.48 us |   165.2 us |
|         GetStdDev |   399.8 us |  3.65 us |  3.24 us |   398.7 us |
|          GetStoch |   438.1 us |  2.61 us |  3.20 us |   436.7 us |
|       GetStochRsi |   891.1 us |  6.78 us |  6.01 us |   890.0 us |
|     GetUlcerIndex | 1,551.6 us | 16.60 us | 14.71 us | 1,544.8 us |
|       GetWilliamR |   396.8 us |  6.56 us |  7.81 us |   393.3 us |
|            GetWma |   930.6 us |  5.19 us |  4.60 us |   929.1 us |
|         GetZigZag |   285.5 us |  1.83 us |  1.71 us |   285.2 us |

## cleaners

|           Method |     Mean |    Error |   StdDev |
|----------------- |---------:|---------:|---------:|
|   PrepareHistory | 73.00 us | 0.467 us | 0.414 us |
| PrepareBasicData | 32.71 us | 0.375 us | 0.351 us |

## shared functions

| Method | Periods |        Mean |     Error |    StdDev |      Median |
|------- |-------- |------------:|----------:|----------:|------------:|
| StdDev |      20 |    41.32 ns |  1.526 ns |  4.404 ns |    38.53 ns |
| StdDev |      50 |   100.08 ns |  0.846 ns |  0.750 ns |    99.76 ns |
| StdDev |     250 |   537.13 ns |  2.564 ns |  2.273 ns |   537.04 ns |
| StdDev |    1000 | 2,173.03 ns | 36.878 ns | 32.691 ns | 2,157.19 ns |
