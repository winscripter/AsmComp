using System.Collections;

namespace AsmComp.Core.Utilities;

/// <summary>
/// Represents a list whose values can be managed specifically by AsmComp.
/// </summary>
/// <typeparam name="T">Type of the list.</typeparam>
public class InternalList<T> : IEnumerable<T> {
    private readonly List<T> _list;

    internal InternalList() {
        _list = new();
    }

    /// <summary>
    /// Gets the value at a specific index.
    /// </summary>
    /// <param name="index">Index of the element.</param>
    /// <returns>An element at zero-based index <paramref name="index"/>.</returns>
    public T this[int index] {
        get => ((IList<T>)_list)[index];
        internal set => ((IList<T>)_list)[index] = value;
    }

    /// <summary>
    /// Represents a count of elements in the list.
    /// </summary>
    public int Count => ((ICollection<T>)_list).Count;

    /// <summary>
    /// Is the list read-only?
    /// </summary>
    public bool IsReadOnly => ((ICollection<T>)_list).IsReadOnly;

    internal void Add(T item) {
        ((ICollection<T>)_list).Add(item);
    }

    internal void Clear() {
        ((ICollection<T>)_list).Clear();
    }

    /// <summary>
    /// Checks whether the list contains the given element.
    /// </summary>
    /// <param name="item">The element in the list to check.</param>
    /// <returns>A boolean that indicates whether <paramref name="item"/> is in the list.</returns>
    public bool Contains(T item) {
        return ((ICollection<T>)_list).Contains(item);
    }

    public IEnumerator<T> GetEnumerator() {
        return ((IEnumerable<T>)_list).GetEnumerator();
    }

    public int IndexOf(T item) {
        return ((IList<T>)_list).IndexOf(item);
    }

    internal void Insert(int index, T item) {
        ((IList<T>)_list).Insert(index, item);
    }

    internal bool Remove(T item) {
        return ((ICollection<T>)_list).Remove(item);
    }

    internal void RemoveAt(int index) {
        ((IList<T>)_list).RemoveAt(index);
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return ((IEnumerable)_list).GetEnumerator();
    }
}
