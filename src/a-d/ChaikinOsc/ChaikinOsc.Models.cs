using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class ChaikinOscResult : ResultBase
    {
        public decimal MoneyFlowMultiplier { get; set; }
        public decimal MoneyFlowVolume { get; set; }
        public decimal Adl { get; set; }
        public decimal? Oscillator { get; set; }
    }
}