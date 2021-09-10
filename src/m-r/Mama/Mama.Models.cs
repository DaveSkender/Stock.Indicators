using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class MamaResult : ResultBase
    {
        public decimal? Mama { get; set; }
        public decimal? Fama { get; set; }
    }
}