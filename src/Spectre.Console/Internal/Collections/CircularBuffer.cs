namespace Spectre.Console;

/// <summary>
/// Defines a circular buffer that automatically overwrites the oldest elements
/// when new elements are added beyond its capacity.
/// </summary>
/// <typeparam name="T">The type to hold in the buffer.</typeparam>
/// <remarks>
/// <para>
/// Circular buffers are useful for scenarios such as stats where we want to
/// automatically expire the oldest elements without the need to reallocate memory.
/// </para>
/// <para>
/// This implementation uses a regular <see cref="List{T}"/> and adds new elements until
/// the capacity is reached. When this occurs, an internal _offset is incremented and
/// the new element overwrites the (_offset + Count) element in the list, wrapping
/// as needed. Likewise, removal is done by decrementing the Count property until it
/// reaches zero.
/// </para>
/// <para>
/// Other than the initial resizing of <see cref="List{T}"/> both addition and removal
/// of elements have a time complexity of O(1) and require 0 memory allocations.
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
        if (Count < _capacity)
        {
            AddInternal(item);
            Count++;
        }
        else
        {
            AddInternal(item);
            _offset = (_offset + 1) % _capacity;
        }
    }

    public void Clear()
    {
        _offset = 0;
        Count = 0;
    }

    public bool Contains(T item)
    {
        for (var i = 0; i < Count; i++)
        {
            if (EqualityComparer<T>.Default.Equals(this[i], item))
            {
                return true;
            }
        }

        return false;
    }

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

    public void Insert(int index, T item) => throw new NotSupportedException();

    public bool Remove(T item) => throw new NotSupportedException();

    public void Remove()
    {
        if (Count <= 0)
        {
            return;
        }

        --Count;
        _offset = (_offset + 1) % _capacity;
    }

    public void Remove(Func<T, bool> predicate)
    {
        while (Count > 0 && predicate(_internal[_offset]))
        {
            Remove();
        }
    }

    public void RemoveAt(int index) => throw new NotSupportedException();

    private void AddInternal(T item)
    {
        if (_internal.Count < _capacity)
        {
            _internal.Add(item);
        }
        else
        {
            _internal[(_offset + Count) % _capacity] = item;
        }
    }

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