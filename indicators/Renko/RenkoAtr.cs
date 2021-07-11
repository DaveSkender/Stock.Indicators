using System.Collections.Generic;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // RENKO CHART (ATR VARIANT)
        /// <include file='./info.xml' path='indicators/type[@name="atr"]/*' />
        /// 
        public static IEnumerable<RenkoResult> GetRenkoAtr<TQuote>(
            this IEnumerable<TQuote> quotes,
            int atrPeriods,
            EndType endType = EndType.Close)
            where TQuote : IQuote
        {

            // TODO: get atr using last N+100 periods of quotes

            // mock ATR
            decimal brickSize = atrPeriods / 2.5m;

            return quotes.GetRenko(brickSize, endType);
        }
    }
}
