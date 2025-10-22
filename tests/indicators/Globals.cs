using System.Runtime.CompilerServices;

[assembly: CLSCompliant(true)]
[assembly: InternalsVisibleTo("Tests.Other")]         // these use test data
[assembly: InternalsVisibleTo("Tests.Performance")]

[assembly: Parallelize(Scope = ExecutionScope.MethodLevel, Workers = 0)]
