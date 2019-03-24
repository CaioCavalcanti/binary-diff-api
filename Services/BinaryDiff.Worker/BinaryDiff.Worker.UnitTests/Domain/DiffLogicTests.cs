using BinaryDiff.Worker.Domain.Enums;
using BinaryDiff.Worker.Domain.Logic;
using BinaryDiff.Worker.Domain.Models;
using System.Collections.Generic;
using Xunit;

namespace BinaryDiff.Worker.UnitTests.Domain
{
    public class DiffLogicTests
    {
        private static IDiffLogic _logic => new DiffLogic();

        /// <summary>
        /// Tests diff results for given scenarios
        /// </summary>
        /// <param name="left">Input data on left position</param>
        /// <param name="right">Input data on right position</param>
        /// <param name="expectedResult">Expected DiffResult</param>
        [Theory]
        [MemberData(nameof(ExpectEqual))]
        [MemberData(nameof(ExpectLeftIsLarger))]
        [MemberData(nameof(ExpectRightIsLarger))]
        [MemberData(nameof(ExpectDifferentWithDifferences))]
        public void CompareData_returns_expected_result(InputData left, InputData right, DiffResult expectedResult)
        {
            var result = _logic.CompareData(left, right);

            Assert.Equal(expectedResult.Result, result.Result);
            Assert.Equal(expectedResult.Differences, result.Differences);
        }

        /// <summary>
        /// Tests if CompareData handles result for different input positions
        /// </summary>
        /// <param name="largerInputPosition">Input position that should be larger</param>
        /// <param name="smallerInputPosition">Input position that should be smaller</param>
        /// <param name="expectedResult">Expected result type</param>
        [Theory]
        [InlineData(InputPosition.Left, InputPosition.Right, ResultType.LeftIsLarger)]
        [InlineData(InputPosition.Right, InputPosition.Left, ResultType.RightIsLarger)]
        public void CompareData_returns_expected_result_no_matter_input_position(
            InputPosition largerInputPosition,
            InputPosition smallerInputPosition,
            ResultType expectedResult)
        {
            var input = new InputData() { Data = "aaa", Position = largerInputPosition };
            var opposingInput = new InputData { Data = "a", Position = smallerInputPosition };

            var result = _logic.CompareData(input, opposingInput);

            Assert.Equal(result.Result, expectedResult);
        }

        /// <summary>
        /// Tests if GetOpposing position returns the correct opposing position
        /// </summary>
        /// <param name="position">Position to compare</param>
        /// <param name="expectedOpposingPosition">Expected opposing position</param>
        [Theory]
        [InlineData(InputPosition.Left, InputPosition.Right)]
        [InlineData(InputPosition.Right, InputPosition.Left)]
        public void GetOpposingPosition_returns_expected_opposing_position_for_given_position(
            InputPosition position,
            InputPosition expectedOpposingPosition
        )
        {
            var result = _logic.GetOpposingPosition(position);

            Assert.Equal(result, expectedOpposingPosition);
        }

        /// <summary>
        /// Resolve test data for scenarios where both inputs are equal
        /// </summary>
        public static IEnumerable<object[]> ExpectEqual => new List<object[]>
        {
            NewExpectedResult(null, null, ResultType.Equal),
            NewExpectedResult("", "", ResultType.Equal),
            NewExpectedResult("abc", "abc", ResultType.Equal)
        };

        /// <summary>
        /// Resolve test data for different scenarios where left is larger
        /// </summary>
        public static IEnumerable<object[]> ExpectLeftIsLarger => new List<object[]>
        {
            NewExpectedResult("abc", null, ResultType.LeftIsLarger),
            NewExpectedResult("abc", "", ResultType.LeftIsLarger),
            NewExpectedResult("abc", "a", ResultType.LeftIsLarger)
        };

        /// <summary>
        /// Resolve test data for different scenarios with expected results
        /// </summary>
        public static IEnumerable<object[]> ExpectRightIsLarger => new List<object[]>
        {
            NewExpectedResult(null, "abc", ResultType.RightIsLarger),
            NewExpectedResult("", "abc", ResultType.RightIsLarger),
            NewExpectedResult("a", "abc", ResultType.RightIsLarger),
        };

        /// <summary>
        /// Resolve test data for different scenarios with expected results
        /// </summary>
        public static IEnumerable<object[]> ExpectDifferentWithDifferences => new List<object[]>
        {
            NewExpectedResult("abc", "xbc", ResultType.Different, new Dictionary<int, int> { { 0, 1 } }),
            NewExpectedResult("abc", "axc", ResultType.Different, new Dictionary<int, int> { { 1, 1 } }),
            NewExpectedResult("abc", "xyz", ResultType.Different, new Dictionary<int, int> { { 0, 3 } }),
            NewExpectedResult("abc", "axx", ResultType.Different, new Dictionary<int, int> { { 1, 2 } }),
            NewExpectedResult("abc", "abx", ResultType.Different, new Dictionary<int, int> { { 2, 1 } }),
            NewExpectedResult("abc", "xbx", ResultType.Different, new Dictionary<int, int> { { 0, 1 }, { 2, 1 } }),
            NewExpectedResult("abc", "xxc", ResultType.Different, new Dictionary<int, int> { { 0, 2 } })
        };

        private static object[] NewExpectedResult(string left, string right, ResultType result, IDictionary<int, int> differences = null)
            => new object[] {
                new InputData(InputPosition.Left, left),
                new InputData(InputPosition.Right, right),
                new DiffResult(result, differences)
            };
    }
}
