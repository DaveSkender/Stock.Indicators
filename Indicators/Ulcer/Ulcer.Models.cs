using System;

namespace Skender.Stock.Indicators
{

    public class UlcerIndexResult
    {
        public int Index { get; set; }
        public DateTime Date { get; internal set; }
        public decimal? UI { get; internal set; }
    }

}
