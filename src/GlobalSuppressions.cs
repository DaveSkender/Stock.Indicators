// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

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
    Justification = "Not compatible with `or` statement (Microsoft bug)",
    Scope = "member",
    Target = "~M:Skender.Stock.Indicators.ResultUtility.Condense``1(System.Collections.Generic.IEnumerable{``0})~System.Collections.Generic.IEnumerable{``0}")]
