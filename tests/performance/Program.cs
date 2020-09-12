using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Internal.Tests;
using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;

namespace Performance.Tests
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Running benchmarks");

            if (args.Length == 0)
            {
                // run all
                // example: dotnet run -c Release
                BenchmarkRunner.Run(typeof(Program).Assembly);
            }
            else
            {
                // run based on arguments (e.g. filter)
                // example: dotnet run -c Release --filter *.GetAdx
                BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
            }
        }
    }

    [MarkdownExporterAttribute.GitHub]
    public class Marks
    {
        private readonly IEnumerable<Quote> hm = History.GetHistory();
        private readonly IEnumerable<Quote> ho = History.GetHistoryOther();

        [Benchmark]
        public void GetAdl()
        {
            Indicator.GetAdl(hm);
        }

        [Benchmark]
        public void GetAdx()
        {
            Indicator.GetAdx(hm);
        }

        [Benchmark]
        public void GetAroon()
        {
            Indicator.GetAroon(hm);
        }

        [Benchmark]
        public void GetAtr()
        {
            Indicator.GetAtr(hm);
        }

        [Benchmark]
        public void GetBeta()
        {
            Indicator.GetBeta(hm, ho, 30);
        }

        [Benchmark]
        public void GetCorrelation()
        {
            Indicator.GetCorrelation(hm, ho, 30);
        }

        [Benchmark]
        public void GetBollingerBands()
        {
            Indicator.GetBollingerBands(hm);
        }

        [Benchmark]
        public void GetCci()
        {
            Indicator.GetCci(hm);
        }

        [Benchmark]
        public void GetCmf()
        {
            Indicator.GetCmf(hm);
        }

        [Benchmark]
        public void GetChaikinOsc()
        {
            Indicator.GetChaikinOsc(hm);
        }

        [Benchmark]
        public void GetChandelier()
        {
            Indicator.GetChandelier(hm);
        }

        [Benchmark]
        public void GetConnorsRsi()
        {
            Indicator.GetConnorsRsi(hm);
        }

        [Benchmark]
        public void GetDonchian()
        {
            Indicator.GetDonchian(hm);
        }

        [Benchmark]
        public void GetDoubleEma()
        {
            Indicator.GetDoubleEma(hm, 14);
        }
        
        [Benchmark]
        public void GetEma()
        {
            Indicator.GetEma(hm, 14);
        }

        [Benchmark]
        public void GetHeikinAshi()
        {
            Indicator.GetHeikinAshi(hm);
        }

        [Benchmark]
        public void GetHma()
        {
            Indicator.GetHma(hm, 14);
        }

        [Benchmark]
        public void GetIchimoku()
        {
            Indicator.GetIchimoku(hm);
        }

        [Benchmark]
        public void GetKeltner()
        {
            Indicator.GetKeltner(hm);
        }

        [Benchmark]
        public void GetMacd()
        {
            Indicator.GetMacd(hm);
        }

        [Benchmark]
        public void GetMfi()
        {
            Indicator.GetMfi(hm);
        }

        [Benchmark]
        public void GetObv()
        {
            Indicator.GetObv(hm);
        }

        [Benchmark]
        public void GetParabolicSar()
        {
            Indicator.GetParabolicSar(hm);
        }

        [Benchmark]
        public void GetPmo()
        {
            Indicator.GetPmo(hm);
        }

        [Benchmark]
        public void GetRoc()
        {
            Indicator.GetRoc(hm, 20);
        }

        [Benchmark]
        public void GetRsi()
        {
            Indicator.GetRsi(hm);
        }

        [Benchmark]
        public void GetSma()
        {
            Indicator.GetSma(hm, 10);
        }

        [Benchmark]
        public void GetStdDev()
        {
            Indicator.GetStdDev(hm, 20);
        }

        [Benchmark]
        public void GetStoch()
        {
            Indicator.GetStoch(hm);
        }

        [Benchmark]
        public void GetStochRsi()
        {
            Indicator.GetStochRsi(hm, 14, 14, 3);
        }

        [Benchmark]
        public void GetUlcerIndex()
        {
            Indicator.GetUlcerIndex(hm);
        }

        [Benchmark]
        public void GetWilliamR()
        {
            Indicator.GetWilliamR(hm);
        }

        [Benchmark]
        public void GetWma()
        {
            Indicator.GetWma(hm, 30);
        }

        [Benchmark]
        public void GetZigZag()
        {
            Indicator.GetZigZag(hm);
        }
    }
}
