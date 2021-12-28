namespace Skender.Stock.Indicators
{
    [Serializable]
    public class RocResult : ResultBase
    {
        public double? Roc { get; set; }
        public double? RocSma { get; set; }
    }

    [Serializable]
    public class RocWbResult : ResultBase
    {
        public double? Roc { get; set; }
        public double? RocEma { get; set; }
        public double? UpperBand { get; set; }
        public double? LowerBand { get; set; }
    }
}
