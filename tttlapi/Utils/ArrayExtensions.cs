#pragma warning disable CS1572, CS1573, CS1591
using System;
using System.Linq;
using System.Text;

namespace tttlapi
{
    public static class ArrayExtensions
    {
        public static T[] Shuffle<T>(this T[] array)
        {
            // shuffle several times
            for (int i = 0; i < 7; i++)
            {
                for (int n = array.Length - 1; n >= 0; n--)
                {
                    var k = array.Length.Random();
                    T temp = array[n];
                    array[n] = array[k];
                    array[k] = temp;
                }
            }

            return array;
        }

        public static int[] SortedIndex<T>(this T[] array, Func<T, T, int> comparer)
        {
            var rc = Enumerable.Range(0, array.Length).ToArray();
            for (int i = 0; i < rc.Length; i++)
            {
                for (int j = i + 1; j < rc.Length; j++)
                {
                    if (comparer(array[rc[i]], array[rc[j]]) > 0)
                    {
                        var temp = rc[j];
                        rc[j] = rc[i];
                        rc[i] = temp;
                    }
                }
            }
            return rc;
        }

        public static string ToMessageString<T>(this T[] arr)
        {
            var sb = new StringBuilder();
            sb.Append("[ ");
            for (int i = 0; i < arr.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(", ");
                }
                sb.Append("\"").Append(arr[i]).Append("\"");
            }
            sb.Append(" ]");

            return sb.ToString();
        }

    }
}
