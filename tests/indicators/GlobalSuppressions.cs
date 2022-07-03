// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "StyleCop.CSharp.NamingRules",
    "SA1304:Non-private readonly fields should begin with upper-case letter",
    Justification = "Acceptable for test project.")]

[assembly: SuppressMessage(
    "StyleCop.CSharp.NamingRules",
    "SA1307:Accessible fields should begin with upper-case letter",
    Justification = "Acceptable for test project.")]

[assembly: SuppressMessage(
    "StyleCop.CSharp.NamingRules",
    "SA1311:Static readonly fields should begin with upper-case letter",
    Justification = "Acceptable for test project.")]

[assembly: SuppressMessage(
    "Security",
    "CA5394:Do not use insecure randomness",
    Justification = "Okay for test rig, non-production code.")]
