namespace Skender.Stock.Indicators
{

    public class BollingerBandsResult : ResultBase
    {
        public decimal? Sma { get; set; }
        public decimal? UpperBand { get; set; }
        public decimal? LowerBand { get; set; }

        public decimal? ZScore { get; set; }
        public decimal? Width { get; set; }
        public bool? IsDiverging { get; set; }
    }

}
