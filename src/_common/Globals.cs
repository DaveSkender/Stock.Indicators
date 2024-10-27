using System.Runtime.CompilerServices;

[assembly: CLSCompliant(true)]
[assembly: InternalsVisibleTo("Tests.Indicators")]  // these test internals
[assembly: InternalsVisibleTo("Tests.Performance")]

namespace Skender.Stock.Indicators;
