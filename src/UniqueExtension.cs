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
    public static class UniqueExtension
    {
        public static IOrderedEnumerable<TSource> UniqueBy<TSource, TKey>(
            this IEnumerable<TSource> list1,
            Func<TSource, TKey> keySelector)
        {
            return UniqueBy(list1, keySelector, null);
        }

        public static IOrderedEnumerable<TSource> UniqueBy<TSource, TKey>(
            this IEnumerable<TSource> list1,
            Func<TSource, TKey> keySelector,
            IComparer<TKey> comparer)
        {
            return new UniqueOrderedEnumerable<TSource, TKey>(list1, keySelector, comparer, false);
        }

        public static IOrderedEnumerable<TSource> UniqueByDescending<TSource, TKey>(
            this IEnumerable<TSource> list1,
            Func<TSource, TKey> keySelector)
        {
            return UniqueByDescending(list1, keySelector, null);
        }

        public static IOrderedEnumerable<TSource> UniqueByDescending<TSource, TKey>(
            this IEnumerable<TSource> list1,
            Func<TSource, TKey> keySelector,
            IComparer<TKey> comparer)
        {
            return new UniqueOrderedEnumerable<TSource, TKey>(list1, keySelector, comparer, true);
        }

        internal abstract class UniqueOrderedEnumerable<TSource> : IOrderedEnumerable<TSource>
        {
            private readonly IEnumerable<TSource> _list1;
            internal UniqueOrderedEnumerable<TSource> Parent;

            protected UniqueOrderedEnumerable(
                IEnumerable<TSource> list1)
            {
                _list1 = list1;
            }

            public IOrderedEnumerable<TSource> CreateOrderedEnumerable<TKey>(
                Func<TSource, TKey> keySelector,
                IComparer<TKey> comparer,
                bool @descending)
            {
                var oe = new UniqueOrderedEnumerable<TSource, TKey>(
                    _list1, keySelector, comparer, descending) { Parent = this };
                return oe;
            }

            public IEnumerator<TSource> GetEnumerator()
            {
                var criteria = GetCriteria().ToArray();

                var selector = new UniqueSelector<TSource>(_list1, criteria);
                return selector.DoSelect().GetEnumerator();
            }

            // Walks the ordering criteria to build an array that the HeapSelector can use.
            private IEnumerable<UniqueOrderedEnumerable<TSource>> GetCriteria()
            {
                var keys = new Stack<UniqueOrderedEnumerable<TSource>>();

                var current = this;
                while (current != null)
                {
                    keys.Push(current);
                    current = current.Parent;
                }
                return keys;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            // The individual ordering criteria instances (UniqueOrderedEnumerable<TElement, TKey>)
            // implement these abstract methods to provice key extraction, comparison, and swapping.
            public abstract void ExtractKey(TSource item, int ix);
            public abstract int CompareKeys(int x, int y);
        }

        internal class UniqueOrderedEnumerable<TSource, TKey> : UniqueOrderedEnumerable<TSource>
        {
            private readonly Func<TSource, TKey> _keySelector;
            private readonly IComparer<TKey> _comparer;
            private readonly bool _descending;
            private readonly TKey[] _keys;

            internal UniqueOrderedEnumerable(
                IEnumerable<TSource> list1,
                Func<TSource, TKey> keySelector,
                IComparer<TKey> comparer,
                bool descending)
                : base(list1)
            {
                _keySelector = keySelector;
                _comparer = comparer ?? Comparer<TKey>.Default;
                _descending = descending;

                _keys = new TKey[2];
            }

            public override int CompareKeys(int x, int y)
            {
                return _descending
                    ? _comparer.Compare(_keys[y], _keys[x])
                    : _comparer.Compare(_keys[x], _keys[y]);
            }

            public override void ExtractKey(TSource item, int ix)
            {
                _keys[ix] = _keySelector(item);
            }
        }

        internal class UniqueSelector<TSource>
        {
            private readonly IEnumerable<TSource> _list1;
            private readonly UniqueOrderedEnumerable<TSource>[] _criteria;

            public UniqueSelector(
                IEnumerable<TSource> list1,
                UniqueOrderedEnumerable<TSource>[] criteria)
            {
                _list1 = list1;
                _criteria = criteria;
            }

            public IEnumerable<TSource> DoSelect()
            {
                // Initialize the iterator
                var i1 = _list1.GetEnumerator();

                // ix controls the key position that's loaded
                var ix = 0;

                var next = new Func<bool>(() =>
                {
                    if (!i1.MoveNext()) return false;
                    ExtractKeys(i1.Current, ix);
                    return true;
                });

                var i1HasItems = next();
                if (!i1HasItems) yield break;

                // output the first item
                yield return i1.Current;
                ix = 1;
                i1HasItems = next();
                while (i1HasItems)
                {
                    // Output the item if it's not equal to the previous item
                    if (Compare(0, 1) != 0)
                    {
                        yield return i1.Current;

                        // Change the index position for the next loaded key
                        ix = (ix == 0) ? 1 : 0;
                    }
                    i1HasItems = next();
                }
            }

            private int Compare(int x, int y)
            {
                var rslt = 0;
                for (var i = 0; rslt == 0 && i < _criteria.Length; ++i)
                {
                    rslt = _criteria[i].CompareKeys(x, y);
                }
                return rslt;
            }

            private void ExtractKeys(TSource item, int ix)
            {
                foreach (var t in _criteria)
                {
                    t.ExtractKey(item, ix);
                }
            }
        }
    }
}
