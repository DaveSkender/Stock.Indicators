using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class FisherTransformResult : ResultBase
    {
        public decimal? Fisher { get; set; }
        public decimal? Trigger { get; set; }
    }

}
