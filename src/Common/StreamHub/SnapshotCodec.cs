namespace FacioQuo.Stock.Indicators;

/// <summary>
/// Reads and writes a hub's internal state as a version byte followed by raw values.
/// </summary>
/// <remarks>
/// Doubles are written as their exact bit patterns, so a restored state
/// continues bit-for-bit, including NaN.
/// </remarks>
internal static class SnapshotCodec
{
    /// <summary>
    /// Encodes state with a leading version.
    /// </summary>
    /// <param name="version">Hub-specific state version.</param>
    /// <param name="write">Writes the state values.</param>
    /// <returns>The encoded state.</returns>
    internal static byte[] Write(byte version, Action<BinaryWriter> write)
    {
        using MemoryStream stream = new();
        using (BinaryWriter writer = new(stream))
        {
            writer.Write(version);
            write(writer);
        }

        return stream.ToArray();
    }

    /// <summary>
    /// Decodes state written by <see cref="Write"/>.
    /// </summary>
    /// <param name="state">Encoded state.</param>
    /// <param name="version">The only version the caller can read.</param>
    /// <param name="read">Reads the state values.</param>
    /// <exception cref="InvalidDataException">
    /// The state has another version, or is shorter or longer than the reader expects.
    /// </exception>
    internal static void Read(ReadOnlySpan<byte> state, byte version, Action<BinaryReader> read)
    {
        using MemoryStream stream = new(state.ToArray());
        using BinaryReader reader = new(stream);

        if (stream.Length == 0)
        {
            throw new InvalidDataException("State is empty.");
        }

        byte found = reader.ReadByte();

        if (found != version)
        {
            throw new InvalidDataException(
                $"State version {found} is not supported; expected {version}.");
        }

        try
        {
            read(reader);
        }
        catch (EndOfStreamException ex)
        {
            throw new InvalidDataException("State is shorter than expected.", ex);
        }

        if (stream.Position != stream.Length)
        {
            throw new InvalidDataException("State is longer than expected.");
        }
    }
}
