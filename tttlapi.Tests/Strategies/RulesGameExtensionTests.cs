using Xunit;
using tttlapi.Models;
using tttlapi.Strategies;
using System.Linq;
using System.Collections.Generic;

namespace tttlapi.Tests
{
    public class RulesGameExtensionTests
    {
        #region FindEmptySpot

        public static IEnumerable<object[]> OpenSpotData
        {
            get
            {
                return new[] {
                    new object[] { BoardLocation.Center },
                    new object[] { BoardLocation.Corner },
                    new object[] { BoardLocation.Side }
                };
            }
        }

        [Theory, MemberData(nameof(OpenSpotData))]
        public void FindEmptySpot_FindsSpotIfOpen(BoardLocation location)
        {
            var game = new Game
            {
                Moves = new List<Move>
                {
                    new Move { Spot = 0 },
                    new Move { Spot = 1 },
                    new Move { Spot = 7 },
                    new Move { Spot = 2 },
                }
            };

            var rc = game.FindEmptySpot(PlayerIndex.X, location);

            Assert.NotNull(rc);
            Assert.Equal(PlayerIndex.X, rc.PlayerIndex);
            Assert.True(rc.Spot != 0);
        }

        #endregion

        #region FindRandomEmptySpot

        public static IEnumerable<object[]> FindOneData
        {
            get
            {
                return new[] {
                    new object[] { PlayerIndex.X},
                    new object[] { PlayerIndex.O}
                };
            }
        }

        [Theory, MemberData(nameof(FindOneData))]
        public void FindRandomEmptySpot_CanFindOne(PlayerIndex playerIndex)
        {
            var game = new Game { Moves = new List<Move> { new Move { Spot = 0 } } };

            var rc = game.FindRandomEmptySpot(playerIndex);

            Assert.NotNull(rc);
            Assert.Equal(playerIndex, rc.PlayerIndex);
            Assert.True(rc.Spot != 0);
        }

        [Theory, MemberData(nameof(FindOneData))]
        public void FindRandomEmptySpot_CannotFindOneIfFull(PlayerIndex playerIndex)
        {
            var game = new Game();
            game.Moves = Enumerable.Range(0, 9).Select(spot => new Move { Spot = spot }).ToList();

            var rc = game.FindRandomEmptySpot(playerIndex);

            Assert.Null(rc);
        }

        #endregion

        #region FindWinningMove

        public static IEnumerable<object[]> FindOneForPlayer
        {
            get
            {
                return new[] {
                    new object[] {
                        new List<Move>
                        {
                            new Move { PlayerIndex = PlayerIndex.X, Spot = 0 },
                            new Move { PlayerIndex = PlayerIndex.O, Spot = 2 },
                            new Move { PlayerIndex = PlayerIndex.X, Spot = 8 },
                            new Move { PlayerIndex = PlayerIndex.O, Spot = 5 }
                        }, PlayerIndex.X, 4},

                    new object[] {
                        new List<Move>
                        {
                            new Move { PlayerIndex = PlayerIndex.O, Spot = 0 },
                            new Move { PlayerIndex = PlayerIndex.X, Spot = 1 },
                            new Move { PlayerIndex = PlayerIndex.O, Spot = 6 },
                            new Move { PlayerIndex = PlayerIndex.X, Spot = 7 }
                        }, PlayerIndex.O, 3}
                };
            }
        }

        [Theory, MemberData(nameof(FindOneForPlayer))]
        public void FindWinningMove_CanFindOneForPlayer(List<Move> moves, PlayerIndex playerIndex, int winningSpot)
        {
            var game = new Game { Moves = moves };

            var rc = game.FindWinningMove(playerIndex);

            Assert.NotNull(rc);
            Assert.Equal(winningSpot, rc.Spot);
        }

        [Fact]
        public void FindWinningMove_CannotFindOneForPlayerIfBoardFull()
        {
            var game = new Game { Moves = Enumerable.Range(0, 9).Select(r => new Move { PlayerIndex = PlayerIndex.X, Spot = r }).ToList() };

            var rc = game.FindWinningMove(PlayerIndex.X);

            Assert.Null(rc);
        }

        #endregion

        #region O Places Center methods

        public static IEnumerable<object[]> OPlacesCenterData
        {
            get
            {
                return new[] {
                    new object[] {
                        // O does not place center
                        new List<Move>
                        {
                            new Move { PlayerIndex = PlayerIndex.X, Spot = 0 },
                            new Move { PlayerIndex = PlayerIndex.O, Spot = 2 },
                            new Move { PlayerIndex = PlayerIndex.X, Spot = 8 },
                            new Move { PlayerIndex = PlayerIndex.O, Spot = 6 }
                        }, false, null
                    },

                    new object[] {
                        // O places center
                        new List<Move>
                        {
                            new Move { PlayerIndex = PlayerIndex.X, Spot = 0 },
                            new Move { PlayerIndex = PlayerIndex.O, Spot = 1 },
                            new Move { PlayerIndex = PlayerIndex.X, Spot = 7 },
                            new Move { PlayerIndex = PlayerIndex.O, Spot = 4 }
                        }, true, BoardLocation.Corner
                    },

                    new object[] {
                        // O placed center last time, now in corner
                        new List<Move>
                        {
                            new Move { PlayerIndex = PlayerIndex.X, Spot = 0 },
                            new Move { PlayerIndex = PlayerIndex.O, Spot = 4 },
                            new Move { PlayerIndex = PlayerIndex.X, Spot = 7 },
                            new Move { PlayerIndex = PlayerIndex.O, Spot = 2 }
                        }, true, BoardLocation.Corner
                    },

                    new object[] {
                        // O placed center last time, now in Any
                        new List<Move>
                        {
                            new Move { PlayerIndex = PlayerIndex.X, Spot = 0 },
                            new Move { PlayerIndex = PlayerIndex.O, Spot = 4 },
                            new Move { PlayerIndex = PlayerIndex.X, Spot = 7 },
                            new Move { PlayerIndex = PlayerIndex.O, Spot = 3 }
                        }, true, BoardLocation.Any
                    }
                };
            }
        }

        #region AfterOPlacesCenter

        [Theory, MemberData(nameof(OPlacesCenterData))]
        public void AfterOPlacesCenter_ReturnsCorrectMoveOrNull(List<Move> moves, bool shouldHandle, BoardLocation? expectedLocation)
        {
            var game = new Game { Moves = moves };

            var rc = game.AfterOPlacesCenter(PlayerIndex.X);

            if (expectedLocation.HasValue)
            {
                Assert.NotNull(rc);
                if (expectedLocation != BoardLocation.Any)
                {
                    Assert.Equal(expectedLocation, rc.Spot.ToBoardLocation());
                }
                else
                {
                    // Random spot was returned
                    Assert.True((int)rc.Spot >= 0, "GE 0");
                    Assert.True((int)rc.Spot < 9, "LT 9");
                }
            }
            else
            {
                Assert.Null(rc);
            }
        }

        #endregion

        #region IfOPlacesCenterThen

        [Theory, MemberData(nameof(OPlacesCenterData))]
        public void IfOPlacesCenterThen_ReturnsTrueOrFalse(List<Move> moves, bool shouldHandle, BoardLocation? expectedLocation)
        {
            var game = new Game { Moves = moves };

            var rc = game.IfOPlacedCenterThen();

            Assert.Equal(shouldHandle, rc);
        }

        #endregion

        #region PlayerLocationPlacedLast

        public static IEnumerable<object[]> PlayerPlacesLastData
        {
            get
            {
                return new[] {
                    new object[] {
                        // O does not place center
                        new List<Move>
                        {
                            new Move { PlayerIndex = PlayerIndex.X, Spot = 0 },
                            new Move { PlayerIndex = PlayerIndex.O, Spot = 2 },
                            new Move { PlayerIndex = PlayerIndex.X, Spot = 8 },
                            new Move { PlayerIndex = PlayerIndex.O, Spot = 6 }
                        }, BoardLocation.Corner, BoardLocation.Corner
                    },

                    new object[] {
                        // O places center
                        new List<Move>
                        {
                            new Move { PlayerIndex = PlayerIndex.X, Spot = 0 },
                            new Move { PlayerIndex = PlayerIndex.O, Spot = 1 },
                            new Move { PlayerIndex = PlayerIndex.X, Spot = 7 },
                            new Move { PlayerIndex = PlayerIndex.O, Spot = 4 }
                        }, BoardLocation.Side, BoardLocation.Center
                    },

                    new object[] {
                        // O placed center last time, now in corner
                        new List<Move>
                        {
                            new Move { PlayerIndex = PlayerIndex.X, Spot = 0 },
                            new Move { PlayerIndex = PlayerIndex.O, Spot = 4 },
                            new Move { PlayerIndex = PlayerIndex.X, Spot = 7 },
                            new Move { PlayerIndex = PlayerIndex.O, Spot = 2 }
                        }, BoardLocation.Side, BoardLocation.Corner
                    },

                    new object[] {
                        // O placed center last time, now in Any
                        new List<Move>
                        {
                            new Move { PlayerIndex = PlayerIndex.X, Spot = 0 },
                            new Move { PlayerIndex = PlayerIndex.O, Spot = 4 },
                            new Move { PlayerIndex = PlayerIndex.X, Spot = 7 },
                            new Move { PlayerIndex = PlayerIndex.O, Spot = 3 }
                        }, BoardLocation.Side, BoardLocation.Side
                    }
                };
            }
        }

        [Theory, MemberData(nameof(PlayerPlacesLastData))]
        public void PlayerLocationPlacedLast_EvaluatesCorrectLastMove(List<Move> moves, BoardLocation xLocation, BoardLocation oLocation)
        {
            var game = new Game { Moves = moves };

            Assert.True(game.PlayerLocationPlacedLast(PlayerIndex.X, xLocation));
            Assert.True(game.PlayerLocationPlacedLast(PlayerIndex.O, oLocation));
        }

        #endregion

        #region PlayerLocationPlacedPreviously

        public static IEnumerable<object[]> PlayerLocationPlacedPreviouslyData
        {
            get
            {
                return new[] {
                    new object[] {
                        // O does not place center
                        new List<Move>
                        {
                            new Move { PlayerIndex = PlayerIndex.X, Spot = 0 },
                            new Move { PlayerIndex = PlayerIndex.O, Spot = 2 },
                            new Move { PlayerIndex = PlayerIndex.X, Spot = 8 },
                            new Move { PlayerIndex = PlayerIndex.O, Spot = 6 }
                        }, BoardLocation.Corner, BoardLocation.Corner
                    },

                    new object[] {
                        // O places center
                        new List<Move>
                        {
                            new Move { PlayerIndex = PlayerIndex.X, Spot = 0 },
                            new Move { PlayerIndex = PlayerIndex.O, Spot = 1 },
                            new Move { PlayerIndex = PlayerIndex.X, Spot = 7 },
                            new Move { PlayerIndex = PlayerIndex.O, Spot = 4 }
                        }, BoardLocation.Corner, BoardLocation.Side
                    },

                    new object[] {
                        // O placed center last time, now in corner
                        new List<Move>
                        {
                            new Move { PlayerIndex = PlayerIndex.X, Spot = 7 },
                            new Move { PlayerIndex = PlayerIndex.O, Spot = 4 },
                            new Move { PlayerIndex = PlayerIndex.X, Spot = 0 },
                            new Move { PlayerIndex = PlayerIndex.O, Spot = 2 }
                        }, BoardLocation.Side, BoardLocation.Center
                    },

                    new object[] {
                        // O placed center last time, now in Any
                        new List<Move>
                        {
                            new Move { PlayerIndex = PlayerIndex.X, Spot = 4 },
                            new Move { PlayerIndex = PlayerIndex.O, Spot = 0 },
                            new Move { PlayerIndex = PlayerIndex.X, Spot = 7 },
                            new Move { PlayerIndex = PlayerIndex.O, Spot = 3 }
                        }, BoardLocation.Center, BoardLocation.Corner
                    }
                };
            }
        }

        [Theory, MemberData(nameof(PlayerLocationPlacedPreviouslyData))]
        public void PlayerLocationPlacedPreviously_EvaluatesCorrectLastMove(List<Move> moves, BoardLocation xLocation, BoardLocation oLocation)
        {
            var game = new Game { Moves = moves };

            Assert.True(game.PlayerLocationPlacedPreviously(PlayerIndex.X, xLocation), "Player X");
            Assert.True(game.PlayerLocationPlacedPreviously(PlayerIndex.O, oLocation), "Player O");
        }

        #endregion

        #endregion
    }
}