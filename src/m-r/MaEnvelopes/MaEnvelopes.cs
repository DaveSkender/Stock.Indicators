using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // MOVING AVERAGE ENVELOPES
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<MaEnvelopeResult> GetMaEnvelopes<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods,
            double percentOffset = 2.5,
            MaType movingAverageType = MaType.SMA)
            where TQuote : IQuote
        {

            // check parameter arguments
            // note: most validations are done in variant methods
            ValidateMaEnvelopes(percentOffset);

            // initialize
            decimal offsetRatio = (decimal)percentOffset / 100m;

            // get envelopes variant
            return movingAverageType switch
            {
                MaType.ALMA => quotes.MaEnvAlma(lookbackPeriods, offsetRatio),
                MaType.DEMA => quotes.MaEnvDema(lookbackPeriods, offsetRatio),
                MaType.EMA => quotes.MaEnvEma(lookbackPeriods, offsetRatio),
                MaType.EPMA => quotes.MaEnvEpma(lookbackPeriods, offsetRatio),
                MaType.HMA => quotes.MaEnvHma(lookbackPeriods, offsetRatio),
                MaType.SMA => quotes.MaEnvSma(lookbackPeriods, offsetRatio),
                MaType.SMMA => quotes.MaEnvSmma(lookbackPeriods, offsetRatio),
                MaType.TEMA => quotes.MaEnvTema(lookbackPeriods, offsetRatio),
                MaType.WMA => quotes.MaEnvWma(lookbackPeriods, offsetRatio),

                _ => throw new ArgumentOutOfRangeException(
                         nameof(movingAverageType), movingAverageType,
                         string.Format(EnglishCulture,
                         "Moving Average Envelopes does not support {0}.",
                         Enum.GetName(typeof(MaType), movingAverageType)))
            };
        }

        private static IEnumerable<MaEnvelopeResult> MaEnvAlma<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods,
            decimal offsetRatio)
            where TQuote : IQuote
        {
            return quotes.GetAlma(lookbackPeriods)
            .Select(x => new MaEnvelopeResult
            {
                Date = x.Date,
                Centerline = x.Alma,
                UpperEnvelope = x.Alma + x.Alma * offsetRatio,
                LowerEnvelope = x.Alma - x.Alma * offsetRatio
            });
        }

        private static IEnumerable<MaEnvelopeResult> MaEnvDema<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods,
            decimal offsetRatio)
            where TQuote : IQuote
        {
            return quotes.GetDoubleEma(lookbackPeriods)
            .Select(x => new MaEnvelopeResult
            {
                Date = x.Date,
                Centerline = x.Dema,
                UpperEnvelope = x.Dema + x.Dema * offsetRatio,
                LowerEnvelope = x.Dema - x.Dema * offsetRatio
            });
        }

        private static IEnumerable<MaEnvelopeResult> MaEnvEma<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods,
            decimal offsetRatio)
            where TQuote : IQuote
        {
            return quotes.GetEma(lookbackPeriods)
            .Select(x => new MaEnvelopeResult
            {
                Date = x.Date,
                Centerline = x.Ema,
                UpperEnvelope = x.Ema + x.Ema * offsetRatio,
                LowerEnvelope = x.Ema - x.Ema * offsetRatio
            });
        }

        private static IEnumerable<MaEnvelopeResult> MaEnvEpma<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods,
            decimal offsetRatio)
            where TQuote : IQuote
        {
            return quotes.GetEpma(lookbackPeriods)
            .Select(x => new MaEnvelopeResult
            {
                Date = x.Date,
                Centerline = x.Epma,
                UpperEnvelope = x.Epma + x.Epma * offsetRatio,
                LowerEnvelope = x.Epma - x.Epma * offsetRatio
            });
        }

        private static IEnumerable<MaEnvelopeResult> MaEnvHma<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods,
            decimal offsetRatio)
            where TQuote : IQuote
        {
            return quotes.GetHma(lookbackPeriods)
            .Select(x => new MaEnvelopeResult
            {
                Date = x.Date,
                Centerline = x.Hma,
                UpperEnvelope = x.Hma + x.Hma * offsetRatio,
                LowerEnvelope = x.Hma - x.Hma * offsetRatio
            });
        }

        private static IEnumerable<MaEnvelopeResult> MaEnvSma<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods,
            decimal offsetRatio)
            where TQuote : IQuote
        {
            return quotes.GetSma(lookbackPeriods)
            .Select(x => new MaEnvelopeResult
            {
                Date = x.Date,
                Centerline = x.Sma,
                UpperEnvelope = x.Sma + x.Sma * offsetRatio,
                LowerEnvelope = x.Sma - x.Sma * offsetRatio
            });
        }

        private static IEnumerable<MaEnvelopeResult> MaEnvSmma<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods,
            decimal offsetRatio)
            where TQuote : IQuote
        {
            return quotes.GetSmma(lookbackPeriods)
            .Select(x => new MaEnvelopeResult
            {
                Date = x.Date,
                Centerline = x.Smma,
                UpperEnvelope = x.Smma + x.Smma * offsetRatio,
                LowerEnvelope = x.Smma - x.Smma * offsetRatio
            });
        }

        private static IEnumerable<MaEnvelopeResult> MaEnvTema<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods,
            decimal offsetRatio)
            where TQuote : IQuote
        {
            return quotes.GetTripleEma(lookbackPeriods)
            .Select(x => new MaEnvelopeResult
            {
                Date = x.Date,
                Centerline = x.Tema,
                UpperEnvelope = x.Tema + x.Tema * offsetRatio,
                LowerEnvelope = x.Tema - x.Tema * offsetRatio
            });
        }

        private static IEnumerable<MaEnvelopeResult> MaEnvWma<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods,
            decimal offsetRatio)
            where TQuote : IQuote
        {
            return quotes.GetWma(lookbackPeriods)
            .Select(x => new MaEnvelopeResult
            {
                Date = x.Date,
                Centerline = x.Wma,
                UpperEnvelope = x.Wma + x.Wma * offsetRatio,
                LowerEnvelope = x.Wma - x.Wma * offsetRatio
            });
        }

        // parameter validation
        private static void ValidateMaEnvelopes(
            double percentOffset)
        {

            // check parameter arguments
            if (percentOffset <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(percentOffset), percentOffset,
                    "Percent Offset must be greater than 0 for Moving Average Envelopes.");
            }
        }
    }
}
