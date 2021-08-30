using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class MfiResult : ResultBase
    {
        public decimal? Mfi { get; set; }
    }
}