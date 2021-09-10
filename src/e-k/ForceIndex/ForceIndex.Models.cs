using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class ForceIndexResult : ResultBase
    {
        public decimal? ForceIndex { get; set; }
    }
}
