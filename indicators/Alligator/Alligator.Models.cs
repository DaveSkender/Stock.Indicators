using System;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class AlligatorResult : ResultBase
    {
        public decimal? Jaw { get; set; }
        public decimal? Teeth { get; set; }
        public decimal? Lips { get; set; }
    }
}
