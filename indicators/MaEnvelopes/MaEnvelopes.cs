﻿using System;
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
                MaType.ALMA => GetAlma(quotes, lookbackPeriods)
                    .Select(x => new MaEnvelopeResult
                    {
                        Date = x.Date,
                        Centerline = x.Alma,
                        UpperEnvelope = x.Alma + x.Alma * offsetRatio,
                        LowerEnvelope = x.Alma - x.Alma * offsetRatio
                    }),

                MaType.DEMA => GetDoubleEma(quotes, lookbackPeriods)
                    .Select(x => new MaEnvelopeResult
                    {
                        Date = x.Date,
                        Centerline = x.Dema,
                        UpperEnvelope = x.Dema + x.Dema * offsetRatio,
                        LowerEnvelope = x.Dema - x.Dema * offsetRatio
                    }),

                MaType.EPMA => GetEpma(quotes, lookbackPeriods)
                    .Select(x => new MaEnvelopeResult
                    {
                        Date = x.Date,
                        Centerline = x.Epma,
                        UpperEnvelope = x.Epma + x.Epma * offsetRatio,
                        LowerEnvelope = x.Epma - x.Epma * offsetRatio
                    }),

                MaType.EMA => GetEma(quotes, lookbackPeriods)
                    .Select(x => new MaEnvelopeResult
                    {
                        Date = x.Date,
                        Centerline = x.Ema,
                        UpperEnvelope = x.Ema + x.Ema * offsetRatio,
                        LowerEnvelope = x.Ema - x.Ema * offsetRatio
                    }),

                MaType.HMA => GetHma(quotes, lookbackPeriods)
                    .Select(x => new MaEnvelopeResult
                    {
                        Date = x.Date,
                        Centerline = x.Hma,
                        UpperEnvelope = x.Hma + x.Hma * offsetRatio,
                        LowerEnvelope = x.Hma - x.Hma * offsetRatio
                    }),

                MaType.SMA => GetSma(quotes, lookbackPeriods)
                    .Select(x => new MaEnvelopeResult
                    {
                        Date = x.Date,
                        Centerline = x.Sma,
                        UpperEnvelope = x.Sma + x.Sma * offsetRatio,
                        LowerEnvelope = x.Sma - x.Sma * offsetRatio
                    }),

                MaType.TEMA => GetTripleEma(quotes, lookbackPeriods)
                    .Select(x => new MaEnvelopeResult
                    {
                        Date = x.Date,
                        Centerline = x.Tema,
                        UpperEnvelope = x.Tema + x.Tema * offsetRatio,
                        LowerEnvelope = x.Tema - x.Tema * offsetRatio
                    }),

                MaType.WMA => GetWma(quotes, lookbackPeriods)
                    .Select(x => new MaEnvelopeResult
                    {
                        Date = x.Date,
                        Centerline = x.Wma,
                        UpperEnvelope = x.Wma + x.Wma * offsetRatio,
                        LowerEnvelope = x.Wma - x.Wma * offsetRatio
                    }),

                _ => throw new ArgumentOutOfRangeException(
                         nameof(movingAverageType), movingAverageType,
                         string.Format(
                             EnglishCulture,
                         "Moving Average Envelopes does not support {0}.",
                         Enum.GetName(typeof(MaType), movingAverageType)))
            };
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
