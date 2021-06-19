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
            return h.GetAdl();
        }

        [Benchmark]
        public object GetAdlWithSma()
        {
            return h.GetAdl(14);
        }

        [Benchmark]
        public object GetAdx()
        {
            return h.GetAdx();
        }

        [Benchmark]
        public object GetAlligator()
        {
            return h.GetAlligator();
        }

        [Benchmark]
        public object GetAlma()
        {
            return h.GetAlma();
        }

        [Benchmark]
        public object GetAroon()
        {
            return h.GetAroon();
        }

        [Benchmark]
        public object GetAtr()
        {
            return h.GetAtr();
        }

        [Benchmark]
        public object GetAwesome()
        {
            return h.GetAwesome();
        }

        [Benchmark]
        public object GetBeta()
        {
            return Indicator.GetBeta(h, ho, 20);
        }

        [Benchmark]
        public object GetBollingerBands()
        {
            return h.GetBollingerBands();
        }

        [Benchmark]
        public object GetBop()
        {
            return h.GetBop();
        }

        [Benchmark]
        public object GetCci()
        {
            return h.GetCci();
        }

        [Benchmark]
        public object GetChaikinOsc()
        {
            return h.GetChaikinOsc();
        }

        [Benchmark]
        public object GetChandelier()
        {
            return h.GetChandelier();
        }

        [Benchmark]
        public object GetChop()
        {
            return h.GetChop();
        }

        [Benchmark]
        public object GetCmf()
        {
            return h.GetCmf();
        }

        [Benchmark]
        public object GetConnorsRsi()
        {
            return h.GetConnorsRsi();
        }

        [Benchmark]
        public object GetCorrelation()
        {
            return h.GetCorrelation(ho, 20);
        }

        [Benchmark]
        public object GetDonchian()
        {
            return h.GetDonchian();
        }

        [Benchmark]
        public object GetDoubleEma()
        {
            return h.GetDoubleEma(14);
        }

        [Benchmark]
        public object GetElderRay()
        {
            return h.GetElderRay();
        }

        [Benchmark]
        public object GetEma()
        {
            return h.GetEma(14);
        }

        [Benchmark]
        public object GetEpma()
        {
            return h.GetEpma(14);
        }

        [Benchmark]
        public object GetFcb()
        {
            return h.GetFcb(14);
        }

        [Benchmark]
        public object GetFisherTransform()
        {
            return h.GetFisherTransform(10);
        }

        [Benchmark]
        public object GetForceIndex()
        {
            return h.GetForceIndex(13);
        }

        [Benchmark]
        public object GetFractal()
        {
            return h.GetFractal();
        }

        [Benchmark]
        public object GetGator()
        {
            return h.GetGator();
        }

        [Benchmark]
        public object GetHeikinAshi()
        {
            return h.GetHeikinAshi();
        }

        [Benchmark]
        public object GetHma()
        {
            return h.GetHma(14);
        }

        [Benchmark]
        public object GetHtTrendline()
        {
            return h.GetHtTrendline();
        }

        [Benchmark]
        public object GetIchimoku()
        {
            return h.GetIchimoku();
        }

        [Benchmark]
        public object GetKama()
        {
            return h.GetKama();
        }

        [Benchmark]
        public object GetKlinger()
        {
            return h.GetKvo();
        }

        [Benchmark]
        public object GetKeltner()
        {
            return h.GetKeltner();
        }

        [Benchmark]
        public object GetMacd()
        {
            return h.GetMacd();
        }

        [Benchmark]
        public object GetMaEnvelopes()
        {
            return h.GetMaEnvelopes(20, 2.5, MaType.SMA);
        }

        [Benchmark]
        public object GetMama()
        {
            return h.GetMama();
        }

        [Benchmark]
        public object GetMfi()
        {
            return h.GetMfi();
        }

        [Benchmark]
        public object GetObv()
        {
            return h.GetObv();
        }

        [Benchmark]
        public object GetObvWithSma()
        {
            return h.GetObv(14);
        }

        [Benchmark]
        public object GetParabolicSar()
        {
            return h.GetParabolicSar();
        }

        [Benchmark]
        public object GetPivotPoints()
        {
            return h.GetPivotPoints(PeriodSize.Month, PivotPointType.Standard);
        }

        [Benchmark]
        public object GetPmo()
        {
            return h.GetPmo();
        }

        [Benchmark]
        public object GetPrs()
        {
            return h.GetPrs(ho);
        }

        [Benchmark]
        public object GetPrsWithSma()
        {
            return h.GetPrs(ho, null, 5);
        }

        [Benchmark]
        public object GetPvo()
        {
            return h.GetPvo();
        }

        [Benchmark]
        public object GetRoc()
        {
            return h.GetRoc(20);
        }

        [Benchmark]
        public object GetRocWb()
        {
            return h.GetRocWb(12, 3, 12);
        }

        [Benchmark]
        public object GetRocWithSma()
        {
            return h.GetRoc(20, 14);
        }

        [Benchmark]
        public object GetRsi()
        {
            return h.GetRsi();
        }

        [Benchmark]
        public object GetSlope()
        {
            return h.GetSlope(20);
        }

        [Benchmark]
        public object GetSma()
        {
            return h.GetSma(10);
        }

        [Benchmark]
        public object GetSmaExtended()
        {
            return h.GetSmaExtended(10);
        }

        [Benchmark]
        public object GetSmma()
        {
            return h.GetSmma(10);
        }

        [Benchmark]
        public object GetStarcBands()
        {
            return h.GetStarcBands();
        }

        [Benchmark]
        public object GetStdDev()
        {
            return h.GetStdDev(20);
        }

        [Benchmark]
        public object GetStdDevWithSma()
        {
            return h.GetStdDev(20, 14);
        }

        [Benchmark]
        public object GetStdDevChannels()
        {
            return h.GetStdDevChannels();
        }

        [Benchmark]
        public object GetStoch()
        {
            return h.GetStoch();
        }

        [Benchmark]
        public object GetStochRsi()
        {
            return h.GetStochRsi(14, 14, 3);
        }

        [Benchmark]
        public object GetSuperTrend()
        {
            return h.GetSuperTrend();
        }

        [Benchmark]
        public object GetTripleEma()
        {
            return h.GetTripleEma(14);
        }

        [Benchmark]
        public object GetTrix()
        {
            return h.GetTrix(14);
        }

        [Benchmark]
        public object GetTrixWithSma()
        {
            return h.GetTrix(14, 5);
        }

        [Benchmark]
        public object GetTsi()
        {
            return h.GetTsi();
        }

        [Benchmark]
        public object GetT3()
        {
            return h.GetT3();
        }

        [Benchmark]
        public object GetUlcerIndex()
        {
            return h.GetUlcerIndex();
        }

        [Benchmark]
        public object GetUltimate()
        {
            return h.GetUltimate();
        }

        [Benchmark]
        public object GetVolSma()
        {
            return h.GetVolSma(14);
        }

        [Benchmark]
        public object GetVortex()
        {
            return h.GetVortex(14);
        }

        [Benchmark]
        public object GetVwap()
        {
            return hday.GetVwap();
        }

        [Benchmark]
        public object GetWilliamsR()
        {
            return h.GetWilliamsR();
        }

        [Benchmark]
        public object GetWma()
        {
            return h.GetWma(14);
        }

        [Benchmark]
        public object GetZigZag()
        {
            return h.GetZigZag();
        }
    }
}
