global using System.Text;
global using Microsoft.CodeAnalysis;
global using Microsoft.CodeAnalysis.CSharp;
global using Microsoft.CodeAnalysis.CSharp.Syntax;

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests.Indicators")]  // these test internals
[assembly: InternalsVisibleTo("Tests.Generators")]
