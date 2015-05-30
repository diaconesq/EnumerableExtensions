using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Mischel.EnumerableExtensions;
using NUnit.Framework;

namespace Helpers.UnitTests
{
    [TestFixture]
    public class LinqMergeTestWithComplexObjects
    {
        private static readonly DateTime[] _dates1 = 
        {
            new DateTime(2005, 5, 10),
            new DateTime(2005, 5, 12),
            new DateTime(2005, 5, 13),
        };

        private static readonly DateTime[] _dates2 = 
        {
            new DateTime(2005, 5, 11),
            new DateTime(2005, 5, 12),
            new DateTime(2005, 5, 13),
            new DateTime(2005, 5, 15),
        };

        private static readonly IEnumerable<ItemWithDate> _items1 = _dates1.Select(day => new ItemWithDate(day, "item1"));
        private static readonly IEnumerable<ItemWithDate> _items2 = _dates2.Select(day => new ItemWithDate(day, "item2"));

        private static readonly ItemWithDate[] _expectation = 
        {
            new ItemWithDate(new DateTime(2005, 5, 10), "item1"),
            new ItemWithDate(new DateTime(2005, 5, 11), "item2"),
            new ItemWithDate(new DateTime(2005, 5, 12), "item1"), 
            new ItemWithDate(new DateTime(2005, 5, 12), "item2"),
            new ItemWithDate(new DateTime(2005, 5, 13), "item1"),
            new ItemWithDate(new DateTime(2005, 5, 13), "item2"),
            new ItemWithDate(new DateTime(2005, 5, 15), "item2")
        };

        private class ItemWithDate
        {
            public DateTime Day { get; private set; }

            public string Title { get; private set; }

            public ItemWithDate(DateTime day, string title)
            {
                Day = day;
                Title = title;
            }

            protected bool Equals(ItemWithDate other)
            {
                return Day.Equals(other.Day) && string.Equals(Title, other.Title);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((ItemWithDate) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Day.GetHashCode()*397) ^ (Title != null ? Title.GetHashCode() : 0);
                }
            }
        }

        [Test]
        public void MergeBy_Date_ReturnsAllItemsOrderedByDate()
        {
            var result = LinqMethods
                .MergeBy(new[] { _items1, _items2 }, x => x.Day);

            result.Should()
                .BeInAscendingOrder(item => item.Day)
                .And.BeEquivalentTo(_expectation, "some items were not found in the result") 
                ;
        }
        [Test]
        public void MergeBy_DateAndTitle_ReturnsExactlyExpectedResult()
        {
            var result = LinqMethods
                .MergeBy(
                    new[] { _items1, _items2 },
                    x => x.Day)
                .ThenBy(x => x.Title);

            result.Should().Equal(_expectation);

        }
    }
}