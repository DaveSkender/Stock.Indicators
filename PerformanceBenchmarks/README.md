# Performance benchmarks

## for v0.10.7

These are the execution times for the current indicators using two years of historical daily stock quotes (502 periods) with default or typical parameters.

``` bash
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19041.508 (2004/?/20H1)
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.1.402
  [Host]     : .NET Core 3.1.8 (CoreCLR 4.700.20.41105, CoreFX 4.700.20.41903), X64 RyuJIT
  DefaultJob : .NET Core 3.1.8 (CoreCLR 4.700.20.41105, CoreFX 4.700.20.41903), X64 RyuJIT
```
|            Method |        Mean |       Error |      StdDev |
|------------------ |------------:|------------:|------------:|
|            GetAdl |    330.6 μs |     4.37 μs |     4.09 μs |
|          GetAroon | 25,331.6 μs |   131.59 μs |   109.88 μs |
|            GetAdx |  2,221.6 μs |    26.82 μs |    25.09 μs |
|            GetAtr |    365.5 μs |     4.89 μs |     4.58 μs |
|           GetBeta |  6,004.8 μs |    58.12 μs |    54.36 μs |
| GetBollingerBands | 23,928.9 μs |   301.26 μs |   281.80 μs |
|            GetCci |  2,421.4 μs |    19.66 μs |    18.39 μs |
|            GetCmf |  1,997.0 μs |    30.86 μs |    27.35 μs |
|     GetChaikinOsc |  2,687.7 μs |    51.92 μs |    59.79 μs |
|     GetChandelier | 26,586.2 μs |   279.97 μs |   261.88 μs |
|     GetConnorsRsi |  5,249.0 μs |    54.26 μs |    50.75 μs |
|    GetCorrelation |  4,803.9 μs |    55.58 μs |    51.99 μs |
|       GetDonchian | 24,625.0 μs |   301.55 μs |   282.07 μs |
|            GetEma |    386.8 μs |     3.80 μs |     3.56 μs |
|     GetHeikinAshi |    376.9 μs |     5.77 μs |     5.12 μs |
|            GetHma | 55,590.3 μs |   642.85 μs |   501.90 μs |
|       GetIchimoku | 93,090.7 μs | 1,177.02 μs | 1,043.40 μs |
|        GetKeltner |  3,190.5 μs |    34.21 μs |    32.00 μs |
|           GetMacd |    621.0 μs |     6.02 μs |     5.63 μs |
|            GetMfi |  9,389.9 μs |    56.31 μs |    49.92 μs |
|            GetObv |    252.6 μs |     5.02 μs |     9.54 μs |
|   GetParabolicSar |    299.8 μs |      TBD μs |      TBD μs |
|            GetPmo | 38,080.1 μs |   748.88 μs | 1,460.64 μs |
|            GetRoc | 32,278.3 μs |   630.76 μs |   647.74 μs |
|            GetRsi |    945.4 μs |    15.41 μs |    13.66 μs |
|            GetSma | 40,899.6 μs |   812.66 μs | 2,097.74 μs |
|         GetStdDev |  3,417.5 μs |    64.90 μs |   115.36 μs |
|          GetStoch | 42,766.5 μs |   835.11 μs | 1,462.63 μs |
|       GetStochRsi |  7,986.5 μs |   152.54 μs |   350.48 μs |
|     GetUlcerIndex | 43,950.7 μs |   863.87 μs | 1,622.55 μs |
|       GetWilliamR | 42,279.5 μs |   829.55 μs | 1,049.11 μs |
|            GetWma | 42,921.2 μs |   812.04 μs |   868.88 μs |
|         GetZigZag |  2,430.3 μs |    48.48 μs |    63.03 μs |