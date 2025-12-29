using System.Runtime.CompilerServices;

[assembly: CLSCompliant(true)]
[assembly: InternalsVisibleTo("Tests.PublicApi")]    // these use test data
[assembly: InternalsVisibleTo("Tests.Performance")]
[assembly: InternalsVisibleTo("Test.DataGenerator")]

[assembly: Parallelize(Scope = ExecutionScope.MethodLevel, Workers = 0)]
