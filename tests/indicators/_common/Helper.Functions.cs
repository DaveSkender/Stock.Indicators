using System;

namespace Internal.Tests
{
    // TEST HELPERS
    internal static class Helper
    {
        internal static decimal ToDecimal(this string value)
        {
            return decimal.TryParse(value, out decimal d) ? d : d;
        }

        internal static decimal? ToDecimalNull(this string value)
        {
            return decimal.TryParse(value, out decimal d) ? d : null;
        }

        internal static decimal? NullRound(this decimal? value, int decimals)
        {
            return value == null
                    ? value
                    : Math.Round((decimal)value, decimals);
        }
    }
}
