using System;
using System.Collections.Generic;

namespace BinaryDiff.Domain.Helpers
{
    /// <summary>
    /// String extensions
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Compare two strings of the same size and return the differences as dictionary
        /// </summary>
        /// <param name="a">String to compare</param>
        /// <param name="b">String to compare against (right)</param>
        /// <param name="differences">out list of differences as dictinoary where key is offset and value is length</param>
        /// <exception cref="InvalidOperationException">Throws <b>InvalidOperationException</b> if string don't have the same size</exception>
        /// <returns>Dictionary where key is offset and value is length</returns>
        public static bool EqualsToSameSizeString(this string a, string b, out IDictionary<int, int> differences)
        {
            differences = null;

            if (Equals(a, b))
            {
                return true;
            }

            if (a.IsLargerThan(b) || b.IsLargerThan(a))
            {
                throw new InvalidOperationException("Strings provided dont have the same size");
            }

            var length = 0;
            int? offset = null;

            for (var i = 0; i <= a.Length; i++)
            {
                if (i < a.Length && a[i] != b[i])
                {
                    length++;

                    if (!offset.HasValue)
                    {
                        offset = i;
                    }
                }
                else if (offset.HasValue)
                {
                    if (differences == null)
                    {
                        differences = new Dictionary<int, int>();
                    }

                    differences[offset.Value] = length;

                    offset = null;
                    length = 0;
                }
            }

            return differences.Count == 0;
        }

        /// <summary>
        /// Safely compares two string by their length, considering null and empty as valid options (length 0)
        /// </summary>
        /// <param name="a">String instace</param>
        /// <param name="b">String instace to compare with</param>
        /// <returns>bool whether a is larger than b or not</returns>
        public static bool IsLargerThan(this string a, string b)
        {
            var aIsNullOrEmpty = string.IsNullOrEmpty(a);
            var bIsNullOrEmpty = string.IsNullOrEmpty(b);

            return (
                (!aIsNullOrEmpty && bIsNullOrEmpty) ||
                (!aIsNullOrEmpty && !bIsNullOrEmpty && a.Length > b.Length)
            );
        }
    }
}
