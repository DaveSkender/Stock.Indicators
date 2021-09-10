using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class MaEnvelopeResult : ResultBase
    {
        public decimal? Centerline { get; set; }
        public decimal? UpperEnvelope { get; set; }
        public decimal? LowerEnvelope { get; set; }
    }
}