using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class UltimateResult : ResultBase
    {
        public decimal? Ultimate { get; set; }

        // internal use only
        internal decimal? Bp { get; set; }  // buying pressure
        internal decimal? Tr { get; set; }  // true range
    }
}
