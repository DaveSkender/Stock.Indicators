using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Maintainability",
    "CA1510:Use ArgumentNullException throw helper",
    Justification = "Does not support .NET Standard and before .NET 6")]

[assembly: SuppressMessage(
    "Naming",
    "CA1710:Identifiers should have correct suffix",
    Justification = "BufferList is the established naming convention for this library",
    Scope = "type",
    Target = "~T:Skender.Stock.Indicators.BufferList`1")]

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
    "Naming",
    "CA1716:Identifiers should not match keywords",
    Justification = "Temporary, during deprecation period.",
    Scope = "member",
    Target = "~P:Skender.Stock.Indicators.ISeries.Date")]
