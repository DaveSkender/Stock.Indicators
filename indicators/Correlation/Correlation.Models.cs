using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class CorrResult : ResultBase
    {
        public decimal? VarianceA { get; set; }
        public decimal? VarianceB { get; set; }
        public decimal? Covariance { get; set; }
        public decimal? Correlation { get; set; }
        public decimal? RSquared { get; set; }
    }
}