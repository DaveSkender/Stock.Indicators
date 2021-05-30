using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Internal.Tests;
using Skender.Stock.Indicators;

namespace Tests.Performance
{

    [MarkdownExporterAttribute.GitHub]
    public class MarkFunctions
    {
        private double[] values;

        [Params(20, 50, 250, 1000)]
        public int Periods;

        [GlobalSetup]
        public void Setup()
        {
            values = HistoryTestData.GetLong(Periods)
                .Select(x => (double)x.Close)
                .ToArray();
        }


        [Benchmark]
        public object StdDev()
        {
            return Functions.StdDev(values);
        }
    }


    [MarkdownExporterAttribute.GitHub]
    public class MarkHistoryHelpers
    {
        private static readonly IEnumerable<Quote> h = HistoryTestData.Get();
        private static readonly IEnumerable<Quote> i = HistoryTestData.GetIntraday();

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
    }
}
