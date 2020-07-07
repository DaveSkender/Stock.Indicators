namespace Skender.Stock.Indicators
{

    public class DonchianResult : ResultBase
    {
        public decimal? UpperBand { get; set; }
        public decimal? Centerline { get; set; }
        public decimal? LowerBand { get; set; }
        public decimal? Width { get; set; }
        public bool? IsDiverging { get; set; }
    }

}
