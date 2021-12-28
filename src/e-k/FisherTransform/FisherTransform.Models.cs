namespace Skender.Stock.Indicators
{
    [Serializable]
    public class FisherTransformResult : ResultBase
    {
        public double? Fisher { get; set; }
        public double? Trigger { get; set; }
    }

}
