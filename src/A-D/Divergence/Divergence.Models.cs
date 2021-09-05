using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class DivergenceResult : ResultBase
    {
        public decimal? BullishRegular { get; set; }
        public decimal? BullishHidden { get; set; }
        public decimal? BearishRegular { get; set; }
        public decimal? BearishHidden { get; set; }
        public PivotsResult PivotsA { get; set; }
        public PivotsResult PivotsB { get; set; }
    }
}
