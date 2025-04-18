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
    "Maintainability",
    "CA1515:Consider making public types internal",
    Justification = "Test projects use public class types.")]

[assembly: SuppressMessage(
    "Security",
    "CA5394:Do not use insecure randomness",
    Justification = "Okay for test rig, non-production code.")]

[assembly: SuppressMessage(
    "StyleCop.CSharp.SpacingRules",
    "SA1010:Opening square brackets should be spaced correctly",
    Justification = "Invalid for new C# 12 [ collection ] syntax.")]
