// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Design",
    "CA1002:Do not expose generic lists",
    Justification = "This was intentional.")]

[assembly: SuppressMessage(
    "Globalization",
    "CA1303:Do not pass literals as localized parameters",
    Justification = "Not doing localization.")]

[assembly: SuppressMessage(
    "Naming",
    "CA1716:Identifiers should not match keywords",
    Justification = "Allow it for this class.",
    Scope = "member",
    Target = "~P:Skender.Stock.Indicators.IQuote.Date")]

[assembly: SuppressMessage(
    "Naming",
    "CA1716:Identifiers should not match keywords",
    Justification = "Allow it for this class.",
    Scope = "member",
    Target = "~P:Skender.Stock.Indicators.IResultBase.Date")]
