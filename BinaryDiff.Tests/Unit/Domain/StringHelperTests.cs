using BinaryDiff.Domain.Helpers;
using System;
using System.Collections.Generic;
using Xunit;

namespace BinaryDiff.Tests.Unit.Domain
{
    public class StringHelperTests
    {
        [Fact]
        public void CompareToSameSizeString_throws_InvalidOperationException_if_different_sizes()
        {
            var a = "abc";
            var b = "a";

            Assert.Throws<InvalidOperationException>(() => a.CompareToSameSizeString(b));
        }

        [Theory]
        [InlineData("abc", null)]
        [InlineData(null, "abc")]
        public void CompareToSameSizeString_throws_NullReferenceException_if_any_are_null(string a, string b)
        {
            Assert.Throws<NullReferenceException>(() => a.CompareToSameSizeString(b));
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void CompareToSameSizeString_returns_differences_as_dict_with_offset_and_length(string a, string b, IDictionary<int, int> expected)
        {
            var result = a.CompareToSameSizeString(b);

            Assert.Equal(expected, result);
        }

        public static IEnumerable<object[]> TestData => new List<object[]>
        {
            new object[] { "", "",  new Dictionary<int, int>() },
            new object[] { "abc", "abc",  new Dictionary<int, int>() },
            new object[] { "abc", "xyz",  new Dictionary<int, int> { { 0, 3 } } },
            new object[] { "abc", "xbc",  new Dictionary<int, int> { { 0, 1 } } },
            new object[] { "abc", "axc",  new Dictionary<int, int> { { 1, 1 } } },
            new object[] { "abc", "axx",  new Dictionary<int, int> { { 1, 2 } } },
            new object[] { "abc", "abx",  new Dictionary<int, int> { { 2, 1 } } },
            new object[] { "abc", "xbx",  new Dictionary<int, int> { { 0, 1 }, { 2, 1 } } },
            new object[] { "abc", "xxc",  new Dictionary<int, int> { { 0, 2 } } }
        };
    }
}
