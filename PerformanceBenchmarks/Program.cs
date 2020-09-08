using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Skender.Stock.Indicators;
using StockIndicators.Tests;
using System;
using System.Collections.Generic;

namespace PerformanceBenchmarks
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Running benchmarks");
            Summary[] summary = BenchmarkRunner.Run(typeof(Program).Assembly);
        }
    }

    [MarkdownExporterAttribute.GitHub]
    public class TwentyYearDailyHistory
    {
        private readonly IEnumerable<Quote> h = History.GetHistory();

        [Benchmark]
        public void GetAdl()
        {
            Indicator.GetAdl(h);
        }

        [Benchmark]
        public void GetAroon()
        {
            Indicator.GetAroon(h);
        }

        [Benchmark]
        public void GetAdx()
        {
            Indicator.GetAdx(h);
        }

        [Benchmark]
        public void GetAtr()
        {
            Indicator.GetAtr(h);
        }

        [Benchmark]
        public void GetBollingerBands()
        {
            Indicator.GetBollingerBands(h);
        }

        [Benchmark]
        public void GetCci()
        {
            Indicator.GetCci(h);
        }

        [Benchmark]
        public void GetCmf()
        {
            Indicator.GetCmf(h);
        }

        [Benchmark]
        public void GetChaikinOsc()
        {
            Indicator.GetChaikinOsc(h);
        }

        [Benchmark]
        public void GetChandelier()
        {
            Indicator.GetChandelier(h);
        }

        [Benchmark]
        public void GetConnorsRsi()
        {
            Indicator.GetConnorsRsi(h);
        }

        [Benchmark]
        public void GetDonchian()
        {
            Indicator.GetDonchian(h);
        }

        [Benchmark]
        public void GetEma()
        {
            Indicator.GetEma(h, 14);
        }

        [Benchmark]
        public void GetHeikinAshi()
        {
            Indicator.GetHeikinAshi(h);
        }

        [Benchmark]
        public void GetHma()
        {
            Indicator.GetHma(h, 14);
        }

        [Benchmark]
        public void GetIchimoku()
        {
            Indicator.GetIchimoku(h);
        }

        [Benchmark]
        public void GetKeltner()
        {
            Indicator.GetKeltner(h);
        }

        [Benchmark]
        public void GetMacd()
        {
            Indicator.GetMacd(h);
        }

        [Benchmark]
        public void GetMfi()
        {
            Indicator.GetMfi(h);
        }

        [Benchmark]
        public void GetObv()
        {
            Indicator.GetObv(h);
        }

        [Benchmark]
        public void GetParabolicSar()
        {
            Indicator.GetParabolicSar(h);
        }

        [Benchmark]
        public void GetPmo()
        {
            Indicator.GetPmo(h);
        }

        [Benchmark]
        public void GetRoc()
        {
            Indicator.GetRoc(h, 20);
        }

        [Benchmark]
        public void GetRsi()
        {
            Indicator.GetRsi(h);
        }

        [Benchmark]
        public void GetSma()
        {
            Indicator.GetSma(h, 10);
        }

        [Benchmark]
        public void GetStdDev()
        {
            Indicator.GetStdDev(h, 20);
        }

        [Benchmark]
        public void GetStoch()
        {
            Indicator.GetStoch(h);
        }

        [Benchmark]
        public void GetStochRsi()
        {
            Indicator.GetStochRsi(h, 14, 14, 3);
        }

        [Benchmark]
        public void GetUlcerIndex()
        {
            Indicator.GetUlcerIndex(h);
        }

        [Benchmark]
        public void GetWilliamR()
        {
            Indicator.GetWilliamR(h);
        }

        [Benchmark]
        public void GetWma()
        {
            Indicator.GetWma(h, 30);
        }

        [Benchmark]
        public void GetZigZag()
        {
            Indicator.GetZigZag(h);
        }
    }
}
