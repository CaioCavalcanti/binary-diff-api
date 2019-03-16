using BinaryDiff.Domain.Enum;
using BinaryDiff.Domain.Logic;
using BinaryDiff.Domain.Logic.Implementation;
using BinaryDiff.Domain.Models;
using System;
using System.Collections.Generic;
using Xunit;

namespace BinaryDiff.Tests.Unit.Domain
{
    /// <summary>
    /// Tests for diff logic
    /// </summary>
    public class DiffLogicTests
    {
        private static IDiffLogic _logic => new DiffLogic();

        /// <summary>
        /// Tests diff results for given scenarios
        /// </summary>
        /// <param name="left">String data for left position</param>
        /// <param name="right">String data for right position</param>
        /// <param name="expectedResult">Expected ResultType</param>
        /// <param name="expectedDifferences">Expected differences as dictionary, where key is offset and value is length</param>
        [Theory]
        [MemberData(nameof(TestData))]
        public void GetResult_ShouldReturnDiffResult(string left, string right, ResultType expectedResult, IDictionary<int, int> expectedDifferences)
        {
            var diff = new Diff()
            {
                Id = Guid.NewGuid(),
                Left = left,
                Right = right
            };

            var result = _logic.GetResult(diff);

            Assert.Equal(result.Id, result.Id);
            Assert.Equal(expectedResult, result.Result);
            Assert.Equal(expectedDifferences, result.Differences);
        }

        /// <summary>
        /// Test data for different scenarios with expected results
        /// </summary>
        public static IEnumerable<object[]> TestData => new List<object[]>
        {
            // Equal scenarios
            ExpectedResult(null, null, ResultType.Equal),
            ExpectedResult("", "", ResultType.Equal),
            ExpectedResult("abc", "abc", ResultType.Equal),
            
            // Left > right scenarios
            ExpectedResult("abc", null, ResultType.LeftIsLarger),
            ExpectedResult("abc", "", ResultType.LeftIsLarger),
            ExpectedResult("abc", "a", ResultType.LeftIsLarger),

            // Right > left scenarios
            ExpectedResult(null, "abc", ResultType.RightIsLarger),
            ExpectedResult("", "abc", ResultType.RightIsLarger),
            ExpectedResult("a", "abc", ResultType.RightIsLarger),

            // Different scenarios
            ExpectedResult("abc", "xbc", ResultType.Different, new Dictionary<int, int> { { 0, 1 } }),
            ExpectedResult("abc", "axc", ResultType.Different, new Dictionary<int, int> { { 1, 1 } }),
            ExpectedResult("abc", "xyz", ResultType.Different, new Dictionary<int, int> { { 0, 3 } }),
            ExpectedResult("abc", "axx", ResultType.Different, new Dictionary<int, int> { { 1, 2 } }),
            ExpectedResult("abc", "abx", ResultType.Different, new Dictionary<int, int> { { 2, 1 } }),
            ExpectedResult("abc", "xbx", ResultType.Different, new Dictionary<int, int> { { 0, 1 }, { 2, 1 } }),
            ExpectedResult("abc", "xxc", ResultType.Different, new Dictionary<int, int> { { 0, 2 } })
        };

        private static object[] ExpectedResult(string left, string right, ResultType result, IDictionary<int, int> differences = null)
            => new object[] { left, right, result, differences };
    }
}
