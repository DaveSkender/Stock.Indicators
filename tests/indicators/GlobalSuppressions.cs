// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Globalization",
    "CA1303:Do not pass literals as localized parameters",
    Justification = "Not worth refactoring for tests.")]

[assembly: SuppressMessage(
    "Style",
    "IDE0058:Expression value is never used",
    Justification = "Not worth refactoring for tests.")]

[assembly: SuppressMessage(
    "StyleCop.CSharp.DocumentationRules",
    "SA1600:Elements should be documented",
    Justification = "Not documenting unit test projects.")]
