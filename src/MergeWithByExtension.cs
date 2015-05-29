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
    public static class MergeWithByExtension
    {
        public static IOrderedEnumerable<TSource> MergeBy<TSource, TKey>(
            this IEnumerable<TSource> list1,
            IEnumerable<TSource> list2,
            Func<TSource, TKey> keySelector)
        {
            return MergeBy(list1, list2, keySelector, null);
        }

        public static IOrderedEnumerable<TSource> MergeBy<TSource, TKey>(
            this IEnumerable<TSource> list1,
            IEnumerable<TSource> list2,
            Func<TSource, TKey> keySelector,
            IComparer<TKey> comparer)
        {
            return new MergeOrderedEnumerable<TSource, TKey>(list1, list2, keySelector, comparer, false);
        }

        public static IOrderedEnumerable<TSource> MergeByDescending<TSource, TKey>(
            this IEnumerable<TSource> list1,
            IEnumerable<TSource> list2,
            Func<TSource, TKey> keySelector)
        {
            return MergeByDescending(list1, list2, keySelector, null);
        }

        public static IOrderedEnumerable<TSource> MergeByDescending<TSource, TKey>(
            this IEnumerable<TSource> list1,
            IEnumerable<TSource> list2,
            Func<TSource, TKey> keySelector,
            IComparer<TKey> comparer)
        {
            return new MergeOrderedEnumerable<TSource, TKey>(list1, list2, keySelector, comparer, true);
        }

        internal abstract class MergeOrderedEnumerable<TSource> : IOrderedEnumerable<TSource>
        {
            private readonly IEnumerable<TSource> _list1;
            private readonly IEnumerable<TSource> _list2;
            internal MergeOrderedEnumerable<TSource> Parent;

            protected MergeOrderedEnumerable(
                IEnumerable<TSource> list1,
                IEnumerable<TSource> list2)
            {
                _list1 = list1;
                _list2 = list2;
            }

            public IOrderedEnumerable<TSource> CreateOrderedEnumerable<TKey>(
                Func<TSource, TKey> keySelector,
                IComparer<TKey> comparer,
                bool @descending)
            {
                var oe = new MergeOrderedEnumerable<TSource, TKey>(
                    _list1, _list2, keySelector, comparer, descending) {Parent = this};
                return oe;
            }

            public IEnumerator<TSource> GetEnumerator()
            {
                var criteria = GetCriteria().ToArray();

                var selector = new MergeSelector<TSource>(_list1, _list2, criteria);
                return selector.DoSelect().GetEnumerator();
            }

            // Walks the ordering criteria to build an array that the HeapSelector can use.
            private IEnumerable<MergeOrderedEnumerable<TSource>> GetCriteria()
            {
                var keys = new Stack<MergeOrderedEnumerable<TSource>>();

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

            // The individual ordering criteria instances (MergeOrderedEnumerable<TElement, TKey>)
            // implement these abstract methods to provice key extraction, comparison, and swapping.
            public abstract void ExtractKey(TSource item, int ix);
            public abstract int CompareKeys(int x, int y);
        }

        internal class MergeOrderedEnumerable<TSource, TKey> : MergeOrderedEnumerable<TSource>
        {
            private readonly Func<TSource, TKey> _keySelector;
            private readonly IComparer<TKey> _comparer;
            private readonly bool _descending;
            private readonly TKey[] _keys;

            internal MergeOrderedEnumerable(
                IEnumerable<TSource> list1,
                IEnumerable<TSource> list2,
                Func<TSource, TKey> keySelector,
                IComparer<TKey> comparer,
                bool descending)
                : base(list1, list2)
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

        internal class MergeSelector<TSource>
        {
            private readonly IEnumerable<TSource> _list1;
            private readonly IEnumerable<TSource> _list2;
            private readonly MergeOrderedEnumerable<TSource>[] _criteria;

            public MergeSelector(
                IEnumerable<TSource> list1,
                IEnumerable<TSource> list2,
                MergeOrderedEnumerable<TSource>[] criteria)
            {
                _list1 = list1;
                _list2 = list2;
                _criteria = criteria;
            }

            public IEnumerable<TSource> DoSelect()
            {
                // Initialize the iterators
                var iterators = new IEnumerator<TSource>[2];

                var next = new Func<int, bool>(ix =>
                {
                    if (!iterators[ix].MoveNext()) return false;
                    ExtractKeys(iterators[ix].Current, ix);
                    return true;
                });

                iterators[0] = _list1.GetEnumerator();
                iterators[1] = _list2.GetEnumerator();
                var i1HasItems = next(0);
                var i2HasItems = next(1);
                while (i1HasItems && i2HasItems)
                {
                    if (Compare(0, 1) <= 0)
                    {
                        yield return iterators[0].Current;
                        i1HasItems = next(0);
                    }
                    else
                    {
                        yield return iterators[1].Current;
                        i2HasItems = next(1);
                    }
                }

                while (i1HasItems)
                {
                    yield return iterators[0].Current;
                    i1HasItems = next(0);
                }

                while (i2HasItems)
                {
                    yield return iterators[1].Current;
                    i2HasItems = next(1);
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
 