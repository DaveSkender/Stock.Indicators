using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class KamaResult : ResultBase
    {
        public decimal? Kama { get; set; }
    }
}