# Performance benchmarks

## for v0.10.5 (internal release)

As a baseline, here is the starting point on the performance of the current indicators **using two years of historical daily stock quotes** (e.g. 502 periods) with default or typical parameters.  I believe the performance to be correlated with `history` length, so you can extrapolate based how much you use.

``` ini
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19041.450 (2004/?/20H1)
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.1.401
  [Host]     : .NET Core 3.1.7 (CoreCLR 4.700.20.36602, CoreFX 4.700.20.37001), X64 RyuJIT
  DefaultJob : .NET Core 3.1.7 (CoreCLR 4.700.20.36602, CoreFX 4.700.20.37001), X64 RyuJIT
```

|            Method |        Mean |       Error |      StdDev |
|------------------ |------------:|------------:|------------:|
|            GetAdl |    335.2 μs |     3.92 μs |     3.27 μs |
|          GetAroon | 24,933.9 μs |   363.25 μs |   303.33 μs |
|            GetAdx |  2,311.2 μs |    42.90 μs |    35.83 μs |
|            GetAtr |    380.2 μs |     7.38 μs |     9.59 μs |
| GetBollingerBands | 23,738.6 μs |   310.69 μs |   290.62 μs |
|            GetCci |  2,587.7 μs |    27.05 μs |    23.98 μs |
|            GetCmf |  1,987.9 μs |    21.33 μs |    19.95 μs |
|     GetChaikinOsc |  2,701.4 μs |    11.66 μs |    10.91 μs |
|     GetChandelier | 26,071.7 μs |   153.06 μs |   135.68 μs |
|     GetConnorsRsi |  5,362.2 μs |    47.29 μs |    39.49 μs |
|       GetDonchian | 25,198.0 μs |   144.92 μs |   135.55 μs |
|            GetEma |    387.5 μs |     2.85 μs |     2.53 μs |
|     GetHeikinAshi |    374.2 μs |     3.81 μs |     3.38 μs |
|            GetHma | 60,646.3 μs | 1,209.50 μs | 2,359.03 μs |
|       GetIchimoku | 99,853.4 μs | 2,407.53 μs | 6,750.96 μs |
|        GetKeltner |  3,105.3 μs |    18.69 μs |    16.57 μs |
|           GetMacd |  3,584.8 μs |    67.56 μs |    63.20 μs |
|            GetMfi |  8,464.6 μs |   125.72 μs |   117.60 μs |
|            GetObv |    235.0 μs |     2.70 μs |     2.53 μs |
|   GetParabolicSar | 22,322.4 μs |   184.00 μs |   143.66 μs |
|            GetPmo | 24,433.4 μs |   245.61 μs |   229.74 μs |
|            GetRoc | 21,636.6 μs |   145.37 μs |   128.87 μs |
|            GetRsi |    682.6 μs |     9.08 μs |     8.49 μs |
|            GetSma | 26,592.6 μs |   290.79 μs |   257.78 μs |
|         GetStdDev |  2,147.0 μs |    29.63 μs |    26.26 μs |
|          GetStoch | 26,191.8 μs |   184.06 μs |   163.17 μs |
|       GetStochRsi |  4,671.5 μs |    31.04 μs |    27.51 μs |
|     GetUlcerIndex | 28,494.0 μs |   442.06 μs |   391.87 μs |
|       GetWilliamR | 25,799.3 μs |   281.23 μs |   219.56 μs |
|            GetWma | 26,955.1 μs |   182.45 μs |   161.74 μs |
|         GetZigZag |  1,599.9 μs |    31.77 μs |    69.74 μs |