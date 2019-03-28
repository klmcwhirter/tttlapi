using System.Linq;
using Xunit;
using tttlapi.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace tttlapi.Tests
{
    public partial class ArrayExtensionsTests
    {
        public class ArrayParameter
        {
            public object[] Array { get; set; }
            public string Expected { get; set; }
        }

        static readonly ArrayParameter IntArray = new ArrayParameter
        {
            Array = new object[] { 1, 3, 4, 9 },
            Expected = "[ \"1\", \"3\", \"4\", \"9\" ]"
        };

        static readonly ArrayParameter StringArray = new ArrayParameter
        {
            Array = new object[] { "hello", "world", "this", "array" },
            Expected = "[ \"hello\", \"world\", \"this\", \"array\" ]"
        };

        public static IEnumerable<object[]> ArraysMemberDataSource()
        {
            yield return new object[] { IntArray };
            yield return new object[] { StringArray };
        }

        [Theory]
        [MemberData(nameof(ArraysMemberDataSource))]
        public void ShuffleIntArrayShouldShuffle(object orig)
        {
            var param = orig as ArrayParameter;
            var origArr = param.Array;
            //Given
            var arr = (object[])origArr.Clone();

            var retryCt = 0;
            var diff = false;
            do
            {
                //When
                arr.Shuffle();

                //Then
                for (int i = 0; i < origArr.Length; i++)
                {
                    if (((IComparable)origArr[i]).CompareTo(((IComparable)arr[i])) != 0)
                    {
                        diff = true;
                    }
                }
            // Retry a couple of times in case the random # generator just happened to shuffle them exactly like orig
            } while (!diff && (retryCt++ < 2));

            var origStr = origArr.ToMessageString();
            var arrStr = arr.ToMessageString();

            Assert.True(diff, $"arr should have been shuffled\norigArr = {origStr}\narr = {arrStr}");
        }
    }
}
