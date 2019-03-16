using BinaryDiff.Domain.Helpers;
using System;
using System.Collections.Generic;
using Xunit;

namespace BinaryDiff.Tests.Unit.Domain
{
    public class StringHelperTests
    {
        [Theory]
        [InlineData("abc", "")]
        [InlineData("abc", null)]
        [InlineData(null, "abc")]
        public void EqualsToSameSizeString_IfDifferentSizes_ThrowsInvalidOperationException(string a, string b)
        {
            Assert.Throws<InvalidOperationException>(() => a.EqualsToSameSizeString(b, out var differences));
        }

        [Theory]
        [MemberData(nameof(TestDataForSameSizeStrings))]
        public void EqualsToSameSizeString_ReturnsEqualityResult_OutDifferencesOnDict(string a, string b, bool areEqual, IDictionary<int, int> expectedDifferences)
        {
            var result = a.EqualsToSameSizeString(b, out var differences);

            Assert.Equal(areEqual, result);
            Assert.Equal(expectedDifferences, differences);
        }

        [Theory]
        [InlineData(null, null, false)]
        [InlineData("abc", "ab", true)]
        [InlineData("abc", "", true)]
        [InlineData(null, "abc", false)]
        [InlineData("ab", "abc", false)]
        [InlineData("", "abc", false)]
        [InlineData("abc", null, true)]
        public void IsLargerThan_Returns(string a, string b, bool expect)
        {
            var result = a.IsLargerThan(b);

            Assert.Equal(expect, result);
        }

        public static IEnumerable<object[]> TestDataForSameSizeStrings => new List<object[]>
        {
            new object[] { "", "", true, new Dictionary<int, int>() },
            new object[] { "abc", "abc", true, new Dictionary<int, int>() },
            new object[] { "abc", "xyz", false, new Dictionary<int, int> { { 0, 3 } } },
            new object[] { "abc", "xbc", false, new Dictionary<int, int> { { 0, 1 } } },
            new object[] { "abc", "axc", false, new Dictionary<int, int> { { 1, 1 } } },
            new object[] { "abc", "axx", false, new Dictionary<int, int> { { 1, 2 } } },
            new object[] { "abc", "abx", false, new Dictionary<int, int> { { 2, 1 } } },
            new object[] { "abc", "xbx", false, new Dictionary<int, int> { { 0, 1 }, { 2, 1 } } },
            new object[] { "abc", "xxc", false, new Dictionary<int, int> { { 0, 2 } } }
        };
    }
}
