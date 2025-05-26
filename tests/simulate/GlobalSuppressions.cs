using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Maintainability",
    "CA1510:Use ArgumentNullException throw helper",
    Justification = "Does not support .NET Standard.")]

[assembly: SuppressMessage(
    "Maintainability",
    "CA1515:Consider making public types internal",
    Justification = "Test projects use public class types.")]

[assembly: SuppressMessage("Naming",
    "CA1720:Identifier contains type name"
    , Justification = "Not really an issue.",
    Scope = "type",
    Target = "~T:Skender.Stock.Indicators.Direction")]

[assembly: SuppressMessage(
    "Naming",
    "CA1720:Identifier contains type name",
    Justification = "Not really an issue.",
    Scope = "member",
    Target = "~F:Skender.Stock.Indicators.Direction.Long")]

[assembly: SuppressMessage(
    "Naming",
    "CA1720:Identifier contains type name",
    Justification = "Not really an issue.",
    Scope = "member",
    Target = "~F:Skender.Stock.Indicators.Direction.Short")]

[assembly: SuppressMessage(
    "Maintainability",
    "CA1510:Use ArgumentNullException throw helper",
    Justification = "Can only use with .NET 6 or later.  We support .NET Framework and .NET Standard.")]

[assembly: SuppressMessage(
    "Naming",
    "CA1716:Identifiers should not match keywords",
    Justification = "Temporary, during deprecation period.",
    Scope = "member",
    Target = "~P:Skender.Stock.Indicators.ISeries.Date")]
