namespace Skender.Stock.Indicators
{
    [Serializable]
    public class CciResult : ResultBase
    {
        public double? Cci { get; set; }

        // internal use only
        internal double? Tp { get; set; }
    }
}
