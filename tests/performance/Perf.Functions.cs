using BenchmarkDotNet.Attributes;
using Internal.Tests;
using Skender.Stock.Indicators;
using System.Collections.Generic;
using System.Linq;

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
            values = History.GetHistoryLong(Periods)
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
    public class MarkCleaners
    {
        private static readonly IEnumerable<Quote> h = History.GetHistory();
        private static readonly IEnumerable<BasicData> b = Cleaners.ConvertHistoryToBasic(h);

        [Benchmark]
        public object PrepareHistory()
        {
            return Cleaners.PrepareHistory(h);
        }

        [Benchmark]
        public object PrepareBasicData()
        {
            return Cleaners.PrepareBasicData(b);
        }

    }

}
