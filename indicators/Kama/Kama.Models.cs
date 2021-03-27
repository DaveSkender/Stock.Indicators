using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class KamaResult : ResultBase
    {
        public decimal? ER { get; set; }
        public decimal? Kama { get; set; }
    }
}
