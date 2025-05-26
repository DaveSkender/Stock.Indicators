global using System.Collections.Immutable;
global using System.Runtime.CompilerServices;
global using Generators.Analyzer.Rules;
global using Microsoft.CodeAnalysis;
global using Microsoft.CodeAnalysis.CSharp;
global using Microsoft.CodeAnalysis.CSharp.Syntax;
global using Microsoft.CodeAnalysis.Diagnostics;

[assembly: InternalsVisibleTo("Tests.Indicators")]  // these test internals
[assembly: InternalsVisibleTo("Tests.Generators")]
