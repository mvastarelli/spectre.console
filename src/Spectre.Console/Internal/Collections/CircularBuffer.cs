namespace Spectre.Console;

/// <summary>
/// Defines a circular buffer that automatically overwrites the oldest elements
/// when new elements are added beyond its capacity.
/// </summary>
/// <typeparam name="T">The type to hold in the buffer.</typeparam>
/// <remarks>
/// <para>
/// Circular buffers are useful for scenarios such as <see cref="ProgressTask"/>
/// where we want to cap the number of samples to a fixed size and automatically
/// remove old samples when new ones are added. Using <see cref="List{T}.RemoveAt"/>
/// for this is inefficient since it requires shifting the entire array to the
/// left by one. In contrast, a circular buffer supports both addition and removal
/// with a time complexity of O(1).
/// </para>
/// <para>
/// This implementation uses a regular <see cref="List{T}"/> and adds new elements until
/// the capacity is reached. Afterward, new elements overwrite the
/// (_offset + Count) element and the internal _offset is incremented, wrapping around
/// as needed. Likewise, removal is done by decrementing the Count property until it
/// reaches zero.
/// </para>
/// </remarks>
internal sealed class CircularBuffer<T> : IList<T>
{
    private readonly List<T> _internal = [];
    private readonly int _capacity;
    private int _offset;

    public CircularBuffer(int capacity)
    {
        if (capacity <= 0)
        {
            throw new ArgumentException("Capacity must be greater than zero.", nameof(capacity));
        }

        _capacity = capacity;
        _offset = 0;
    }

    public int Count { get; private set; }

    public bool IsReadOnly => false;

    public T this[int index]
    {
        get => _internal[(_offset + index) % _capacity];
        set => throw new NotSupportedException();
    }

    public void Add(T item)
    {
        if (_internal.Count < _capacity)
        {
            _internal.Add(item);
        }
        else
        {
            _internal[(_offset + Count) % _capacity] = item;
        }

        if (Count < _capacity)
        {
            Count++;
        }
        else
        {
            _offset = (_offset + 1) % _capacity;
        }
    }

    public void Clear()
    {
        _internal.Clear();
        _offset = 0;
        Count = 0;
    }

    public bool Contains(T item) => IndexOf(item) != -1;

    public void CopyTo(T[] array, int arrayIndex)
    {
        for (var i = 0; i < Count; i++)
        {
            array[arrayIndex + i] = this[i];
        }
    }

    public IEnumerator<T> GetEnumerator() => new CircularListEnumerator(this);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int IndexOf(T item)
    {
        for (var i = 0; i < Count; i++)
        {
            if (EqualityComparer<T>.Default.Equals(this[i], item))
            {
                return i;
            }
        }

        return -1;
    }

    public void Insert(int index, T item)
        => throw new NotSupportedException("This type does not support insertion at arbitrary index locations.");

    public bool Remove(T item)
        => throw new NotSupportedException("This type does not support removal of arbitrary items.");

    public void Remove()
    {
        if (Count <= 0)
        {
            return;
        }

        --Count;
        _offset = (_offset + 1) % _capacity;
    }

    public void RemoveAt(int index)
        => throw new NotSupportedException("This type does not support removal at arbitrary index locations.");

    private sealed class CircularListEnumerator(CircularBuffer<T> buffer) : IEnumerator<T>
    {
        private int _currentOffset = -1;

        public T Current => buffer[_currentOffset];

        object IEnumerator.Current => buffer[_currentOffset]!;

        public bool MoveNext() => ++_currentOffset < buffer.Count;

        public void Reset() => _currentOffset = -1;

        public void Dispose()
        {
        }
    }
}