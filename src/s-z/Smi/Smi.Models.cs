using System;

namespace Skender.Stock.Indicators
{
    /// <include file='./info.xml' path='indicator/type[@name="Results"]/*' />
    /// 
    [Serializable]
    public class SmiResult : ResultBase
    {
        public decimal? Smi { get; set; }
        public decimal? Signal { get; set; }
    }
}
