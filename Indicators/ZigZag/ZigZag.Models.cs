using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class ZigZagResult : ResultBase
    {
        public decimal? ZigZag { get; set; }  // zig zag line
    }
}
