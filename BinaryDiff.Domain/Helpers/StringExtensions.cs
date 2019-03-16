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
        /// <exception cref="InvalidOperationException">Throws <b>InvalidOperationException</b> if string don't have the same size</exception>
        /// <returns>Dictionary where key is offset and value is length</returns>
        public static IDictionary<int, int> CompareToSameSizeString(this string a, string b)
        {
            if (a.Length != b.Length)
            {
                throw new InvalidOperationException("Strings provided dont have the same size");
            }

            var differences = new Dictionary<int, int>();

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
                    differences[offset.Value] = length;

                    offset = null;
                    length = 0;
                }
            }

            return differences;
        }
    }
}
