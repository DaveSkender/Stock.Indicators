using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class CorrResult : ResultBase
    {
        public double? VarianceA { get; set; }
        public double? VarianceB { get; set; }
        public double? Covariance { get; set; }
        public double? Correlation { get; set; }
        public double? RSquared { get; set; }
    }
}
