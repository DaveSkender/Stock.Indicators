using System.Globalization;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests.Indicators")]
[assembly: InternalsVisibleTo("Tests.Performance")]
namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        private static readonly CultureInfo englishCulture = new CultureInfo("en-US", false);
    }
}
