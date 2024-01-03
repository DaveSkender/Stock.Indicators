using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Naming",
    "CA1716:Identifiers should not match keywords",
    Justification = "Not really an issue.",
    Scope = "member",
    Target = "~P:Skender.Stock.Indicators.ISeries.Date")]

[assembly: SuppressMessage(
    "Naming",
    "CA1716:Identifiers should not match keywords",
    Justification = "Not really an issue.",
    Scope = "member",
    Target = "~P:Skender.Stock.Indicators.Quote.Date")]

[assembly: SuppressMessage(
    "Naming",
    "CA1716:Identifiers should not match keywords",
    Justification = "Not really an issue.",
    Scope = "member",
    Target = "~P:Skender.Stock.Indicators.IBasicData.Date")]

[assembly: SuppressMessage(
    "Naming",
    "CA1720:Identifier contains type name",
    Justification = "Not really an issue.",
    Scope = "member",
    Target = "~F:Skender.Stock.Indicators.ChandelierType.Short")]

[assembly: SuppressMessage(
    "StyleCop.CSharp.SpacingRules",
    "SA1008:Opening parenthesis should be spaced correctly",
    Justification = "Not compatible with `or` statement (analyzer bug)",
    Scope = "member",
    Target = "~M:Skender.Stock.Indicators.ResultUtility.Condense``1(System.Collections.Generic.IEnumerable{``0})~System.Collections.Generic.IEnumerable{``0}")]

[assembly: SuppressMessage(
    "Maintainability",
    "CA1510:Use ArgumentNullException throw helper",
    Justification = "Can only use with .NET 6 or later.  We support .NET Framework and .NET Standard.")]

[assembly: SuppressMessage(
    "StyleCop.CSharp.SpacingRules",
    "SA1010:Opening square brackets should be spaced correctly",
    Justification = "Invalid for new C# 12 [ collection ] syntax.")]

[assembly: SuppressMessage(
    "Naming",
    "CA1725:Parameter names should match base declaration",
    Justification = "The microsoft OnError implementation uses reserved word Error",
    Scope = "member",
    Target = "~M:Skender.Stock.Indicators.QuoteObserver.OnError(System.Exception)")]

[assembly: SuppressMessage(
    "Naming",
    "CA1716:Identifiers should not match keywords",
    Justification = "The microsoft OnError implementation uses reserved word Error",
    Scope = "member",
    Target = "~M:Skender.Stock.Indicators.QuoteObserver.OnError(System.Exception)")]

[assembly: SuppressMessage(
    "Naming",
    "CA1716:Identifiers should not match keywords",
    Justification = "The microsoft OnError implementation uses reserved word Error",
    Scope = "member", Target = "~M:Skender.Stock.Indicators.TupleObserver.OnError(System.Exception)")]
