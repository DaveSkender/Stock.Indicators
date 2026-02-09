global using System.Runtime.CompilerServices;
global using System.Text.Json.Serialization;

[assembly: CLSCompliant(true)]
[assembly: InternalsVisibleTo("Tests.Indicators")]  // these test internals
[assembly: InternalsVisibleTo("Tests.Performance")]
