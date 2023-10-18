namespace Skender.Stock.Indicators;

// AVERAGE DIRECTIONAL INDEX (COMMON)

public partial class Adx
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
    // TODO: add self-storage variant of variant
    /// <include file='./info.xml' path='info/type[@name="increment"]/*' />
    ///
    public AdxResult Increment<TQuote>(TQuote quote)
        where TQuote : IQuote
    {
        AdxResult r = new(quote.Date);
        QuoteD q = quote.ToQuoteD();
        int i = ProtectedResults.Count;

        // skip first period
        if (i == 0)
        {
            PrevHigh = q.High;
            PrevLow = q.Low;
            PrevClose = q.Close;

            ProtectedResults.Add(r);
            return r;
        }

        double tr = Tr.Increment(PrevClose, q.High, q.Low);

        double hmph = q.High - PrevHigh;
        double plml = PrevLow - q.Low;

        double pdm1 = hmph > plml ? Math.Max(hmph, 0) : 0;
        double mdm1 = plml > hmph ? Math.Max(plml, 0) : 0;

        PrevHigh = q.High;
        PrevLow = q.Low;
        PrevClose = q.Close;

        // initialization period
        if (i <= LookbackPeriods)
        {
            SumTr += tr;
            PrevPdm += pdm1;
            PrevMdm += mdm1;
        }

        // skip DM initialization period
        if (i < LookbackPeriods)
        {
            ProtectedResults.Add(r);
            return r;
        }

        // smoothed true range and directional movement
        double trs;
        double pdm;
        double mdm;

        // directional movement, initial
        if (i == LookbackPeriods)
        {
            trs = SumTr;
            pdm = PrevPdm;
            mdm = PrevMdm;

            SumTr = double.NaN;
        }

        // directional movement
        else
        {
            trs = PrevTrs - (PrevTrs / LookbackPeriods) + tr;
            pdm = PrevPdm - (PrevPdm / LookbackPeriods) + pdm1;
            mdm = PrevMdm - (PrevMdm / LookbackPeriods) + mdm1;
        }

        PrevTrs = trs;
        PrevPdm = pdm;
        PrevMdm = mdm;

        if (trs is 0)
        {
            ProtectedResults.Add(r);
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
        if (i > (2 * LookbackPeriods) - 1)
        {
            adx = ((PrevAdx * (LookbackPeriods - 1)) + dx) / LookbackPeriods;
            r.Adx = adx.NaN2Null();

            double? priorAdx = ProtectedResults[i + 1 - LookbackPeriods].Adx;

            r.Adxr = (adx + priorAdx).NaN2Null() / 2;
            PrevAdx = adx;
        }

        // initial ADX
        else if (i == (2 * LookbackPeriods) - 1)
        {
            SumDx += dx;
            adx = SumDx / LookbackPeriods;
            r.Adx = adx.NaN2Null();
            PrevAdx = adx;

            SumDx = double.NaN;
        }

        // initialization
        else
        {
            SumDx += dx;
        }

        ProtectedResults.Add(r);
        return r;
    }
}
