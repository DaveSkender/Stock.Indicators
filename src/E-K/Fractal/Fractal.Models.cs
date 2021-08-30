using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class FractalResult : ResultBase
    {
        public decimal? FractalBear { get; set; }
        public decimal? FractalBull { get; set; }
    }
}