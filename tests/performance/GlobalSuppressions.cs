// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Design",
    "CA1051:Do not declare visible instance fields",
    Justification = "Required for BenchmarkDotNet")]

[assembly: SuppressMessage(
    "Style",
    "IDE0058:Expression value is never used",
    Justification = "Not worth refactoring for tests.")]

[assembly: SuppressMessage(
    "StyleCop.CSharp.DocumentationRules",
    "SA1600:Elements should be documented",
    Justification = "Not documenting unit test projects.")]

[assembly: SuppressMessage(
    "Performance",
    "CA1822:Mark members as static",
    Justification = "Tests are excluded when static, for some reason")]
