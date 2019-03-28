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
        [Theory]
        [MemberData(nameof(ArraysMemberDataSource))]
        public void ToMessageStringOnArrayShouldGiveCorrectString(object orig)
        {
            //Given
            var param = orig as ArrayParameter;
            var arr = (object[])param.Array;

            //When
            var rc = arr.ToMessageString();

            //Then
            Assert.Equal(param.Expected, rc); // $"correct string should have been output\nexpected = {param.Expected}\nrc = {rc}");
        }
    }
}
