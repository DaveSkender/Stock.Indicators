using System;

namespace Skender.Stock.Indicators
{

    public class MacdResult
    {
        public DateTime Date { get; internal set; }
        public decimal? Macd { get; internal set; }
        public decimal? Signal { get; internal set; }
        public decimal? Histogram { get; internal set; }
        public bool? IsBullish { get; internal set; }
        public bool? IsDiverging { get; internal set; }
    }

}
