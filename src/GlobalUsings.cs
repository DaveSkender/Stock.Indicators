global using System.Text.Json.Serialization;

using System.Runtime.CompilerServices;

[assembly: CLSCompliant(true)]
[assembly: InternalsVisibleTo("Tests.Indicators")]  // these test internals
[assembly: InternalsVisibleTo("Tests.Generators")]
[assembly: InternalsVisibleTo("Tests.Performance")]
