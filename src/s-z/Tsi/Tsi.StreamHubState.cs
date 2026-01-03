namespace Skender.Stock.Indicators;

/// <summary>
/// State for TSI calculations using StreamHubState pattern.
/// </summary>
/// <param name="PrevValue">Previous close value</param>
/// <param name="PrevCs1">Previous first-stage change EMA</param>
/// <param name="PrevAs1">Previous first-stage absolute change EMA</param>
/// <param name="PrevCs2">Previous second-stage change EMA</param>
/// <param name="PrevAs2">Previous second-stage absolute change EMA</param>
/// <param name="PrevSignal">Previous signal line value</param>
/// <param name="IsFirstPeriod">Whether this is the first period</param>
public record TsiState(
    double PrevValue,
    double PrevCs1,
    double PrevAs1,
    double PrevCs2,
    double PrevAs2,
    double PrevSignal,
    bool IsFirstPeriod) : IHubState;

/// <summary>
/// Streaming hub for True Strength Index (TSI) using state management.
/// </summary>
public class TsiHubState
    : ChainHubState<IReusable, TsiState, TsiResult>, ITsi
{
    private readonly double mult1;  // smoothing constant for first EMA
    private readonly double mult2;  // smoothing constant for second EMA
    private readonly double multS;  // smoothing constant for signal EMA

    // State variables
    private bool _isFirstPeriod;
    private double _prevValue;
    private double _prevCs1;
    private double _prevAs1;
    private double _prevCs2;
    private double _prevAs2;
    private double _prevSignal;

    // History lists for second smoothing initialization
    private readonly List<double> _cs1History;
    private readonly List<double> _as1History;

    internal TsiHubState(
        IChainProvider<IReusable> provider,
        int lookbackPeriods,
        int smoothPeriods,
        int signalPeriods) : base(provider)
    {
        Tsi.Validate(lookbackPeriods, smoothPeriods, signalPeriods);
        LookbackPeriods = lookbackPeriods;
        SmoothPeriods = smoothPeriods;
        SignalPeriods = signalPeriods;

        mult1 = 2d / (lookbackPeriods + 1);
        mult2 = 2d / (smoothPeriods + 1);
        multS = 2d / (signalPeriods + 1);

        Name = $"TSI({lookbackPeriods},{smoothPeriods},{signalPeriods})";

        _isFirstPeriod = true;
        _prevValue = double.NaN;
        _prevCs1 = double.NaN;
        _prevAs1 = double.NaN;
        _prevCs2 = double.NaN;
        _prevAs2 = double.NaN;
        _prevSignal = double.NaN;

        _cs1History = [];
        _as1History = [];

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public int SmoothPeriods { get; init; }

    /// <inheritdoc/>
    public int SignalPeriods { get; init; }

    /// <inheritdoc/>
    protected override void RestorePreviousState(TsiState? previousState)
    {
        if (previousState is null)
        {
            _isFirstPeriod = true;
            _prevValue = double.NaN;
            _prevCs1 = double.NaN;
            _prevAs1 = double.NaN;
            _prevCs2 = double.NaN;
            _prevAs2 = double.NaN;
            _prevSignal = double.NaN;
            _cs1History.Clear();
            _as1History.Clear();
        }
        else
        {
            _isFirstPeriod = previousState.IsFirstPeriod;
            _prevValue = previousState.PrevValue;
            _prevCs1 = previousState.PrevCs1;
            _prevAs1 = previousState.PrevAs1;
            _prevCs2 = previousState.PrevCs2;
            _prevAs2 = previousState.PrevAs2;
            _prevSignal = previousState.PrevSignal;
        }
    }

    /// <inheritdoc/>
    protected override (TsiResult result, TsiState state, int index)
        ToIndicatorState(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        double currentValue = item.Value;

        if (_isFirstPeriod)
        {
            _prevValue = currentValue;
            _isFirstPeriod = false;
            _cs1History.Add(double.NaN);
            _as1History.Add(double.NaN);

            TsiState firstState = new(_prevValue, _prevCs1, _prevAs1, _prevCs2, _prevAs2, _prevSignal, false);
            return (new TsiResult(item.Timestamp), firstState, i);
        }

        double change = currentValue - _prevValue;
        double absChange = Math.Abs(change);
        _prevValue = currentValue;

        // First smoothing
        double cs1, as1;
        if (double.IsNaN(_prevCs1) && i >= LookbackPeriods)
        {
            double sumC = 0, sumA = 0;
            for (int p = i - LookbackPeriods + 1; p <= i; p++)
            {
                double pValue = ProviderCache[p].Value;
                double pPrevValue = ProviderCache[p - 1].Value;
                double pChange = pValue - pPrevValue;
                sumC += pChange;
                sumA += Math.Abs(pChange);
            }
            cs1 = sumC / LookbackPeriods;
            as1 = sumA / LookbackPeriods;
            _prevCs1 = cs1;
            _prevAs1 = as1;
        }
        else if (!double.IsNaN(_prevCs1))
        {
            cs1 = ((change - _prevCs1) * mult1) + _prevCs1;
            as1 = ((absChange - _prevAs1) * mult1) + _prevAs1;
            _prevCs1 = cs1;
            _prevAs1 = as1;
        }
        else
        {
            cs1 = double.NaN;
            as1 = double.NaN;
        }

        _cs1History.Add(cs1);
        _as1History.Add(as1);

        // Second smoothing
        double cs2, as2;
        if (double.IsNaN(_prevCs2) && i >= SmoothPeriods && !double.IsNaN(cs1))
        {
            double sumCs = 0, sumAs = 0;
            for (int p = i - SmoothPeriods + 1; p <= i; p++)
            {
                sumCs += _cs1History[p];
                sumAs += _as1History[p];
            }
            cs2 = sumCs / SmoothPeriods;
            as2 = sumAs / SmoothPeriods;
            _prevCs2 = cs2;
            _prevAs2 = as2;
        }
        else if (!double.IsNaN(_prevCs2) && !double.IsNaN(cs1))
        {
            cs2 = ((cs1 - _prevCs2) * mult2) + _prevCs2;
            as2 = ((as1 - _prevAs2) * mult2) + _prevAs2;
            _prevCs2 = cs2;
            _prevAs2 = as2;
        }
        else
        {
            cs2 = double.NaN;
            as2 = double.NaN;
        }

        double tsi = as2 != 0 ? 100d * (cs2 / as2) : double.NaN;
        double signal = CalculateSignal(i, tsi);

        TsiResult r = new(
            Timestamp: item.Timestamp,
            Tsi: tsi.NaN2Null(),
            Signal: signal.NaN2Null());

        TsiState currentState = new(_prevValue, _prevCs1, _prevAs1, _prevCs2, _prevAs2, _prevSignal, false);
        return (r, currentState, i);
    }

    private double CalculateSignal(int index, double tsi)
    {
        if (SignalPeriods > 1)
        {
            if (double.IsNaN(_prevSignal) && index > SignalPeriods)
            {
                double sum = tsi;
                for (int p = index - SignalPeriods + 1; p < index; p++)
                {
                    sum += Cache[p].Tsi.Null2NaN();
                }
                _prevSignal = sum / SignalPeriods;
                return _prevSignal;
            }
            else if (!double.IsNaN(_prevSignal) && !double.IsNaN(tsi))
            {
                _prevSignal = ((tsi - _prevSignal) * multS) + _prevSignal;
                return _prevSignal;
            }
        }
        else if (SignalPeriods == 1)
        {
            return tsi;
        }
        return double.NaN;
    }

    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        base.RollbackState(timestamp);

        _isFirstPeriod = true;
        _prevValue = double.NaN;
        _prevCs1 = double.NaN;
        _prevAs1 = double.NaN;
        _prevCs2 = double.NaN;
        _prevAs2 = double.NaN;
        _prevSignal = double.NaN;
        _cs1History.Clear();
        _as1History.Clear();

        if (timestamp <= DateTime.MinValue || ProviderCache.Count == 0)
        {
            return;
        }

        int index = ProviderCache.IndexGte(timestamp);
        if (index <= 0)
        {
            return;
        }

        RestoreStateIfNeeded(index - 1);
    }

    private void RestoreStateIfNeeded(int targetIndex)
    {
        for (int i = 0; i <= targetIndex; i++)
        {
            IReusable item = ProviderCache[i];
            double currentValue = item.Value;

            if (_isFirstPeriod)
            {
                _prevValue = currentValue;
                _isFirstPeriod = false;
                _cs1History.Add(double.NaN);
                _as1History.Add(double.NaN);
                continue;
            }

            double change = currentValue - _prevValue;
            double absChange = Math.Abs(change);
            _prevValue = currentValue;

            double cs1, as1;
            if (double.IsNaN(_prevCs1) && i >= LookbackPeriods)
            {
                double sumC = 0, sumA = 0;
                for (int p = i - LookbackPeriods + 1; p <= i; p++)
                {
                    double pValue = ProviderCache[p].Value;
                    double pPrevValue = ProviderCache[p - 1].Value;
                    double pChange = pValue - pPrevValue;
                    sumC += pChange;
                    sumA += Math.Abs(pChange);
                }
                cs1 = sumC / LookbackPeriods;
                as1 = sumA / LookbackPeriods;
                _prevCs1 = cs1;
                _prevAs1 = as1;
            }
            else if (!double.IsNaN(_prevCs1))
            {
                cs1 = ((change - _prevCs1) * mult1) + _prevCs1;
                as1 = ((absChange - _prevAs1) * mult1) + _prevAs1;
                _prevCs1 = cs1;
                _prevAs1 = as1;
            }
            else
            {
                cs1 = double.NaN;
                as1 = double.NaN;
            }

            _cs1History.Add(cs1);
            _as1History.Add(as1);

            double cs2, as2;
            if (double.IsNaN(_prevCs2) && i >= SmoothPeriods && !double.IsNaN(cs1))
            {
                double sumCs = 0, sumAs = 0;
                for (int p = i - SmoothPeriods + 1; p <= i; p++)
                {
                    sumCs += _cs1History[p];
                    sumAs += _as1History[p];
                }
                cs2 = sumCs / SmoothPeriods;
                as2 = sumAs / SmoothPeriods;
                _prevCs2 = cs2;
                _prevAs2 = as2;
            }
            else if (!double.IsNaN(_prevCs2) && !double.IsNaN(cs1))
            {
                cs2 = ((cs1 - _prevCs2) * mult2) + _prevCs2;
                as2 = ((as1 - _prevAs2) * mult2) + _prevAs2;
                _prevCs2 = cs2;
                _prevAs2 = as2;
            }
            else
            {
                cs2 = double.NaN;
                as2 = double.NaN;
            }

            double tsi = as2 != 0 ? 100d * (cs2 / as2) : double.NaN;

            if (SignalPeriods > 1)
            {
                if (double.IsNaN(_prevSignal) && i > SignalPeriods && !double.IsNaN(tsi))
                {
                    double sum = tsi;
                    for (int p = i - SignalPeriods + 1; p < i; p++)
                    {
                        if (p >= 0 && p < Cache.Count)
                        {
                            sum += Cache[p].Tsi.Null2NaN();
                        }
                    }
                    _prevSignal = sum / SignalPeriods;
                }
                else if (!double.IsNaN(_prevSignal) && !double.IsNaN(tsi))
                {
                    _prevSignal = ((tsi - _prevSignal) * multS) + _prevSignal;
                }
            }
            else if (SignalPeriods == 1)
            {
                _prevSignal = tsi;
            }
        }
    }
}

public static partial class Tsi
{
    /// <summary>
    /// Creates a TSI streaming hub with state management from a chain provider.
    /// </summary>
    public static TsiHubState ToTsiHubState(
        this IChainProvider<IReusable> chainProvider,
        int lookbackPeriods = 25,
        int smoothPeriods = 13,
        int signalPeriods = 7)
        => new(chainProvider, lookbackPeriods, smoothPeriods, signalPeriods);
}
