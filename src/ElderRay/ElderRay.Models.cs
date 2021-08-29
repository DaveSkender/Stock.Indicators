using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class ElderRayResult : ResultBase
    {
        public decimal? Ema { get; set; }
        public decimal? BullPower { get; set; }
        public decimal? BearPower { get; set; }
    }
}
