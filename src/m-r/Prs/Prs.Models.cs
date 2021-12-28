namespace Skender.Stock.Indicators
{
    [Serializable]
    public class PrsResult : ResultBase
    {
        public double? Prs { get; set; }
        public double? PrsSma { get; set; }
        public double? PrsPercent { get; set; }
    }
}
