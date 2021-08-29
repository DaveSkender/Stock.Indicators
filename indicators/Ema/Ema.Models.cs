using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class EmaResult : ResultBase
    {
        public decimal? Ema { get; set; }
    }

    [Serializable]
    public class TemaResult : ResultBase
    {
        public decimal? Tema { get; set; }
    }
}
