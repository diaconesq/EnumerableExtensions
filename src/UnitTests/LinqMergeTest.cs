using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Mischel.EnumerableExtensions;
using NUnit.Framework;

namespace Helpers.UnitTests
{
    [TestFixture]
    public class LinqMergeTest
    {

        [Test]
        public void MergeBy_WithSimpleOrderedLists_ReturnsOrderedList()
        {
            var list1 = new[] {1, 3, 5, 7};
            var list2 = new[] {2, 4, 6, 8};
            var list3 = new[] {-1, 11};

            var result = LinqMethods.MergeBy(x => x, list1, list2, list3);
            result.Should().Equal(-1, 1, 2, 3, 4, 5, 6, 7, 8, 11);
        }

        [Test]
        [Timeout(5000)]
        public void MergeBy_WithInfiniteLists_DoesNotHang()
        {
            var infiniteList1 = GetAllLongs();
            var infiniteList2 = GetAllLongs();
            var result = LinqMethods.MergeBy(x => x, infiniteList1, infiniteList2)
                .Take(100000).ToList();

            result.Should().BeInAscendingOrder()
                .And.HaveCount(100000);
        }

        private IEnumerable<long> GetAllLongs(long? start = null, int? step = null, long? end = null)
        {
            var _start = start ?? long.MinValue;
            var _step = step ?? 1;
            var _end = end ?? long.MaxValue;

            for (var i = _start; i <= _end - _step; i += _step)
            {
                yield return i;
            }
        }

        [Test]
        [Timeout(5000)]
        public void MergeBy_WithLargeNumberOfLists_DoesNotHang()
        {
            var listsNo = 5000;
            var random = new Random(0); //random, but the same random every time...
            var lists = Enumerable.Range(1, listsNo)
                .Select(_ =>
                {
                    var start = random.Next();
                    var step = random.Next(1,100); //positive
                    var end = start + step * random.Next(0, 10);
                    return GetAllLongs(start, step, end);
                });

            var result = LinqMethods.MergeBy(lists, x => x).ToList();

            result.Should().BeInAscendingOrder();
        }
    }

}
