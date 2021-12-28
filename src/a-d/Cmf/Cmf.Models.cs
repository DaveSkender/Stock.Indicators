namespace Skender.Stock.Indicators
{
    [Serializable]
    public class CmfResult : ResultBase
    {
        public double MoneyFlowMultiplier { get; set; }
        public double MoneyFlowVolume { get; set; }
        public double? Cmf { get; set; }
    }
}
