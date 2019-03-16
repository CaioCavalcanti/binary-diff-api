using System.Collections.Generic;

namespace BinaryDiff.Domain.Helpers
{
    public static class StringHelper
    {
        public static IDictionary<int, int> GetStringDifferences(string a, string b)
        {
            var differences = new Dictionary<int, int>();

            int length = 0;
            int offset = -1;

            for (int i = 0; i <= a.Length; i++)
            {
                if (i < a.Length && a[i] != b[i])
                {
                    length++;

                    if (offset < 0)
                    {
                        offset = i;
                    }
                }
                else if (offset != -1)
                {
                    differences[offset] = length;

                    length = 0;
                    offset = 1;
                }
            }

            return differences;
        }
    }
}
