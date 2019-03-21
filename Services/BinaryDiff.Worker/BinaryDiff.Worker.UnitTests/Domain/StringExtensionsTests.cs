using BinaryDiff.Worker.Domain.Utils;
using System;
using System.Collections.Generic;
using Xunit;

namespace BinaryDiff.Worker.UnitTests.Domain
{
    public class StringExtensionsTests
    {
        /// <summary>
        /// Tests exception if trying to compare string from different sizes
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Theory]
        [InlineData("abc", "")]
        [InlineData("abc", null)]
        [InlineData(null, "abc")]
        public void EqualsToSameSizeString_throws_InvalidOperationException_if_different_sizes(string a, string b)
        {
            Assert.Throws<InvalidOperationException>(() => a.EqualsToSameSizeString(b, out var differences));
        }

        /// <summary>
        /// Tests same size string comparison for given scenarios provided as member data
        /// </summary>
        /// <param name="a">String instance</param>
        /// <param name="b">String to compare with</param>
        /// <param name="areEqual">Expected comparison result</param>
        /// <param name="expectedDifferences">Expected differences as dict where key is offset and value is length</param>
        [Theory]
        [MemberData(nameof(TestDataForSameSizeStrings))]
        public void EqualsToSameSizeString_returns_expected_result(string a, string b, bool areEqual, IDictionary<int, int> expectedDifferences)
        {
            var result = a.EqualsToSameSizeString(b, out var differences);

            Assert.Equal(areEqual, result);
            Assert.Equal(expectedDifferences, differences);
        }

        /// <summary>
        /// Tests string size length for given scenarios provided inline
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="expect">Expected result</param>
        [Theory]
        [InlineData(null, null, false)]
        [InlineData("abc", "ab", true)]
        [InlineData("abc", "", true)]
        [InlineData(null, "abc", false)]
        [InlineData("ab", "abc", false)]
        [InlineData("", "abc", false)]
        [InlineData("abc", null, true)]
        public void IsLargerThan_returns_expected_results(string a, string b, bool expect)
        {
            var result = a.IsLargerThan(b);

            Assert.Equal(expect, result);
        }

        /// <summary>
        /// Test data in different scenarios to be tested on EqualsToSameSizeString
        /// </summary>
        public static IEnumerable<object[]> TestDataForSameSizeStrings => new List<object[]>
        {
            new object[] { "", "", true, null },
            new object[] { "abc", "abc", true, null },
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
