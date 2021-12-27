// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Naming",
    "CA1716:Identifiers should not match keywords",
    Justification = "Making an exception",
    Scope = "member",
    Target = "~P:Skender.Stock.Indicators.IQuote.Date")]

[assembly: SuppressMessage(
    "Naming",
    "CA1716:Identifiers should not match keywords",
    Justification = "Making an exception",
    Scope = "member",
    Target = "~P:Skender.Stock.Indicators.IResult.Date")]

// this can be removed after Microsoft publishes fix,
// see https://github.com/dotnet/roslyn/issues/55014
[assembly: SuppressMessage(
    "Style",
    "IDE0130:Namespace does not match folder structure",
    Justification = "Microsoft bug?, not a real problem")]
