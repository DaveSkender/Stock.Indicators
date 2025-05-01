global using System.Collections.Immutable;
global using System.Globalization;
global using System.Runtime.CompilerServices;
global using System.Text;
global using Generators.Analyzer.Rules;
global using Generators.Catalogger.Utils;
global using Microsoft.CodeAnalysis;
global using Microsoft.CodeAnalysis.CSharp;
global using Microsoft.CodeAnalysis.CSharp.Syntax;
global using Microsoft.CodeAnalysis.Diagnostics;
global using Microsoft.CodeAnalysis.Text;

[assembly: InternalsVisibleTo("Tests.Indicators")]  // these test internals
[assembly: InternalsVisibleTo("Tests.Generators")]
