using System;
using System.Diagnostics.CodeAnalysis;

namespace Skender.Stock.Indicators
{
    [Serializable]
    public class ChandelierResult : ResultBase
    {
        public decimal? ChandelierExit { get; set; }
    }

    [SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "Not a problem")]
    public enum ChandelierType
    {
        Long = 0,
        Short = 1
    }
}