using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Internal.Tests;
using Skender.Stock.Indicators;

namespace Tests.Performance
{
    // HELPERS, both public and private

    [MarkdownExporterAttribute.GitHub]
    public class HelperPerformance
    {
        private static IEnumerable<Quote> h;
        private static IEnumerable<Quote> i;
        private static IEnumerable<ObvResult> obv;

        [GlobalSetup]
        public void Setup()
        {
            h = TestData.GetDefault();
        }

        [Benchmark]
        public object Sort()
        {
            return h.Sort();
        }

        [Benchmark]
        public object Validate()
        {
            return h.Validate();
        }

        [GlobalSetup(Targets = new[] { nameof(Aggregate) })]
        public void SetupIntraday()
        {
            i = TestData.GetIntraday();
        }

        [Benchmark]
        public object Aggregate()
        {
            return i.Aggregate(PeriodSize.FifteenMinutes);
        }

        [Benchmark]
        public object ConvertToBasic()
        {
            return h.ConvertToBasic();
        }

        [Benchmark]
        public object ConvertToCandles()
        {
            return h.ConvertToCandles();
        }

        [GlobalSetup(Targets = new[] { nameof(ConvertToQuotes) })]
        public void SetupQuotes()
        {
            h = TestData.GetDefault();
            obv = h.GetObv();
        }

        [Benchmark]
        public object ConvertToQuotes()
        {
            return obv.ConvertToQuotes();
        }
    }
}
