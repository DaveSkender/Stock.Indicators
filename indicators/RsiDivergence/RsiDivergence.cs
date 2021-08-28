using System;
using System.Collections.Generic;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // RSI DIVERGENCE
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<DivergenceResult> GetRsiDivergence<TQuote>(
            this IEnumerable<TQuote> quotes,
            int rsiPeriods = 14,
            int leftSpan = 2,
            int rightSpan = 2,
            int maxTrendPeriods = 20,
            EndType endType = EndType.HighLow)
            where TQuote : IQuote
        {

            // check parameter arguments
            ValidatePivots(quotes, leftSpan, rightSpan, maxTrendPeriods, "RSI Divergence");

            // initialize
            IEnumerable<PivotsResult> pricePivots = quotes
                .GetPivots(leftSpan, rightSpan, maxTrendPeriods, endType);

            IEnumerable<PivotsResult> rsiPivots = quotes
                .GetRsi(rsiPeriods)
                .ConvertToQuotes()
                .GetPivots(leftSpan, rightSpan, maxTrendPeriods, endType);

            // calculate
            return CalcDivergence(pricePivots, rsiPivots);
        }
    }
}
