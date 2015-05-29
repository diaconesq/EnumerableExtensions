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
using System.Collections.Generic;
using System.Linq;

namespace Mischel.EnumerableExtensions
{
    public static class LinqMethods
    {
        public static IOrderedEnumerable<TSource> MergeBy<TSource, TKey>(
            Func<TSource, TKey> keyExtractor,
            params IEnumerable<TSource>[] lists)
        {
            return MergeBy(lists, keyExtractor);
        }
        public static IOrderedEnumerable<TSource> MergeBy<TSource, TKey>(
            Func<TSource, TKey> keyExtractor,
            IComparer<TKey> comparer,
            params IEnumerable<TSource>[] lists)
        {
            return MergeBy(lists, keyExtractor, comparer);
        }

        public static IOrderedEnumerable<TSource> MergeBy<TSource, TKey>(
            IEnumerable<IEnumerable<TSource>> lists,
            Func<TSource, TKey> keyExtractor)
        {
            return MergeBy(lists, keyExtractor, null);
        }

        public static IOrderedEnumerable<TSource> MergeBy<TSource, TKey>(
            IEnumerable<IEnumerable<TSource>> lists,
            Func<TSource, TKey> keyExtractor,
            IComparer<TKey> comparer)
        {
            return new MergeByOrderedEnumerable<TSource, TKey>(lists, keyExtractor, comparer, false);
        }

        public static IOrderedEnumerable<TSource> MergeByDescending<TSource, TKey>(
            IEnumerable<IEnumerable<TSource>> lists,
            Func<TSource, TKey> keyExtractor)
        {
            return MergeByDescending(lists, keyExtractor, null);
        }

        public static IOrderedEnumerable<TSource> MergeByDescending<TSource, TKey>(
            IEnumerable<IEnumerable<TSource>> lists,
            Func<TSource, TKey> keyExtractor,
            IComparer<TKey> comparer)
        {
            return new MergeByOrderedEnumerable<TSource, TKey>(lists, keyExtractor, comparer, true);
        }
    }
 }
