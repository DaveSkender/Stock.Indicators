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
    Target = "~P:Skender.Stock.Indicators.IResultBase.Date")]

[assembly: SuppressMessage(
    "StyleCop.CSharp.MaintainabilityRules",
    "SA1402:File may only contain a single type",
    Justification = "This will be removed.",
    Scope = "type",
    Target = "~T:Skender.Stock.Indicators.Cleaners")]
