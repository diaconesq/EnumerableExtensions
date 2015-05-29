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
    internal abstract class MergeByOrderedEnumerable<TSource>: IOrderedEnumerable<TSource>
    {
        private readonly IEnumerable<IEnumerable<TSource>> _lists;
        private MergeByOrderedEnumerable<TSource> _parent;

        protected MergeByOrderedEnumerable(IEnumerable<IEnumerable<TSource>> lists)
        {
            _lists = lists;
        }

        public IOrderedEnumerable<TSource> CreateOrderedEnumerable<TKey>(
            Func<TSource, TKey> keySelector,
            IComparer<TKey> comparer,
            bool @descending)
        {
            var oe = new MergeByOrderedEnumerable<TSource, TKey>(
                _lists, keySelector, comparer, @descending) { _parent = this };
            return oe;
        }

        public IEnumerator<TSource> GetEnumerator()
        {
            var criteria = GetCriteria().ToArray();
            var selector = new MergeBySelector<TSource>(_lists, criteria);
            return selector.DoSelect().GetEnumerator();
        }

        // Walks the ordering criteria to build an array that the HeapSelector can use.
        private IEnumerable<MergeByOrderedEnumerable<TSource>> GetCriteria()
        {
            var keys = new Stack<MergeByOrderedEnumerable<TSource>>();

            var current = this;
            while (current != null)
            {
                keys.Push(current);
                current = current._parent;
            }
            return keys;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public abstract void CreateKeys(int count);
        public abstract void ExtractKey(TSource item, int ix);
        public abstract int CompareKeys(int x, int y);
        public abstract void SwapKeys(int x, int y);
    }

    internal class MergeByOrderedEnumerable<TSource, TKey> : MergeByOrderedEnumerable<TSource>
    {
        private readonly Func<TSource, TKey> _keySelector;
        private readonly IComparer<TKey> _comparer;
        private readonly bool _descending;
        private TKey[] _keys;

        public MergeByOrderedEnumerable(
            IEnumerable<IEnumerable<TSource>> lists,
            Func<TSource, TKey> keySelector,
            IComparer<TKey> comparer,
            bool descending)
            : base(lists)
        {
            _keySelector = keySelector;
            _comparer = comparer ?? Comparer<TKey>.Default;
            _descending = @descending;
        }

        public override void CreateKeys(int count)
        {
            _keys = new TKey[count];
        }

        public override void ExtractKey(TSource item, int ix)
        {
            _keys[ix] = _keySelector(item);
        }

        public override int CompareKeys(int x, int y)
        {
            if (_descending)
                return _comparer.Compare(_keys[y], _keys[x]);

            return _comparer.Compare(_keys[x], _keys[y]);
        }

        public override void SwapKeys(int x, int y)
        {
            var t = _keys[x];
            _keys[x] = _keys[y];
            _keys[y] = t;
        }
    }

    internal class MergeBySelector<TSource>
    {
        private readonly IEnumerable<IEnumerable<TSource>> _lists;
        private readonly MergeByOrderedEnumerable<TSource>[] _criteria;

        // The heap
        private IEnumerator<TSource>[] _items;
        private int _count;

        public MergeBySelector(
            IEnumerable<IEnumerable<TSource>> lists,
            MergeByOrderedEnumerable<TSource>[] criteria)
        {
            _lists = lists;
            _criteria = criteria;
        }

        public IEnumerable<TSource> DoSelect()
        {
            // Get an array of all the lists that have items
            _items = _lists
                .Select(l => l.GetEnumerator())
                .Where(i => i.MoveNext())
                .ToArray();

            // and initialize the keys in each of the criteria
            foreach (var x in _criteria)
            {
                x.CreateKeys(_items.Length);
            }

            // Extract the keys for each of the items in the array
            _count = 0;
            foreach (var l in _items)
            {
                ExtractKeys(l, _count);
                ++_count;
            }
            // And turn the array into a heap
            Heapify();

            // Now do the merge.
            while (_count > 0)
            {
                var lowest = _items[0];
                yield return lowest.Current;
                if (lowest.MoveNext())
                {
                    ExtractKeys(lowest, 0);
                    SiftDown(0);
                }
                else
                {
                    RemoveRoot();
                }
            }
        }

        private IEnumerator<TSource> RemoveRoot()
        {
            // remove the first item on the heap
            var lowest = _items[0];
            Swap(0, _count - 1);
            --_count;
            if (_count > 0)
            {
                SiftDown(0);
            }
            return lowest;
        }

        private void Heapify()
        {
            for (int i = _count / 2; i >= 0; --i)
            {
                SiftDown(i);
            }
        }

        private void SiftDown(int ix)
        {
            var child = (ix * 2) + 1;
            while (child < _count)
            {
                if (child + 1 < _count && Compare(child, child + 1) > 0)
                    ++child;
                if (Compare(child, ix) >= 0)
                    break;
                Swap(ix, child);
                ix = child;
                child = (ix * 2) + 1;
            }
        }

        // Extract keys from the record into the array at index ix.
        // Also calls the ExtractKey method for each ordering criteria.
        private void ExtractKeys(IEnumerator<TSource> item, int ix)
        {
            _items[ix] = item;
            foreach (var t in _criteria)
            {
                t.ExtractKey(item.Current, ix);
            }
        }

        // Walks the list of comparers, doing the comparisons.
        // The first unequal comparison short-circuits the loop.
        private int Compare(int x, int y)
        {
            var rslt = 0;
            for (var i = 0; rslt == 0 && i < _criteria.Length; ++i)
            {
                rslt = _criteria[i].CompareKeys(x, y);
            }
            return rslt;
        }

        // Swap two items. This swaps the elements in the local array,
        // and calls the Swap method for each of the ordering criteria.
        private void Swap(int x, int y)
        {
            var temp = _items[x];
            _items[x] = _items[y];
            _items[y] = temp;
            foreach (var t in _criteria)
            {
                t.SwapKeys(x, y);
            }
        }
    }

}