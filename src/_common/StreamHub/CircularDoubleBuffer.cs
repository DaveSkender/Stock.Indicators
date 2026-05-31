namespace Skender.Stock.Indicators;

/// <summary>
/// Fixed-size circular buffer for <c>double</c> values with O(capacity) max/min scan.
/// Zero heap allocation per tick; cache-line-resident for small windows (&lt;= ~32 elements).
/// </summary>
internal struct CircularDoubleBuffer : IEquatable<CircularDoubleBuffer>
{
    private readonly double[] _values;
    private int _head;  // next write position
    private int _fill;  // count of valid entries (0..Capacity)

    internal CircularDoubleBuffer(int capacity)
    {
        _values = new double[capacity];
        _head = 0;
        _fill = 0;
    }

    internal readonly int Capacity => _values.Length;
    internal readonly bool IsFull => _fill == _values.Length;
    internal readonly bool IsEmpty => _fill == 0;

    internal void Add(double value)
    {
        _values[_head] = value;
        if (++_head >= _values.Length)
        {
            _head = 0;
        }

        if (_fill < _values.Length)
        {
            _fill++;
        }
    }

    internal void Clear()
    {
        _head = 0;
        _fill = 0;
    }

    internal readonly double GetMax()
    {
        if (_fill == 0)
        {
            return double.NaN;
        }

        double max = _values[0];
        for (int i = 1; i < _fill; i++)
        {
            if (_values[i] > max)
            {
                max = _values[i];
            }
        }

        return max;
    }

    internal readonly double GetMin()
    {
        if (_fill == 0)
        {
            return double.NaN;
        }

        double min = _values[0];
        for (int i = 1; i < _fill; i++)
        {
            if (_values[i] < min)
            {
                min = _values[i];
            }
        }

        return min;
    }

    /// <inheritdoc/>
    public bool Equals(CircularDoubleBuffer other)
    {
        if (_head != other._head || _fill != other._fill)
        {
            return false;
        }

        double[] a = _values;
        double[] b = other._values;

        if (a == null && b == null)
        {
            return true;
        }

        if (a == null || b == null)
        {
            return false;
        }

        if (a.Length != b.Length)
        {
            return false;
        }

        for (int i = 0; i < a.Length; i++)
        {
            if (!a[i].Equals(b[i]))
            {
                return false;
            }
        }

        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is CircularDoubleBuffer other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 31 + _head;
            hash = hash * 31 + _fill;
            hash = hash * 31 + (_values?.Length ?? 0);

            if (_values is not null)
            {
                int limit = Math.Min(_fill, 4);
                for (int i = 0; i < limit; i++)
                {
                    hash = hash * 31 + _values[i].GetHashCode();
                }
            }

            return hash;
        }
    }

    public static bool operator ==(CircularDoubleBuffer left, CircularDoubleBuffer right) => left.Equals(right);

    public static bool operator !=(CircularDoubleBuffer left, CircularDoubleBuffer right) => !left.Equals(right);
}
