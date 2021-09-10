using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class CmfResult : ResultBase
    {
        public decimal MoneyFlowMultiplier { get; set; }
        public decimal MoneyFlowVolume { get; set; }
        public decimal? Cmf { get; set; }
    }
}