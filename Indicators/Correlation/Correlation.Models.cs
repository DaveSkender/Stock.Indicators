namespace Skender.Stock.Indicators
{

    public class CorrResult : ResultBase
    {
        public decimal? VarianceA { get; set; }
        public decimal? VarianceB { get; set; }
        public decimal? Covariance { get; set; }
        public decimal? Correlation { get; set; }

        // internal use only
        internal decimal PriceA { get; set; }
        internal decimal PriceB { get; set; }
        internal decimal PriceA2 => PriceA * PriceA;
        internal decimal PriceB2 => PriceB * PriceB;
        internal decimal PriceAB => PriceA * PriceB;
    }

}
