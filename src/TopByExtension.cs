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
using System.Linq;

namespace Mischel.EnumerableExtensions
{
    public static class HeapEnumerable
    {
        public static IOrderedEnumerable<TSource> TopBy<TSource, TKey>(
            this IEnumerable<TSource> source,
            int numItems,
            Func<TSource, TKey> keySelector)
        {
            return new HeapOrderedEnumerable<TSource, TKey>(source, numItems, keySelector, null, false);
        }

        public static IOrderedEnumerable<TSource> TopBy<TSource, TKey>(
            this IEnumerable<TSource> source,
            int numItems,
            Func<TSource, TKey> keySelector,
            IComparer<TKey> comparer)
        {
            return new HeapOrderedEnumerable<TSource, TKey>(source, numItems, keySelector, comparer, false);
        }

        public static IOrderedEnumerable<TSource> TopByDescending<TSource, TKey>(
            this IEnumerable<TSource> source,
            int numItems,
            Func<TSource, TKey> keySelector)
        {
            return new HeapOrderedEnumerable<TSource, TKey>(source, numItems, keySelector, null, true);
        }

        public static IOrderedEnumerable<TSource> TopByDescending<TSource, TKey>(
            this IEnumerable<TSource> source,
            int numItems,
            Func<TSource, TKey> keySelector,
            IComparer<TKey> comparer)
        {
            return new HeapOrderedEnumerable<TSource, TKey>(source, numItems, keySelector, comparer, true);
        }

        internal abstract class HeapOrderedEnumerable<TElement> : IOrderedEnumerable<TElement>
        {
            private readonly int _numItems;
            private readonly IEnumerable<TElement> _source;
            private HeapOrderedEnumerable<TElement> _parent;

            protected HeapOrderedEnumerable(
                IEnumerable<TElement> source,
                int numItems)
            {
                _source = source;
                _numItems = numItems;
            }

            public IOrderedEnumerable<TElement> CreateOrderedEnumerable<TKey>(
                Func<TElement, TKey> keySelector,
                IComparer<TKey> comparer, bool @descending)
            {
                var oe = new HeapOrderedEnumerable<TElement, TKey>(
                    _source, _numItems, keySelector, comparer, descending) {_parent = this};
                return oe;
            }

            public IEnumerator<TElement> GetEnumerator()
            {
                int numRecs = 0;
                var recordKeySelector = new Func<TElement, int>(item => ++numRecs);

                // Add a ThenByDescending for the record key.
                // This ensures a stable ordering.
                var oe = (HeapOrderedEnumerable<TElement>) CreateOrderedEnumerable(recordKeySelector, null, true);

                // Get the ordering criteria, starting with the last ordering clause.
                // Which will always be the record key ordering.
                var criteria = oe.GetCriteria().ToArray();

                var selector = new HeapSelector<TElement>(_source, criteria, _numItems);
                return selector.DoSelect().GetEnumerator();
            }

            // Walks the ordering criteria to build an array that the HeapSelector can use.

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            private IEnumerable<HeapOrderedEnumerable<TElement>> GetCriteria()
            {
                var keys = new Stack<HeapOrderedEnumerable<TElement>>();

                HeapOrderedEnumerable<TElement> current = this;
                while (current != null)
                {
                    keys.Push(current);
                    current = current._parent;
                }
                return keys;
            }

            // The individual ordering criteria instances (HeapOrderedEnumerable<TElement, TKey>)
            // implement these abstract methods to provice key extraction, comparison, and swapping.
            public abstract void ExtractKey(TElement item, int ix);
            public abstract int CompareKeys(int x, int y);
            public abstract void SwapKeys(int x, int y);
        }

        internal class HeapOrderedEnumerable<TElement, TKey> : HeapOrderedEnumerable<TElement>
        {
            private readonly IComparer<TKey> _comparer;
            private readonly bool _descending;
            private readonly Func<TElement, TKey> _keySelector;

            private readonly TKey[] _keys;

            internal HeapOrderedEnumerable(
                IEnumerable<TElement> source,
                int numItems,
                Func<TElement, TKey> keySelector,
                IComparer<TKey> comparer,
                bool descending) : base(source, numItems)
            {
                _keySelector = keySelector;
                _comparer = comparer ?? Comparer<TKey>.Default;
                _descending = descending;

                // Allocate one extra key for the working item.
                _keys = new TKey[numItems + 1];
            }

            public override int CompareKeys(int x, int y)
            {
                return _descending
                    ? _comparer.Compare(_keys[y], _keys[x])
                    : _comparer.Compare(_keys[x], _keys[y]);
            }

            public override void SwapKeys(int x, int y)
            {
                TKey t = _keys[x];
                _keys[x] = _keys[y];
                _keys[y] = t;
            }

            public override void ExtractKey(TElement item, int ix)
            {
                _keys[ix] = _keySelector(item);
            }
        }

        internal class HeapSelector<TElement>
        {
            private readonly HeapOrderedEnumerable<TElement>[] _criteria;
            private readonly TElement[] _items;
            private readonly int _numItems;
            private readonly IEnumerable<TElement> _source;
            private int _count;

            public HeapSelector(
                IEnumerable<TElement> source,
                HeapOrderedEnumerable<TElement>[] criteria,
                int numItems)
            {
                _source = source;
                _criteria = criteria;
                _numItems = numItems;
                _items = new TElement[numItems + 1];
            }

            public IEnumerable<TElement> DoSelect()
            {
                // Build a heap from the first _numItems items
                IEnumerator<TElement> srcEnumerator = _source.GetEnumerator();
                while (_count < _numItems && srcEnumerator.MoveNext())
                {
                    ExtractKeys(srcEnumerator.Current, _count);
                    ++_count;
                }
                Heapify();

                // For each remaining item . . .
                while (srcEnumerator.MoveNext())
                {
                    if (ExtractCompare(srcEnumerator.Current, _numItems, _numItems, 0) <= 0) continue;
                    // The current item is larger than the smallest item.
                    // So move the current item to the root and sift down.
                    Swap(0, _numItems);
                    SiftDown(0);
                }

                // Top N items are on the heap. Sort them.
                int saveCount = _count;
                while (_count > 0)
                {
                    // The smallest item goes to the end of the array.
                    // The item that was at the end gets re-inserted in the heap.
                    // The result is an array that's in descending order.
                    --_count;
                    Swap(0, _count);
                    SiftDown(0);
                }

                // Have to use Take here because the _items array is
                // always larger than the number of items requested.
                return _items.Take(saveCount);
            }

            private void Heapify()
            {
                for (int i = _count/2; i >= 0; --i)
                {
                    SiftDown(i);
                }
            }

            private void SiftDown(int ix)
            {
                int child = (ix*2) + 1;
                while (child < _count)
                {
                    if (child + 1 < _count && Compare(child, child + 1) > 0)
                        ++child;
                    if (Compare(child, ix) >= 0)
                        break;
                    Swap(ix, child);
                    ix = child;
                    child = (ix*2) + 1;
                }
            }

            // Extract keys from the record into the array at index ix.
            // Also calls the ExtractKey method for each ordering criteria.
            // This method is called only during the initial heap build process.
            private void ExtractKeys(TElement item, int ix)
            {
                _items[ix] = item;
                foreach (var t in _criteria)
                {
                    t.ExtractKey(item, ix);
                }
            }

            // This method is a hybrid that extracts keys and does comparisons.
            // The idea is to extract only one key at a time, compare it, and early out if possible.
            // If the comparison results in the new item being smaller than the smallest item
            // already on the heap, then the item will not be inserted, so it make no sense to
            // extract all of its keys.
            // This optimization makes a small difference with a single selection criteria.
            // The difference should be much larger with multiple criteria.
            private int ExtractCompare(TElement item, int ix, int x, int y)
            {
                int rslt = 0;
                int i = 0;
                while (rslt == 0 && i < _criteria.Length)
                {
                    // Extract this key
                    _criteria[i].ExtractKey(item, ix);
                    rslt = _criteria[i].CompareKeys(x, y);
                    ++i;
                }
                // If the item isn't larger, we can short circuit
                if (rslt <= 0) return rslt;

                // The new item compares larger, so it will be placed on the heap.
                // Extract the rest of the keys
                _items[ix] = item;
                while (i < _criteria.Length)
                {
                    _criteria[i].ExtractKey(item, ix);
                    ++i;
                }
                return rslt;
            }

            // Walks the list of comparers, doing the comparisons.
            // The first unequal comparison short-circuits the loop.
            private int Compare(int x, int y)
            {
                int rslt = 0;
                for (int i = 0; rslt == 0 && i < _criteria.Length; ++i)
                {
                    rslt = _criteria[i].CompareKeys(x, y);
                }
                return rslt;
            }

            // Swap two items. This swaps the elements in the local array,
            // and calls the Swap method for each of the ordering criteria.
            private void Swap(int x, int y)
            {
                TElement temp = _items[x];
                _items[x] = _items[y];
                _items[y] = temp;
                foreach (var t in _criteria)
                {
                    t.SwapKeys(x, y);
                }
            }
        }
    }
}