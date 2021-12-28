namespace Skender.Stock.Indicators
{
    [Serializable]
    public class AdxResult : ResultBase
    {
        public double? Pdi { get; set; }
        public double? Mdi { get; set; }
        public double? Adx { get; set; }
    }
}
