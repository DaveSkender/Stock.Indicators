using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Maintainability",
    "CA1510:Use ArgumentNullException throw helper",
    Justification = "Does not support .NET Standard.")]

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
    Target = "~F:Skender.Stock.Indicators.ChandelierType.Long")]

[assembly: SuppressMessage(
    "Naming",
    "CA1720:Identifier contains type name",
    Justification = "Not really an issue.",
    Scope = "member",
    Target = "~F:Skender.Stock.Indicators.ChandelierType.Short")]

[assembly: SuppressMessage(
    "Naming",
    "CA1725:Parameter names should match base declaration",
    Justification = "The microsoft OnError implementation uses reserved word Error",
    Scope = "member",
    Target = "~M:Skender.Stock.Indicators.QuoteIn`1.OnError(System.Exception)")]

[assembly: SuppressMessage(
    "Naming",
    "CA1716:Identifiers should not match keywords",
    Justification = "The microsoft OnError implementation uses reserved word Error",
    Scope = "member",
    Target = "~M:Skender.Stock.Indicators.QuoteIn`1.OnError(System.Exception)")]

[assembly: SuppressMessage(
    "Naming",
    "CA1716:Identifiers should not match keywords",
    Justification = "The microsoft OnError implementation uses reserved word Error",
    Scope = "member", Target = "~M:Skender.Stock.Indicators.TupleObserver.OnError(System.Exception)")]

[assembly: SuppressMessage(
    "Naming",
    "CA1716:Identifiers should not match keywords",
    Justification = "The microsoft OnError implementation uses reserved word Error",
    Scope = "member",
    Target = "~M:Skender.Stock.Indicators.TupleInTupleOut.OnError(System.Exception)")]

[assembly: SuppressMessage(
    "Naming",
    "CA1716:Identifiers should not match keywords",
    Justification = "The microsoft OnError implementation uses reserved word Error",
    Scope = "member",
    Target = "~M:Skender.Stock.Indicators.QuoteInTupleOut`1.OnError(System.Exception)")]

[assembly: SuppressMessage(
    "Style",
    "IDE0056:Use index operator",
    Justification = "Does not support .NET Standard.")]
