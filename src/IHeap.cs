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

using System.Collections.Generic;

namespace Mischel.EnumerableExtensions
{
    /// <summary>
    /// Represents a strongly typed heap of objects.
    /// </summary>
    /// <typeparam name="T">The type of elements in the heap</typeparam>
    public interface IHeap<T>: IEnumerable<T>
    {
        /// <summary>
        /// Gets the number of elements actually contained in the IHeap&lt;T&gt;.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Removes all items from the IHeap&lt;T&gt;.
        /// </summary>
        void Clear();

        /// <summary>
        /// Provides a consuming IEnumerable for items in the collection.
        /// </summary>
        /// <returns>An IEnumerable that removes and returns items from the collection, in order.</returns>
        IEnumerable<T> GetConsumingEnumerable();

        /// <summary>
        /// Inserts an item into the IHeap&lt;T&gt;.
        /// </summary>
        /// <param name="item">The object to be added to the Heap. The value cannot be null.</param>
        void Insert(T item);

        /// <summary>
        /// Returns the root element from the IHeap&lt;T&gt;, without removing it.
        /// </summary>
        /// <returns>The root element.</returns>
        T Peek();

        /// <summary>
        /// Removes and returns the root element from the IHeap&lt;T&gt;.
        /// </summary>
        /// <returns>The root element.</returns>
        T RemoveRoot();
    }
}