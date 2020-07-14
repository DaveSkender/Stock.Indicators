using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class UlcerIndexResult : ResultBase
    {
        public decimal? UI { get; internal set; }
    }
}
