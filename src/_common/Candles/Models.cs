using System;

namespace Skender.Stock.Indicators
{
    // CANDLESTICK MODELS

    [Serializable]
    internal class Candle : Quote
    {
        // raw sizes
        internal decimal Size => High - Low;
        internal decimal Body => (Open > Close) ? (Open - Close) : (Close - Open);
        internal decimal UpperWick => High - (Open > Close ? Open : Close);
        internal decimal LowerWick => (Open > Close ? Close : Open) - Low;

        // percent sizes
        internal decimal BodyPct => (Size != 0) ? Body / Size : 1m;
        internal decimal UpperWickPct => (Size != 0) ? UpperWick / Size : 1m;
        internal decimal LowerWickPct => (Size != 0) ? LowerWick / Size : 1m;

        // directional info
        internal bool IsBullish => (Close > Open);
        internal bool IsBearish => (Close < Open);
    }

}
