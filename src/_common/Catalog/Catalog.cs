namespace Skender.Stock.Indicators;

/// <summary>
/// Provides a catalog of all registered indicators.
/// </summary>
/// <remarks>The <see cref="IndicatorCatalog"/> class maintains a collection of indicators that can be accessed
/// through the <see cref="Catalog"/> property. This class is static and cannot be instantiated.</remarks>
public static partial class IndicatorCatalog
{
    private static readonly List<IndicatorListing> _catalog = [];
    private static readonly object _lock = new();
    private static bool _initialized;

    /// <summary>
    /// Catalog of all indicators
    /// </summary>
    public static IReadOnlyList<IndicatorListing> Catalog
    {
        get
        {
            EnsureInitialized();
            return _catalog;
        }
    }

    /// <summary>
    /// Initialize the catalog with all available indicator listings
    /// </summary>
    private static void EnsureInitialized()
    {
        if (!_initialized)
        {
            lock (_lock)
            {
                if (!_initialized)
                {
                    PopulateCatalog();
                    _initialized = true;
                }
            }
        }
    }

    /// <summary>
    /// Clear the catalog (for testing purposes)
    /// </summary>
    internal static void Clear()
    {
        lock (_lock)
        {
            _catalog.Clear();
            _initialized = false;
        }
    }

    /// <summary>
    /// Populate the catalog with all indicator listings
    /// </summary>
    private static void PopulateCatalog()
    {
        // ADL (Accumulation Distribution Line)
        _catalog.Add(Adl.SeriesListing);
        _catalog.Add(Adl.StreamListing);

        // ADX (Average Directional Index)
        _catalog.Add(Adx.BufferListing);
        _catalog.Add(Adx.SeriesListing);

        // Alligator
        _catalog.Add(Alligator.SeriesListing);
        _catalog.Add(Alligator.StreamListing);

        // ALMA (Arnaud Legoux Moving Average)
        _catalog.Add(Alma.SeriesListing);

        // Aroon
        _catalog.Add(Aroon.SeriesListing);

        // ATR (Average True Range)
        _catalog.Add(Atr.SeriesListing);
        _catalog.Add(Atr.StreamListing);

        // ATR Stop
        _catalog.Add(AtrStop.SeriesListing);
        _catalog.Add(AtrStop.StreamListing);

        // Awesome Oscillator
        _catalog.Add(Awesome.SeriesListing);

        // Beta
        _catalog.Add(Beta.SeriesListing);

        // Bollinger Bands
        _catalog.Add(BollingerBands.SeriesListing);

        // BOP (Balance of Power)
        _catalog.Add(Bop.SeriesListing);

        // CCI (Commodity Channel Index)
        _catalog.Add(Cci.SeriesListing);

        // Chaikin Oscillator
        _catalog.Add(ChaikinOsc.SeriesListing);

        // Chandelier Exit
        _catalog.Add(Chandelier.SeriesListing);

        // CHOP (Choppiness Index)
        _catalog.Add(Chop.SeriesListing);

        // CMF (Chaikin Money Flow)
        _catalog.Add(Cmf.SeriesListing);

        // CMO (Chande Momentum Oscillator)
        _catalog.Add(Cmo.SeriesListing);

        // ConnorsRSI
        _catalog.Add(ConnorsRsi.SeriesListing);

        // Correlation
        _catalog.Add(Correlation.SeriesListing);

        // DEMA (Double Exponential Moving Average)
        _catalog.Add(Dema.SeriesListing);

        // Doji
        _catalog.Add(Doji.SeriesListing);

        // Donchian Channels
        _catalog.Add(Donchian.SeriesListing);

        // DPO (Detrended Price Oscillator)
        _catalog.Add(Dpo.SeriesListing);

        // Elder Ray
        _catalog.Add(ElderRay.SeriesListing);

        // EMA (Exponential Moving Average)
        _catalog.Add(Ema.BufferListing);
        _catalog.Add(Ema.SeriesListing);
        _catalog.Add(Ema.StreamListing);

        // EPMA (Endpoint Moving Average)
        _catalog.Add(Epma.SeriesListing);

        // FCB (Fractal Chaos Bands)
        _catalog.Add(Fcb.SeriesListing);

        // Fisher Transform
        _catalog.Add(FisherTransform.SeriesListing);

        // Force Index
        _catalog.Add(ForceIndex.SeriesListing);

        // Fractal
        _catalog.Add(Fractal.SeriesListing);

        // Gator Oscillator
        _catalog.Add(Gator.SeriesListing);

        // Heikin Ashi
        _catalog.Add(HeikinAshi.SeriesListing);

        // HMA (Hull Moving Average)
        _catalog.Add(Hma.SeriesListing);

        // HT Trendline (Hilbert Transform)
        _catalog.Add(HtTrendline.SeriesListing);

        // Hurst Exponent
        _catalog.Add(Hurst.SeriesListing);

        // Ichimoku Cloud
        _catalog.Add(Ichimoku.SeriesListing);

        // KAMA (Kaufman Adaptive Moving Average)
        _catalog.Add(Kama.SeriesListing);

        // Keltner Channels
        _catalog.Add(Keltner.SeriesListing);

        // KVO (Klinger Volume Oscillator)
        _catalog.Add(Kvo.SeriesListing);

        // MA Envelopes (Moving Average Envelopes)
        _catalog.Add(MaEnvelopes.SeriesListing);

        // MACD (Moving Average Convergence Divergence)
        _catalog.Add(Macd.SeriesListing);

        // MAMA (MESA Adaptive Moving Average)
        _catalog.Add(Mama.SeriesListing);

        // Marubozu
        _catalog.Add(Marubozu.SeriesListing);

        // MFI (Money Flow Index)
        _catalog.Add(Mfi.SeriesListing);

        // McGinley Dynamic
        _catalog.Add(MgDynamic.SeriesListing);

        // OBV (On Balance Volume)
        _catalog.Add(Obv.SeriesListing);

        // Parabolic SAR
        _catalog.Add(ParabolicSar.SeriesListing);

        // Pivot Points
        _catalog.Add(PivotPoints.SeriesListing);

        // Pivots
        _catalog.Add(Pivots.SeriesListing);

        // PMO (Price Momentum Oscillator)
        _catalog.Add(Pmo.SeriesListing);

        // PRS (Price Relative Strength)
        _catalog.Add(Prs.SeriesListing);

        // PVO (Price Volume Oscillator)
        _catalog.Add(Pvo.SeriesListing);

        // Renko
        _catalog.Add(Renko.SeriesListing);
        _catalog.Add(Renko.StreamListing);

        // Renko ATR
        _catalog.Add(RenkoAtr.SeriesListing);

        // ROC (Rate of Change)
        _catalog.Add(Roc.SeriesListing);

        // ROC with Bands
        _catalog.Add(RocWb.SeriesListing);

        // Rolling Pivots
        _catalog.Add(RollingPivots.SeriesListing);

        // RSI (Relative Strength Index)
        _catalog.Add(Rsi.SeriesListing);

        // Slope
        _catalog.Add(Slope.SeriesListing);

        // SMA (Simple Moving Average)
        _catalog.Add(Sma.SeriesListing);
        _catalog.Add(Sma.StreamListing);

        // SMA Analysis
        _catalog.Add(SmaAnalysis.SeriesListing);

        // SMI (Stochastic Momentum Index)
        _catalog.Add(Smi.SeriesListing);

        // SMMA (Smoothed Moving Average)
        _catalog.Add(Smma.SeriesListing);

        // STARC Bands
        _catalog.Add(StarcBands.SeriesListing);

        // STC (Schaff Trend Cycle)
        _catalog.Add(Stc.SeriesListing);

        // Standard Deviation
        _catalog.Add(StdDev.SeriesListing);

        // Standard Deviation Channels
        _catalog.Add(StdDevChannels.SeriesListing);

        // Stochastic Oscillator
        _catalog.Add(Stoch.SeriesListing);

        // Stochastic RSI
        _catalog.Add(StochRsi.SeriesListing);

        // SuperTrend
        _catalog.Add(SuperTrend.SeriesListing);

        // T3 (Triple Exponential Moving Average)
        _catalog.Add(T3.SeriesListing);

        // TEMA (Triple Exponential Moving Average)
        _catalog.Add(Tema.SeriesListing);

        // TR (True Range)
        _catalog.Add(Tr.SeriesListing);
        _catalog.Add(Tr.StreamListing);

        // TRIX
        _catalog.Add(Trix.SeriesListing);

        // TSI (True Strength Index)
        _catalog.Add(Tsi.SeriesListing);

        // Ulcer Index
        _catalog.Add(UlcerIndex.SeriesListing);

        // Ultimate Oscillator
        _catalog.Add(Ultimate.SeriesListing);

        // Volatility Stop
        _catalog.Add(VolatilityStop.SeriesListing);

        // Vortex Indicator
        _catalog.Add(Vortex.SeriesListing);

        // VWAP (Volume Weighted Average Price)
        _catalog.Add(Vwap.SeriesListing);

        // VWMA (Volume Weighted Moving Average)
        _catalog.Add(Vwma.SeriesListing);

        // Williams %R
        _catalog.Add(WilliamsR.SeriesListing);

        // WMA (Weighted Moving Average)
        _catalog.Add(Wma.SeriesListing);

        // ZigZag
        _catalog.Add(ZigZag.SeriesListing);
    }
}
