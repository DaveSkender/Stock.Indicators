namespace Skender.Stock.Indicators;

// AVERAGE DIRECTIONAL INDEX (COMMON)

public partial class Adx : ChainProvider
{
    // parameter validation
    internal static void Validate(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 1 for ADX.");
        }
    }

    // increment calculation
    /// <include file='./info.xml' path='info/type[@name="increment"]/*' />
    ///
    public static AdxResult Increment(int index, int lookbackPeriods, AdxInput input)
    {
        AdxResult r = new(DateTime.MinValue);

        return input is null ? r : Increment(index, lookbackPeriods, input, r);
    }

    internal static AdxResult Increment(int index, int lookbackPeriods, AdxInput p, AdxResult r)
    {
        double tr = Tr.Increment(p.PrevClose, p.High, p.Low);

        double hmph = p.High - p.PrevHigh;
        double plml = p.PrevLow - p.Low;

        double pdm1 = hmph > plml ? Math.Max(hmph, 0) : 0;
        double mdm1 = plml > hmph ? Math.Max(plml, 0) : 0;

        p.PrevHigh = p.High;
        p.PrevLow = p.Low;
        p.PrevClose = p.Close;

        // initialization period
        if (index <= lookbackPeriods)
        {
            p.SumTr += tr;
            p.PrevPdm += pdm1;
            p.PrevMdm += mdm1;
        }

        // skip DM initialization period
        if (index < lookbackPeriods)
        {
            return r;
        }

        // smoothed true range and directional movement
        double trs;
        double pdm;
        double mdm;

        // directional movement, initial
        if (index == lookbackPeriods)
        {
            trs = p.SumTr;
            pdm = p.PrevPdm;
            mdm = p.PrevMdm;

            p.SumTr = double.NaN;
        }

        // directional movement
        else
        {
            trs = p.PrevTrs - (p.PrevTrs / lookbackPeriods) + tr;
            pdm = p.PrevPdm - (p.PrevPdm / lookbackPeriods) + pdm1;
            mdm = p.PrevMdm - (p.PrevMdm / lookbackPeriods) + mdm1;
        }

        p.PrevTrs = trs;
        p.PrevPdm = pdm;
        p.PrevMdm = mdm;

        if (trs is 0)
        {
            return r;
        }

        // directional indicators
        double pdi = 100 * pdm / trs;
        double mdi = 100 * mdm / trs;

        r.Pdi = pdi;
        r.Mdi = mdi;

        // calculate ADX
        double dx = (pdi == mdi)
            ? 0
            : (pdi + mdi != 0)
            ? 100 * Math.Abs(pdi - mdi) / (pdi + mdi)
            : double.NaN;

        double adx;

        // normal ADX
        if (index > (2 * lookbackPeriods) - 1)
        {
            adx = ((p.PrevAdx * (lookbackPeriods - 1)) + dx) / lookbackPeriods;
            r.Adx = adx.NaN2Null();

            r.Adxr = (adx + p.WindowAdx).NaN2Null() / 2;
            p.PrevAdx = adx;
        }

        // initial ADX
        else if (index == (2 * lookbackPeriods) - 1)
        {
            p.SumDx += dx;
            adx = p.SumDx / lookbackPeriods;
            r.Adx = adx.NaN2Null();
            p.PrevAdx = adx;

            p.SumDx = double.NaN;
        }

        // initialization
        else
        {
            p.SumDx += dx;
        }

        return r;
    }
}
