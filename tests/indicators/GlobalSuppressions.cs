using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Security",
    "CA5394:Do not use insecure randomness",
    Justification = "Okay for test rig, non-production code.")]
