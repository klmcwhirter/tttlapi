using Xunit;
using tttlapi.Models;
using System.Linq;
using System.Collections.Generic;
using Xunit.Extensions;
using tttlapi.Strategies;

namespace tttlapi.Tests
{
    public class RulesMoveExtensionsTests
    {
        #region Is_DetectsMoveLocations

        public static IEnumerable<object[]> DetectsData
        {
            get
            {
                return new[]
                {
                    new object[] { 4, BoardLocation.Center},
                    new object[] { 0, BoardLocation.Corner},
                    new object[] { 2, BoardLocation.Corner},
                    new object[] { 6, BoardLocation.Corner},
                    new object[] { 8, BoardLocation.Corner},
                    new object[] { 1, BoardLocation.Side},
                    new object[] { 3, BoardLocation.Side},
                    new object[] { 5, BoardLocation.Side},
                    new object[] { 7, BoardLocation.Side}
                };
            }
        }

        [Theory, MemberData(nameof(DetectsData))]
        public void Is_DetectsMoveLocations(int spot, BoardLocation location)
        {
            var move = new Move { Spot = spot };

            var rc = move.Is(location);

            Assert.True(rc);
        }

        #endregion

        #region Is_FailsToDetectBadMoveLocations

        public static IEnumerable<object[]> DetectsBadData
        {
            get
            {
                return new[]
                {
                    new object[] { 0, BoardLocation.Center},
                    new object[] { 4, BoardLocation.Corner},
                    new object[] { 4, BoardLocation.Side},
                    new object[] { -1, BoardLocation.Side},
                    new object[] { 99, BoardLocation.Side},
                };
            }
        }

        [Theory, MemberData(nameof(DetectsBadData))]
        public void Is_FailsToDetectBadMoveLocations(int spot, BoardLocation location)
        {
            var move = new Move { Spot = spot };

            var rc = move.Is(location);

            Assert.False(rc);
        }

        #endregion
    }
}
