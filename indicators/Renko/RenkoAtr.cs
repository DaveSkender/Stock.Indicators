using System.Collections.Generic;
using System.Linq;

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

            // initialize
            IEnumerable<AtrResult> atrResults = quotes.GetAtr(atrPeriods);
            decimal? brickSize = atrResults.LastOrDefault().Atr;

            return brickSize is null or 0 ?
                new List<RenkoResult>()
              : quotes.GetRenko((decimal)brickSize, endType);
        }
    }
}
