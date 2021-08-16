using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // Divergence (uses GetPivots results)
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        internal static IEnumerable<DivergenceResult> CalcDivergence(
            IEnumerable<PivotsResult> pivotsA,
            IEnumerable<PivotsResult> pivotsB)
        {

            // initialize
            List<PivotsResult> pivotsListA = pivotsA.OrderBy(x => x.Date).ToList();
            List<PivotsResult> pivotsListB = pivotsB.OrderBy(x => x.Date).ToList();
            List<DivergenceResult> results = new(pivotsListA.Count);

            // find starting index for price
            DateTime firstBdate = pivotsListB[0].Date;
            int offset = pivotsListA.IndexOf(pivotsA.Find(firstBdate));

            // check parameter arguments
            ValidateDivergence(pivotsListA, pivotsListB, offset);

            for (int i = 0; i < pivotsListB.Count; i++)
            {
                PivotsResult a = pivotsListA[i + offset];
                PivotsResult b = pivotsListB[i];

                DivergenceResult r = new()
                {
                    Date = a.Date,

                    BullishRegular = (a.LowTrend == PivotTrend.LL
                                   && b.LowTrend == PivotTrend.HL) ? b.LowLine : null,

                    BullishHidden = (a.LowTrend == PivotTrend.HL
                                  && b.LowTrend == PivotTrend.LL) ? b.LowLine : null,

                    BearishRegular = (a.HighTrend == PivotTrend.HH
                                   && b.HighTrend == PivotTrend.LH) ? b.HighLine : null,

                    BearishHidden = (a.HighTrend == PivotTrend.LH
                                  && b.HighTrend == PivotTrend.HH) ? b.HighLine : null,
                };
                results.Add(r);
            }

            return results;
        }

        // parameter validation
        private static void ValidateDivergence(
            List<PivotsResult> pivotsA,
            List<PivotsResult> pivotsB,
            int offset)
        {
            string unMatchedMessage
                = "Primary and Comparison Pivots dates are unmatched for Divergence.";

            // check matching length
            if (pivotsA.Count - offset != pivotsB.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(pivotsB), pivotsB, unMatchedMessage);
            }

            // check matching dates
            else
            {
                for (int i = 0; i < pivotsB.Count; i++)
                {
                    if (pivotsA[i + offset].Date != pivotsB[i].Date)
                    {
                        throw new BadQuotesException(nameof(pivotsB), pivotsB, unMatchedMessage);
                    }
                }
            }
        }
    }
}
