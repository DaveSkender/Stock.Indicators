using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class ChandelierResult : ResultBase
    {
        public decimal? ChandelierExit { get; set; }
    }
}
