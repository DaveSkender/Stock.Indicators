using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // RENKO CHART (STANDARD)
        /// <include file='./info.xml' path='indicators/type[@name="standard"]/*' />
        /// 
        public static IEnumerable<RenkoResult> GetRenko<TQuote>(
            this IEnumerable<TQuote> quotes,
            decimal brickSize,
            EndType endType = EndType.Close)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> quotesList = quotes.Sort();

            // check parameter arguments
            ValidateRenko(quotes, brickSize);

            // initialize
            int size = quotesList.Count;
            List<RenkoResult> results = new(size);

            int brickIndex = 1;
            int decimals = brickSize.GetDecimalPlaces();
            TQuote init = quotesList[0];

            decimal o = Math.Round(init.Close, Math.Max(decimals - 1, 0));
            decimal h = init.Close;
            decimal l = init.Close;
            decimal v = init.Close;

            // roll through quotes
            for (int i = 1; i < size; i++)
            {
                TQuote q = quotesList[i];

                // accumulate brick info
                if (i == brickIndex)
                {
                    // reset
                    h = q.High;
                    l = q.Low;
                    v = q.Volume;
                }
                else
                {
                    h = q.High > h ? q.High : h;
                    l = q.Low < l ? q.Low : l;
                    v += q.Volume;
                }

                // determine if new brick threshold is met
                // TODO: add High/Low handling
                int newBrickQty = GetNewBricks(endType, q, o, brickSize);
                int absQty = Math.Abs(newBrickQty);

                // add new brick(s)
                if (newBrickQty != 0)
                {
                    // can add more than one brick!
                    for (int b = 0; b < absQty; b++)
                    {
                        decimal brickClose = newBrickQty > 0 ?
                            o + brickSize : o - brickSize;

                        RenkoResult result = new()
                        {
                            Date = q.Date,
                            Open = o,
                            High = h,
                            Low = l,
                            Close = brickClose,
                            Volume = v / absQty,
                            IsUp = (newBrickQty > 0)
                        };
                        results.Add(result);
                        o = brickClose;
                    }

                    // init next brick(s)
                    brickIndex = i + 1;
                }
            }

            return results;
        }

        // convert to quotes
        /// <include file='../_Common/Results/info.xml' path='info/type[@name="Convert"]/*' />
        ///
        public static IEnumerable<Quote> ConvertToQuotes(
            this IEnumerable<RenkoResult> results)
        {
            return results
              .Select(x => new Quote
              {
                  Date = x.Date,
                  Open = x.Open,
                  High = x.High,
                  Low = x.Low,
                  Close = x.Close,
                  Volume = x.Volume
              })
              .ToList();
        }


        // calculate brick size
        private static int GetNewBricks<TQuote>(
            EndType endType,
            TQuote q,
            decimal lastOpen,
            decimal brickSize)
            where TQuote : IQuote
        {
            switch (endType)
            {
                case EndType.Close:

                    return (int)((q.Close - lastOpen) / brickSize);

                case EndType.HighLow:

                    // high/low assumption: absolute greater diff wins
                    // --> does not consider close direction

                    decimal hQty = (q.High - lastOpen) / brickSize;
                    decimal lQty = (q.Low - lastOpen) / brickSize;

                    return (int)(Math.Abs(hQty) >= Math.Abs(lQty) ? hQty : lQty);

                default: return 0;
            }
        }


        // parameter validation
        private static void ValidateRenko<TQuote>(
            IEnumerable<TQuote> quotes,
            decimal brickSize)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (brickSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(brickSize), brickSize,
                    "Brick size must be greater than 0 for Renko Charts.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = 2;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for Renko Chart.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.",
                    qtyHistory, minHistory);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
