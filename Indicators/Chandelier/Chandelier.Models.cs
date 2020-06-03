using System;

namespace Skender.Stock.Indicators
{

    public class ChandelierResult
    {
        public int Index { get; set; }
        public DateTime Date { get; set; }
        public decimal? ChandelierExit { get; set; }
        public bool? IsExitCross { get; set; }
        public bool? IsCrossed { get; set; }
    }

}
