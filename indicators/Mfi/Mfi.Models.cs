using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class MfiResult : ResultBase
    {
        public decimal? Mfi { get; set; }

        // internal use only
        internal decimal TruePrice { get; set; }
        internal int Direction { get; set; }
        internal decimal RawMF { get; set; }
    }
}