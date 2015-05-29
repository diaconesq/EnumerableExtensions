// Copyright © 2014, Jim Mischel

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Mischel.EnumerableExtensions
{
    /// <summary>
    /// Represents a d-Ary min heap of objects.
    /// </summary>
    /// <typeparam name="T">The type of elements in the heap.</typeparam>
    public class MinDHeap<T> : DHeap<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static private bool MinCompare(int r)
        {
            return r > 0;
        }

        /// <summary>
        /// Initializes a new instance of the DaryMinHeap&lt;T&gt; class that is empty
        /// and has the specified initial capacity.
        /// </summary>
        /// <param name="ary">The order of the heap. For example, a binary heap is a 2-heap.</param>
        /// <param name="capacity">The number of elements that the heap can initially store.</param>
        /// <param name="comparer">The IComparer&lt;T&gt; implementation to use when
        /// comparing keys. If null or not supplied, the default Comparer&lt;T&gt; is used.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <para><paramref name="ary"/>is less than or equal to one.</para>
        /// <para><paramref name="capacity"/>is less than zero.</para>
        /// </exception>
        public MinDHeap(int ary, int capacity, IComparer<T> comparer = null)
            : base(ary, MinCompare, capacity, comparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DaryMinHeap&lt;T&gt; class that is empty
        /// and has the default initial capacity.
        /// </summary>
        /// <param name="ary">The order of the heap. For example, a binary heap is a 2-heap.</param>
        /// <param name="comparer">The IComparer&lt;T&gt; implementation to use when
        /// comparing keys. If null or not supplied, the default Comparer&lt;T&gt; is used.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="ary"/>is less than or equal to one.</exception>
        public MinDHeap(int ary, IComparer<T> comparer = null)
            : this(ary, 0, comparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DaryMinHeap&lt;T&gt; class that contains 
        /// elements copied from the specified collection and has sufficient capacity 
        /// to accommodate the number of elements copied.
        /// </summary>
        /// <param name="ary">The order of the heap. For example, a binary heap is a 2-heap.</param>
        /// <param name="collection">The collection whose elements are copied to the new heap. The collection may not be null.</param>
        /// <param name="comparer">The IComparer&lt;T&gt; implementation to use when
        /// comparing keys. If null or not supplied, the default Comparer&lt;T&gt; is used.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="collection"/>is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="ary"/>is less than or equal to one.</exception>
        public MinDHeap(int ary, IEnumerable<T> collection, IComparer<T> comparer = null)
            : base(ary, MinCompare, collection, comparer)
        {
        }
    }

    /// <summary>
    /// Represents a d-Ary max heap of objects.
    /// </summary>
    /// <typeparam name="T">The type of elements in the heap.</typeparam>
    public class MaxDHeap<T> : DHeap<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static private bool MaxCompare(int r)
        {
            return r < 0;
        }

        /// <summary>
        /// Initializes a new instance of the DaryMaxHeap&lt;T&gt; class that is empty
        /// and has the specified initial capacity.
        /// </summary>
        /// <param name="ary">The order of the heap. For example, a binary heap is a 2-heap.</param>
        /// <param name="capacity">The number of elements that the heap can initially store.</param>
        /// <param name="comparer">The IComparer&lt;T&gt; implementation to use when
        /// comparing keys. If null or not supplied, the default Comparer&lt;T&gt; is used.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <para><paramref name="ary"/>is less than or equal to one.</para>
        /// <para><paramref name="capacity"/>is less than zero.</para>
        /// </exception>
        public MaxDHeap(int ary, int capacity, IComparer<T> comparer = null)
            : base(ary, MaxCompare, capacity, comparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DaryMaxHeap&lt;T&gt; class that is empty
        /// and has the default initial capacity.
        /// </summary>
        /// <param name="ary">The order of the heap. For example, a binary heap is a 2-heap.</param>
        /// <param name="comparer">The IComparer&lt;T&gt; implementation to use when
        /// comparing keys. If null or not supplied, the default Comparer&lt;T&gt; is used.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="ary"/>is less than or equal to one.</exception>
        public MaxDHeap(int ary, IComparer<T> comparer = null)
            : this(ary, 0, comparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DaryMaxHeap&lt;T&gt; class that contains 
        /// elements copied from the specified collection and has sufficient capacity 
        /// to accommodate the number of elements copied.
        /// </summary>
        /// <param name="ary">The order of the heap. For example, a binary heap is a 2-heap.</param>
        /// <param name="collection">The collection whose elements are copied to the new heap. The collection may not be null.</param>
        /// <param name="comparer">The IComparer&lt;T&gt; implementation to use when
        /// comparing keys. If null or not supplied, the default Comparer&lt;T&gt; is used.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="collection"/>is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="ary"/>is less than or equal to one.</exception>
        public MaxDHeap(int ary, IEnumerable<T> collection, IComparer<T> comparer = null)
            : base(ary, MaxCompare, collection, comparer)
        {
        }
    }

    /// <summary>
    /// Implements a d-ary heap of objects.
    /// </summary>
    /// <typeparam name="T">The type of objects in the heap.</typeparam>
    /// <remarks>Clients may not create instances of this class directly. Use DaryMinHeap&lt;T&gt;
    /// and DaryMaxHeap&lt;T&gt;.
    /// </remarks>
    [DebuggerDisplay("Count = {Count}")]
    public class DHeap<T>: IHeap<T>
    {
        private readonly IComparer<T> _comparer;
        private readonly Func<int, bool> _resultComparer;
 
        private readonly int _ary;

        private readonly List<T> _items = new List<T>();

        private DHeap(int ary, Func<int, bool> resultComparer, IComparer<T> comparer = null)
        {
            _ary = ary;
            _comparer = comparer ?? Comparer<T>.Default;
            _resultComparer = resultComparer;
        }

        internal DHeap(int ary, Func<int, bool> resultComparer, int capacity, IComparer<T> comparer = null)
            : this(ary, resultComparer, comparer)
        {
            if (ary < 2)
            {
                throw new ArgumentOutOfRangeException("ary", "must be greater than or equal to two.");
            }
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException("capacity", "must be greater than zero.");
            }
            _items = new List<T>(capacity);
        }

        internal DHeap(int ary, Func<int, bool> resultComparer, IEnumerable<T> collection, IComparer<T> comparer = null)
            : this(ary, resultComparer, comparer)
        {
            if (ary < 2)
            {
                throw new ArgumentOutOfRangeException("ary", "must be greater than or equal to two.");
            }
            if (collection == null)
            {
                throw new ArgumentNullException("collection", "may not be null.");
            }
            _items = new List<T>(collection);
            Heapify();
        }

        private void Heapify()
        {
            for (int i = _items.Count/_ary; i >= 0; --i)
            {
                SiftDown(i);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool DoCompare(T x, T y)
        {
            return _resultComparer(_comparer.Compare(x, y));
        }

        private void SiftDown(int i)
        {
            while ((_ary*i) + 1 < _items.Count)
            {
                int child = (i*_ary) + 1;
                // find the smallest child
                int currentSmallestChild = child;

                int maxChild = child + _ary;
                if (maxChild > _items.Count)
                {
                    maxChild = _items.Count;
                }
                ++child;
                while (child < maxChild)
                {
                    if (DoCompare(_items[currentSmallestChild], _items[child]))
                    {
                        currentSmallestChild = child;
                    }
                    ++child;
                }

                child = currentSmallestChild;
                // percolate one level
                if (DoCompare(_items[child], _items[i]))
                {
                    break;
                }
                var temp = _items[i];
                _items[i] = _items[child];
                _items[child] = temp;
                i = child;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the heap is empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return _items.Count == 0; }
        }

        /// <summary>
        /// Gets the number of items actually contained in the heap.
        /// </summary>
        public int Count
        {
            get { return _items.Count; }
        }
        
        /// <summary>
        /// Gets or sets the total number of items the internal data structure can
        /// hold without resizing.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Capacity is set to a value that is less than Count.</exception>
        /// <exception cref="System.OutOfMemoryException">
        /// There is not enough memory on the system.</exception>
        public int Capacity
        {
            get { return _items.Capacity; }
            set
            {
                if (value < _items.Count)
                {
                    throw new ArgumentOutOfRangeException("value", "must be greater than or equal to Count");
                }
                _items.Capacity = value;
            }
        }

        /// <summary>
        /// Remove all items from the heap.
        /// </summary>
        /// <remarks>This method is an O(n) operation, where n is <c>Count</c>.</remarks>
        public void Clear()
        {
            _items.Clear();
        }

        /// <summary>
        /// Sets the capacity to the actual number of elements in the heap,
        /// if that number is less than a threshold value.
        /// </summary>
        /// <remarks>
        /// <para>This method can be used to minimize a collection's memory overhead if no new 
        /// elements will be added to the collection. The cost of reallocating and copying
        /// a large heap can be considerable, however, so the <c>TrimExcess</c>
        /// method does nothing if the list is at more than 90 percent of capacity.
        /// This avoids incurring a large reallocation cost for a relatively small gain.</para>
        /// <para>This method is an O(n) operation, where n is <c>Count</c>.</para>
        /// <para>To reset a heap to its initial state, call the <c>Clear</c>
        /// method before calling the <c>TrimExcess</c> method. Trimming an empty heap
        /// sets the capacity of the heap to the default capacity.</para>
        /// <para>The capacity can also be set using the <c>Capacity</c> property.</para>
        /// <para>The 90% value may change with changes to the .NET Framework.</para>
        /// </remarks>
        public void TrimExcess()
        {
            _items.TrimExcess();
        }
        
        /// <summary>
        /// Inserts a value into the heap.
        /// </summary>
        /// <param name="item">The value to be inserted. If T is a reference type,
        /// item may not be null.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="item"/> is null.</exception>
        /// <remarks>This is an O(log n) operation, where n is equal to <c>Count</c>.</remarks>
        public void Insert(T item)
        {
            if (ReferenceEquals(item, null))
            {
                throw new ArgumentNullException("item");
            }
            _items.Add(item);
            BubbleUp(_items.Count - 1);
        }

        private void BubbleUp(int i)
        {
            T item = _items[i];
            while (i > 0 && DoCompare(_items[(i - 1) / _ary], item))
            {
                _items[i] = _items[(i - 1) / _ary];
                i = (i - 1) / _ary;
            }
            _items[i] = item;
        }

        /// <summary>
        /// Inserts the items of the specified collection into the MinMaxHeap&lt;T&gt;.
        /// </summary>
        /// <param name="collection">The collection whose elements should be inserted
        /// into the heap.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="collection"/>is null</exception>
        /// <exception cref="System.InvalidOperationException">An item in <paramref name="collection"/>is null.</exception>
        /// <remarks>This is an O(k log n) operation, where k is the number of items to be added,
        /// and n is equal to <c>Count</c>.</remarks>
        public void InsertRange(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            int old = _items.Count;
            _items.AddRange(collection);
            // Simple optimization here.
            // If the number of items being added is more than (_items.Count / _ary),
            // then it's probably faster to re-heapify the entire thing rather than
            // call BubbleUp for each of the new items.
            int itemsAdded = _items.Count - old;
            if (itemsAdded > _items.Count/_ary)
            {
                Heapify();
            }
            else
            {
                // bubble up each item 
                for (int i = old; i < _items.Count; ++i)
                {
                    if (ReferenceEquals(_items[i], null))
                    {
                        throw new InvalidOperationException("Cannot insert a null item into the heap.");
                    }
                    BubbleUp(i);
                }
            }
        }

        /// <summary>
        /// Returns the root item from the heap, without removing it.
        /// </summary>
        /// <returns>Returns the item at the root of the heap.</returns>
        /// <exception cref="System.InvalidOperationException">The heap is empty.</exception>
        /// <remarks>
        /// <para>This is an O(1) operation.</para>
        /// <para>The root element will be the smallest element in a min heap,
        /// and the largest element in a max heap.</para>
        /// </remarks>
        public T Peek()
        {
            if (_items.Count == 0)
            {
                throw new InvalidOperationException("The heap is empty.");
            }
            return _items[0];
        }

        /// <summary>
        /// Removes and returns the root item from the heap.
        /// </summary>
        /// <returns>Returns the item at the root of the heap.</returns>
        /// <exception cref="System.InvalidOperationException">The heap is empty.</exception>
        /// <remarks>
        /// <para>This is an O(log n) operation.</para>
        /// <para>The root element will be the smallest element in a min heap,
        /// and the largest element in a max heap.</para>
        /// </remarks>
        public T RemoveRoot()
        {
            if (_items.Count == 0)
            {
                throw new InvalidOperationException("The heap is empty.");
            }
            // Get the first item
            T rslt = _items[0];

            // Get the last item and insert it at the top
            _items[0] = _items[_items.Count - 1];
            _items.RemoveAt(_items.Count - 1);

            if (_items.Count > 0)
            {
                SiftDown(0);
            }
            return rslt;
        }

        /// <summary>
        /// Removes and returns the root item, and adds the new item.
        /// </summary>
        /// <param name="item">The item to be added to the heap. The item may not be null.</param>
        /// <returns>Returns the item at the root of the heap.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="item"/>is null.</exception>
        /// <exception cref="System.InvalidOperationException">The heap is empty.</exception>
        /// <remarks>This method has the same effect as calling <c>RemoveRoot</c>
        /// followed by <c>Insert</c>, but is much faster.</remarks>
        public T ReplaceRoot(T item)
        {
            if (ReferenceEquals(item, null))
            {
                throw new ArgumentNullException("item", "may not be null.");
            }
            if (_items.Count == 0)
            {
                throw new InvalidOperationException("The heap is empty");
            }
            T oldItem = _items[0];
            _items[0] = item;
            SiftDown(0);

            return oldItem;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the heap.
        /// </summary>
        /// <returns>An IEnumerator&lt;T&gt; that can be used to iterate through the collection.</returns>
        /// <remarks>The enumerator returns items in the order in which they are stored by
        /// the internal data structure. It is not guaranteed to be in sorted order.</remarks>
        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the heap.
        /// </summary>
        /// <returns>An IEnumerator that can be used to iterate through the collection.</returns>
        /// <remarks>The enumerator returns items in the order in which they are stored by
        /// the internal data structure. It is not guaranteed to be in sorted order.</remarks>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns a consuming enumerable for items in the MinMaxHeap&lt;T&gt;.
        /// </summary>
        /// <returns>An IEnumerable&lt;T&gt; that removes and returns items from the collection,
        /// in ascending order.</returns>
        /// <remarks>
        /// <para>Removes and returns items in order, starting with the minimum element.</para>
        /// </remarks>
        public IEnumerable<T> GetConsumingEnumerable()
        {
            while (_items.Count > 0)
            {
                yield return RemoveRoot();
            }
        }
    }
}
