using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Internal.Tests;
using Skender.Stock.Indicators;

namespace Tests.Performance
{
    [MarkdownExporterAttribute.GitHub]
    public class MarkIndicators
    {
        private readonly IEnumerable<Quote> h = HistoryTestData.Get();
        private readonly IEnumerable<Quote> ho = HistoryTestData.GetCompare();
        private readonly IEnumerable<Quote> hday = HistoryTestData.GetIntraday(391);

        [Benchmark]
        public object GetAdl()
        {
            return Indicator.GetAdl(h);
        }

        [Benchmark]
        public object GetAdlWithSma()
        {
            return Indicator.GetAdl(h, 14);
        }

        [Benchmark]
        public object GetAdx()
        {
            return Indicator.GetAdx(h);
        }

        [Benchmark]
        public object GetAlligator()
        {
            return Indicator.GetAlligator(h);
        }

        [Benchmark]
        public object GetAlma()
        {
            return Indicator.GetAlma(h);
        }

        [Benchmark]
        public object GetAroon()
        {
            return Indicator.GetAroon(h);
        }

        [Benchmark]
        public object GetAtr()
        {
            return Indicator.GetAtr(h);
        }

        [Benchmark]
        public object GetAwesome()
        {
            return Indicator.GetAwesome(h);
        }

        [Benchmark]
        public object GetBeta()
        {
            return Indicator.GetBeta(h, ho, 20);
        }

        [Benchmark]
        public object GetBollingerBands()
        {
            return Indicator.GetBollingerBands(h);
        }

        [Benchmark]
        public object GetBop()
        {
            return Indicator.GetBop(h);
        }

        [Benchmark]
        public object GetCci()
        {
            return Indicator.GetCci(h);
        }

        [Benchmark]
        public object GetChaikinOsc()
        {
            return Indicator.GetChaikinOsc(h);
        }

        [Benchmark]
        public object GetChandelier()
        {
            return Indicator.GetChandelier(h);
        }

        [Benchmark]
        public object GetChop()
        {
            return Indicator.GetChop(h);
        }

        [Benchmark]
        public object GetCmf()
        {
            return Indicator.GetCmf(h);
        }

        [Benchmark]
        public object GetConnorsRsi()
        {
            return Indicator.GetConnorsRsi(h);
        }

        [Benchmark]
        public object GetCorrelation()
        {
            return Indicator.GetCorrelation(h, ho, 20);
        }

        [Benchmark]
        public object GetDonchian()
        {
            return Indicator.GetDonchian(h);
        }

        [Benchmark]
        public object GetDoubleEma()
        {
            return Indicator.GetDoubleEma(h, 14);
        }

        [Benchmark]
        public object GetElderRay()
        {
            return Indicator.GetElderRay(h);
        }

        [Benchmark]
        public object GetEma()
        {
            return Indicator.GetEma(h, 14);
        }

        [Benchmark]
        public object GetEpma()
        {
            return Indicator.GetEpma(h, 14);
        }

        [Benchmark]
        public object GetFcb()
        {
            return Indicator.GetFcb(h, 14);
        }

        [Benchmark]
        public object GetForceIndex()
        {
            return Indicator.GetForceIndex(h, 13);
        }

        [Benchmark]
        public object GetFractal()
        {
            return Indicator.GetFractal(h);
        }

        [Benchmark]
        public object GetGator()
        {
            return Indicator.GetGator(h);
        }

        [Benchmark]
        public object GetHeikinAshi()
        {
            return Indicator.GetHeikinAshi(h);
        }

        [Benchmark]
        public object GetHma()
        {
            return Indicator.GetHma(h, 14);
        }

        [Benchmark]
        public object GetHtTrendline()
        {
            return Indicator.GetHtTrendline(h);
        }

        [Benchmark]
        public object GetIchimoku()
        {
            return Indicator.GetIchimoku(h);
        }

        [Benchmark]
        public object GetKama()
        {
            return Indicator.GetKama(h);
        }

        [Benchmark]
        public object GetKeltner()
        {
            return Indicator.GetKeltner(h);
        }

        [Benchmark]
        public object GetMacd()
        {
            return Indicator.GetMacd(h);
        }

        [Benchmark]
        public object GetMaEnvelopes()
        {
            return Indicator.GetMaEnvelopes(h, 20, 2.5, MaType.SMA);
        }

        [Benchmark]
        public object GetMama()
        {
            return Indicator.GetMama(h);
        }

        [Benchmark]
        public object GetMfi()
        {
            return Indicator.GetMfi(h);
        }

        [Benchmark]
        public object GetObv()
        {
            return Indicator.GetObv(h);
        }

        [Benchmark]
        public object GetObvWithSma()
        {
            return Indicator.GetObv(h, 14);
        }

        [Benchmark]
        public object GetParabolicSar()
        {
            return Indicator.GetParabolicSar(h);
        }

        [Benchmark]
        public object GetPivotPoints()
        {
            return Indicator.GetPivotPoints(h, PeriodSize.Month, PivotPointType.Standard);
        }

        [Benchmark]
        public object GetPmo()
        {
            return Indicator.GetPmo(h);
        }

        [Benchmark]
        public object GetPrs()
        {
            return Indicator.GetPrs(h, ho);
        }

        [Benchmark]
        public object GetPrsWithSma()
        {
            return Indicator.GetPrs(h, ho, null, 5);
        }

        [Benchmark]
        public object GetPvo()
        {
            return Indicator.GetPvo(h);
        }

        [Benchmark]
        public object GetRoc()
        {
            return Indicator.GetRoc(h, 20);
        }

        [Benchmark]
        public object GetRocWithSma()
        {
            return Indicator.GetRoc(h, 20, 14);
        }

        [Benchmark]
        public object GetRsi()
        {
            return Indicator.GetRsi(h);
        }

        [Benchmark]
        public object GetSlope()
        {
            return Indicator.GetSlope(h, 20);
        }

        [Benchmark]
        public object GetSma()
        {
            return Indicator.GetSma(h, 10);
        }

        [Benchmark]
        public object GetSmaExtended()
        {
            return Indicator.GetSmaExtended(h, 10);
        }

        [Benchmark]
        public object GetSmma()
        {
            return Indicator.GetSmma(h, 10);
        }

        [Benchmark]
        public object GetStarcBands()
        {
            return Indicator.GetStarcBands(h);
        }

        [Benchmark]
        public object GetStdDev()
        {
            return Indicator.GetStdDev(h, 20);
        }

        [Benchmark]
        public object GetStdDevWithSma()
        {
            return Indicator.GetStdDev(h, 20, 14);
        }

        [Benchmark]
        public object GetStdDevChannels()
        {
            return Indicator.GetStdDevChannels(h);
        }

        [Benchmark]
        public object GetStoch()
        {
            return Indicator.GetStoch(h);
        }

        [Benchmark]
        public object GetStochRsi()
        {
            return Indicator.GetStochRsi(h, 14, 14, 3);
        }

        [Benchmark]
        public object GetSuperTrend()
        {
            return Indicator.GetSuperTrend(h);
        }

        [Benchmark]
        public object GetTripleEma()
        {
            return Indicator.GetTripleEma(h, 14);
        }

        [Benchmark]
        public object GetTrix()
        {
            return Indicator.GetTrix(h, 14);
        }

        [Benchmark]
        public object GetTrixWithSma()
        {
            return Indicator.GetTrix(h, 14, 5);
        }

        [Benchmark]
        public object GetTsi()
        {
            return Indicator.GetTsi(h);
        }

        [Benchmark]
        public object GetT3()
        {
            return Indicator.GetT3(h);
        }

        [Benchmark]
        public object GetUlcerIndex()
        {
            return Indicator.GetUlcerIndex(h);
        }

        [Benchmark]
        public object GetUltimate()
        {
            return Indicator.GetUltimate(h);
        }

        [Benchmark]
        public object GetVolSma()
        {
            return Indicator.GetVolSma(h, 14);
        }

        [Benchmark]
        public object GetVortex()
        {
            return Indicator.GetVortex(h, 14);
        }

        [Benchmark]
        public object GetVwap()
        {
            return Indicator.GetVwap(hday);
        }

        [Benchmark]
        public object GetWilliamsR()
        {
            return Indicator.GetWilliamsR(h);
        }

        [Benchmark]
        public object GetWma()
        {
            return Indicator.GetWma(h, 14);
        }

        [Benchmark]
        public object GetZigZag()
        {
            return Indicator.GetZigZag(h);
        }
    }
}
