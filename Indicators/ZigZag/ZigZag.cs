using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // ZIG ZAG
        public static IEnumerable<ZigZagResult> GetZigZag(
            IEnumerable<Quote> history, ZigZagType type = ZigZagType.Close, decimal percentChange = 5)
        {

            // clean quotes
            history = Cleaners.PrepareHistory(history);

            // check parameters
            ValidateZigZag(history, percentChange);

            // initialize
            List<ZigZagResult> results = new List<ZigZagResult>();

            bool? trendUp = null;  // confirmed trend -> null: unknown, true: up, false: down
            decimal changeThreshold = percentChange / 100m;

            int lastPointIndex = 1;
            int lastHighIndex = lastPointIndex;
            int lastLowIndex = lastPointIndex;
            int confPointIndex = lastPointIndex;

            decimal lastPointValue = history.First().Close;
            decimal lastHighValue = lastPointValue;
            decimal lastLowValue = lastPointValue;
            decimal confPointValue = lastPointValue;

            // TODO: temporary type gate, add conditionals
            if (type != ZigZagType.Close)
            {
                return results;
            }

            // roll through history
            foreach (Quote h in history)
            {

                ZigZagResult result = new ZigZagResult
                {
                    Index = (int)h.Index,
                    Date = h.Date
                };

                // TODO: consider `type`
                decimal currentChange = (h.Close - confPointValue) / confPointValue;

                switch (trendUp)
                {
                    // unknown trend
                    case null:

                        // confirm initial change
                        if (Math.Abs(currentChange) >= changeThreshold)
                        {
                            trendUp = (currentChange >= 0);
                            confPointIndex = (int)h.Index;
                            confPointValue = h.Close;
                        }

                        break;

                    // confirmed up
                    case true:

                        // continued upward trend
                        // reset confPoint/Index to new high
                        if (h.Close > confPointValue)
                        {
                            confPointIndex = (int)h.Index;
                            confPointValue = h.Close;
                        }

                        // reversed trend
                        // set lastPoint/Index and new trend
                        else if (currentChange <= -changeThreshold)
                        {

                            // add in-between ZigZag line values for prior segment
                            if (confPointIndex > lastPointIndex)
                            {
                                IEnumerable<ZigZagResult> line = results
                                    .Where(x => x.Index > lastPointIndex && x.Index < confPointIndex);

                                decimal increment = (confPointValue - lastPointValue) / (confPointIndex - lastPointIndex);

                                foreach (ZigZagResult l in line)
                                {
                                    l.ZigZag = lastPointValue + increment * (l.Index - lastPointIndex);
                                }
                            }

                            // add in-between RetraceHigh line values
                            if (confPointIndex > lastHighIndex && lastHighIndex > 1)
                            {
                                IEnumerable<ZigZagResult> highline = results
                                    .Where(x => x.Index > lastHighIndex && x.Index < confPointIndex);

                                decimal increment = (confPointValue - lastHighValue) / (confPointIndex - lastHighIndex);

                                foreach (ZigZagResult l in highline)
                                {
                                    l.RetraceHigh = lastHighValue + increment * (l.Index - lastHighIndex);
                                }
                            }

                            // set last point
                            lastPointIndex = confPointIndex;
                            lastPointValue = confPointValue;

                            lastHighIndex = confPointIndex;
                            lastHighValue = confPointValue;

                            ZigZagResult lasthigh = results
                                .Where(x => x.Index == lastPointIndex)
                                .FirstOrDefault();

                            lasthigh.PointType = "H";
                            lasthigh.ZigZag = lastPointValue;
                            lasthigh.RetraceHigh = lastPointValue;
                            trendUp = false;
                        }

                        break;

                    // confirmed down
                    case false:

                        // continued downward trend
                        // reset confPoint/Index to new low
                        if (h.Close < confPointValue)
                        {
                            confPointIndex = (int)h.Index;
                            confPointValue = h.Close;
                        }

                        // reversed trend
                        // set lastPoint/Index and new trend
                        else if (currentChange >= changeThreshold)
                        {

                            // add in-between ZigZag line values for prior segment
                            if (confPointIndex > lastPointIndex)
                            {
                                IEnumerable<ZigZagResult> line = results
                                    .Where(x => x.Index > lastPointIndex && x.Index < confPointIndex);

                                decimal increment = (confPointValue - lastPointValue) / (confPointIndex - lastPointIndex);

                                foreach (ZigZagResult l in line)
                                {
                                    l.ZigZag = lastPointValue + increment * (l.Index - lastPointIndex);
                                }
                            }

                            // add in-between RetraceLow line values
                            if (confPointIndex > lastLowIndex && lastLowIndex > 1)
                            {
                                IEnumerable<ZigZagResult> lowline = results
                                    .Where(x => x.Index > lastLowIndex && x.Index < confPointIndex);

                                decimal increment = (confPointValue - lastLowValue) / (confPointIndex - lastLowIndex);

                                foreach (ZigZagResult l in lowline)
                                {
                                    l.RetraceLow = lastLowValue + increment * (l.Index - lastLowIndex);
                                }
                            }

                            // set last point
                            lastPointIndex = confPointIndex;
                            lastPointValue = confPointValue;

                            lastLowIndex = confPointIndex;
                            lastLowValue = confPointValue;

                            ZigZagResult lastlow = results
                                .Where(x => x.Index == lastPointIndex)
                                .FirstOrDefault();

                            lastlow.PointType = "L";
                            lastlow.ZigZag = lastPointValue;
                            lastlow.RetraceLow = lastPointValue;
                            trendUp = true;
                        }

                        break;
                }

                results.Add(result);
            }

            return results;
        }


        private static void ValidateZigZag(IEnumerable<Quote> history, decimal percentChange)
        {

            // check parameters
            if (percentChange <= 0)
            {
                throw new BadParameterException("Percent change must be greater than 0 for ZIGZAG.");
            }

            // check histor
            int qtyHistory = history.Count();
            int minHistory = 2;
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for ZIGZAG.  " +
                        string.Format(cultureProvider,
                        "You provided {0} periods of history when at least {1} is required.",
                        qtyHistory, minHistory));
            }

        }
    }

}
