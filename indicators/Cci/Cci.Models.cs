using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class CciResult : ResultBase
    {
        internal decimal? Tp { get; set; }
        public decimal? Cci { get; set; }
    }
}