using BenchmarkDotNet.Attributes;
using Internal.Tests;
using Skender.Stock.Indicators;
using System.Collections.Generic;

namespace Tests.Performance
{

    [MarkdownExporterAttribute.GitHub]
    public class MarkIndicators
    {
        private readonly IEnumerable<Quote> hm = History.GetHistory();
        private readonly IEnumerable<Quote> ho = History.GetHistoryOther();

        [Benchmark]
        public object GetAdl()
        {
            return Indicator.GetAdl(hm);
        }

        [Benchmark]
        public object GetAdlWithSma()
        {
            return Indicator.GetAdl(hm, 14);
        }

        [Benchmark]
        public object GetAdx()
        {
            return Indicator.GetAdx(hm);
        }

        [Benchmark]
        public object GetAroon()
        {
            return Indicator.GetAroon(hm);
        }

        [Benchmark]
        public object GetAtr()
        {
            return Indicator.GetAtr(hm);
        }

        [Benchmark]
        public object GetBeta()
        {
            return Indicator.GetBeta(hm, ho, 20);
        }

        [Benchmark]
        public object GetBollingerBands()
        {
            return Indicator.GetBollingerBands(hm);
        }

        [Benchmark]
        public object GetCci()
        {
            return Indicator.GetCci(hm);
        }

        [Benchmark]
        public object GetChaikinOsc()
        {
            return Indicator.GetChaikinOsc(hm);
        }

        [Benchmark]
        public object GetChandelier()
        {
            return Indicator.GetChandelier(hm);
        }

        [Benchmark]
        public object GetCmf()
        {
            return Indicator.GetCmf(hm);
        }

        [Benchmark]
        public object GetConnorsRsi()
        {
            return Indicator.GetConnorsRsi(hm);
        }

        [Benchmark]
        public object GetCorrelation()
        {
            return Indicator.GetCorrelation(hm, ho, 20);
        }

        [Benchmark]
        public object GetDonchian()
        {
            return Indicator.GetDonchian(hm);
        }

        [Benchmark]
        public object GetDoubleEma()
        {
            return Indicator.GetDoubleEma(hm, 14);
        }

        [Benchmark]
        public object GetEma()
        {
            return Indicator.GetEma(hm, 14);
        }

        [Benchmark]
        public object GetHeikinAshi()
        {
            return Indicator.GetHeikinAshi(hm);
        }

        [Benchmark]
        public object GetHma()
        {
            return Indicator.GetHma(hm, 14);
        }

        [Benchmark]
        public object GetIchimoku()
        {
            return Indicator.GetIchimoku(hm);
        }

        [Benchmark]
        public object GetKeltner()
        {
            return Indicator.GetKeltner(hm);
        }

        [Benchmark]
        public object GetMacd()
        {
            return Indicator.GetMacd(hm);
        }

        [Benchmark]
        public object GetMfi()
        {
            return Indicator.GetMfi(hm);
        }

        [Benchmark]
        public object GetObv()
        {
            return Indicator.GetObv(hm);
        }

        [Benchmark]
        public object GetObvWithSma()
        {
            return Indicator.GetObv(hm, 14);
        }

        [Benchmark]
        public object GetParabolicSar()
        {
            return Indicator.GetParabolicSar(hm);
        }

        [Benchmark]
        public object GetPmo()
        {
            return Indicator.GetPmo(hm);
        }

        [Benchmark]
        public object GetPrs()
        {
            return Indicator.GetPrs(hm, ho);
        }

        [Benchmark]
        public object GetPrsWithSma()
        {
            return Indicator.GetPrs(hm, ho, 14);
        }

        [Benchmark]
        public object GetRoc()
        {
            return Indicator.GetRoc(hm, 20);
        }

        [Benchmark]
        public object GetRocWithSma()
        {
            return Indicator.GetRoc(hm, 20, 14);
        }

        [Benchmark]
        public object GetRsi()
        {
            return Indicator.GetRsi(hm);
        }

        [Benchmark]
        public object GetSlope()
        {
            return Indicator.GetSlope(hm, 20);
        }

        [Benchmark]
        public object GetSma()
        {
            return Indicator.GetSma(hm, 10);
        }

        [Benchmark]
        public object GetSmaExtended()
        {
            return Indicator.GetSma(hm, 10, true);
        }

        [Benchmark]
        public object GetStdDev()
        {
            return Indicator.GetStdDev(hm, 20);
        }

        [Benchmark]
        public object GetStdDevWithSma()
        {
            return Indicator.GetStdDev(hm, 20, 14);
        }

        [Benchmark]
        public object GetStoch()
        {
            return Indicator.GetStoch(hm);
        }

        [Benchmark]
        public object GetStochRsi()
        {
            return Indicator.GetStochRsi(hm, 14, 14, 3);
        }

        [Benchmark]
        public object GetTripleEma()
        {
            return Indicator.GetTripleEma(hm, 14);
        }

        [Benchmark]
        public object GetUlcerIndex()
        {
            return Indicator.GetUlcerIndex(hm);
        }

        [Benchmark]
        public object GetVolSma()
        {
            return Indicator.GetVolSma(hm, 14);
        }

        [Benchmark]
        public object GetWilliamR()
        {
            return Indicator.GetWilliamR(hm);
        }

        [Benchmark]
        public object GetWma()
        {
            return Indicator.GetWma(hm, 14);
        }

        [Benchmark]
        public object GetZigZag()
        {
            return Indicator.GetZigZag(hm);
        }
    }
}
