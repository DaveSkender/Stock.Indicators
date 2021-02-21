using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class StdDevChannelsResult : ResultBase
    {
        public decimal? Centerline { get; set; }
        public decimal? UpperChannel { get; set; }
        public decimal? LowerChannel { get; set; }
        public bool BreakPoint { get; set; }
    }
}
