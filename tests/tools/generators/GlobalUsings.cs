global using System.Collections.Generic;
global using FluentAssertions;
global using Microsoft.CodeAnalysis;
global using Microsoft.CodeAnalysis.CSharp;
global using Microsoft.CodeAnalysis.CSharp.Syntax;
global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using Skender.Stock.Indicators;

[assembly: Parallelize(Scope = ExecutionScope.MethodLevel)]
