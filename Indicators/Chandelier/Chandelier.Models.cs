namespace Skender.Stock.Indicators
{

    public class ChandelierResult : ResultBase
    {
        public decimal? ChandelierExit { get; set; }
        public bool? IsExitCross { get; set; }
        public bool? IsCrossed { get; set; }
    }

}
