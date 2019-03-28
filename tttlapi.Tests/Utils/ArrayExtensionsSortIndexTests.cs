using Xunit;
using tttlapi.Utils;
using System;

namespace tttlapi.Tests
{
    public partial class ArrayExtensionsTests
    {

        [Theory]
        [MemberData(nameof(ArraysMemberDataSource))]
        public void SortIndexProvidesIndexBasedOnReverseSort(object orig)
        {
            //Given
            var param = orig as ArrayParameter;
            var arr = (object[])param.Array;

            //When
            var index = arr.SortedIndex((e1, e2) => { return -((IComparable)e1).CompareTo((IComparable)e2); });

            //Then
            var notGreater = true;
            for (int i = 0; i < index.Length - 1; i++)
            {
                for (int j = 1; j < index.Length; j++)
                {
                if (((IComparable)arr[index[i]]).CompareTo(((IComparable)arr[index[j]])) < 0)
                {
                    notGreater = false;
                }
                }
            }
            var arrStr = arr.ToMessageString();
            var indexStr = index.ToMessageString();

            Assert.False(notGreater, $"index should have been a sorted index of arr\narr = {arrStr}\nindex = {indexStr}");

        }
    }
}
