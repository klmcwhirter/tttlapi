using Xunit;
using tttlapi.Models;
using System.Linq;
using System.Collections.Generic;

namespace tttlapi.Tests
{
    public class GameExtensionTests
    {
        #region IsBoardFull

        [Fact]
        public void IsBoardFullReturnsFalseWhenNot()
        {
            var game = new Game();

            var rc = game.IsBoardFull();

            Assert.False(rc);

            Enumerable.Range(0, 9 - 1).ToList().ForEach(i => game.Moves.Add(new Move()));
            rc = game.IsBoardFull();

            Assert.False(rc);
        }

        [Fact]
        public void IsBoardFullReturnsTrueWhenIs()
        {
            var game = new Game();
            Enumerable.Range(0, 9).ToList().ForEach(i => game.Moves.Add(new Move()));

            var rc = game.IsBoardFull();

            Assert.True(rc);
        }

        #endregion

        #region IsSpotOccupied

        [Fact]
        public void IsSpotOccupiedReturnsFalseWhenNoMoves()
        {
            var game = new Game();

            var rc = game.IsSpotOccupied(4);

            Assert.False(rc);
        }

        [Fact]
        public void IsSpotOccupiedReturnsTrueWhenItIs()
        {
            var game = new Game();
            game.Moves.Add(new Move { Spot = 4 });

            var rc = game.IsSpotOccupied(4);

            Assert.True(rc);
        }

        #endregion

        #region NextPlayer

        [Fact]
        public void NextPlayerReturnsOWhenCurrIsX()
        {
            var game = new Game();

            var rc = game.NextPlayer(PlayerIndex.X);

            Assert.Equal(PlayerIndex.O, rc);
        }

        [Fact]
        public void NextPlayerReturnsXWhenCurrIsO()
        {
            var game = new Game();

            var rc = game.NextPlayer(PlayerIndex.O);

            Assert.Equal(PlayerIndex.X, rc);
        }

        #endregion

        #region ToVector

        [Fact]
        public void ToVectorReturnsNullsIfNoMoves()
        {
            var game = new Game();

            var rc = game.ToVector();

            Assert.NotNull(rc);
            Assert.NotEmpty(rc);
            Assert.Equal(9, rc.Length);
            Assert.All(rc, e => Assert.Null(e));
        }

        [Fact]
        public void ToVectorReturnsAllMoves()
        {
            var curr = PlayerIndex.O;
            PlayerIndex togglePlayerIndex()
            {
                curr = curr == PlayerIndex.X ? PlayerIndex.O : PlayerIndex.X;
                return curr;
            }

            var game = new Game();
            foreach (var y in Enumerable.Range(0, 3))
            {
                foreach (var x in Enumerable.Range(0, 3))
                {
                    game.Moves.Add(
                        new Move
                        {
                            PlayerIndex = togglePlayerIndex(),
                            Spot = y * 3 + x
                        });
                }
            }

            var rc = game.ToVector();

            Assert.NotNull(rc);
            Assert.NotEmpty(rc);
            Assert.Equal(9, rc.Length);

            curr = PlayerIndex.O;
            Assert.All(rc, e => Assert.Equal((int)togglePlayerIndex(), e));
        }

        #endregion

        #region TryGetCurrentGameResult

        [Fact]
        public void TryGetCurrentGameResultReturnNoneIfNoMoves()
        {
            var game = new Game();

            var rc = game.TryGetCurrentGameResult();

            Assert.Equal(GameResult.None, rc);
        }

        [Fact]
        public void TryGetCurrentGameResultReturnTieIfNoWinnerAndBoardFull()
        {
            var game = new Game();
            game.Moves.AddRange(new Move[]
            {
                new Move { PlayerIndex = PlayerIndex.X, Spot = 0},
                new Move { PlayerIndex = PlayerIndex.O, Spot = 6},
                new Move { PlayerIndex = PlayerIndex.X, Spot = 2},
                new Move { PlayerIndex = PlayerIndex.O, Spot = 8},
                new Move { PlayerIndex = PlayerIndex.X, Spot = 4},
                new Move { PlayerIndex = PlayerIndex.O, Spot = 5},
                new Move { PlayerIndex = PlayerIndex.X, Spot = 7},
                new Move { PlayerIndex = PlayerIndex.O, Spot = 1},
                new Move { PlayerIndex = PlayerIndex.X, Spot = 3}
            });

            var rc = game.TryGetCurrentGameResult();

            Assert.Equal(GameResult.Tie, rc);
        }

        [Fact]
        public void TryGetCurrentGameResultReturnXWinsIfSo()
        {
            var game = new Game();
            game.Moves.AddRange(new Move[]
            {
                new Move { PlayerIndex = PlayerIndex.X, Spot = 0},
                new Move { PlayerIndex = PlayerIndex.X, Spot = 4},
                new Move { PlayerIndex = PlayerIndex.X, Spot = 8}
            });

            var rc = game.TryGetCurrentGameResult();

            Assert.Equal(GameResult.XWins, rc);
        }

        [Fact]
        public void TryGetCurrentGameResultReturnOWinsIfSo()
        {
            var game = new Game();
            game.Moves.AddRange(new Move[]
            {
                new Move { PlayerIndex = PlayerIndex.X, Spot = 1},
                new Move { PlayerIndex = PlayerIndex.O, Spot = 0},
                new Move { PlayerIndex = PlayerIndex.X, Spot = 2},
                new Move { PlayerIndex = PlayerIndex.O, Spot = 4},
                new Move { PlayerIndex = PlayerIndex.X, Spot = 5},
                new Move { PlayerIndex = PlayerIndex.O, Spot = 8}
            });

            var rc = game.TryGetCurrentGameResult();

            Assert.Equal(GameResult.OWins, rc);
        }

        [Fact]
        public void TryGetCurrentGameResultReturnWinsOnLastMove()
        {
            var game = new Game();
            game.Moves.AddRange(new Move[]
            {
                new Move { PlayerIndex = PlayerIndex.X, Spot = 0},
                new Move { PlayerIndex = PlayerIndex.O, Spot = 2},
                new Move { PlayerIndex = PlayerIndex.X, Spot = 4},
                new Move { PlayerIndex = PlayerIndex.O, Spot = 5},
                new Move { PlayerIndex = PlayerIndex.X, Spot = 7},
                new Move { PlayerIndex = PlayerIndex.O, Spot = 1},
                new Move { PlayerIndex = PlayerIndex.X, Spot = 3},
                new Move { PlayerIndex = PlayerIndex.O, Spot = 6},
                new Move { PlayerIndex = PlayerIndex.X, Spot = 8}
            });

            var rc = game.TryGetCurrentGameResult();

            Assert.Equal(GameResult.XWins, rc);
        }

        #endregion

        #region TryCompleteGame

        [Fact]
        public void TryCompleteGameDoesNothingIfGameNotComplete()
        {
            var game = new Game();

            Assert.False(game.Complete);
            Assert.Null(game.EndDate);
            Assert.Equal(GameResult.None, game.Result);

            var rc = game.TryCompleteGame();

            Assert.NotNull(rc);
            Assert.False(rc.Complete);
            Assert.Null(rc.EndDate);
            Assert.Equal(GameResult.None, rc.Result);
        }

        [Fact]
        public void TryCompleteGameMarksCompleteIfWinner()
        {
            var game = new Game();
            game.Moves.AddRange(new Move[]
            {
                new Move { PlayerIndex = PlayerIndex.X, Spot = 0},
                new Move { PlayerIndex = PlayerIndex.X, Spot = 4},
                new Move { PlayerIndex = PlayerIndex.X, Spot = 8}
            });

            Assert.False(game.Complete);
            Assert.Null(game.EndDate);
            Assert.Equal(GameResult.None, game.Result);

            var rc = game.TryCompleteGame();

            Assert.NotNull(rc);
            Assert.True(rc.Complete);
            Assert.NotNull(rc.EndDate);
            Assert.Equal(GameResult.XWins, rc.Result);
        }

        [Fact]
        public void TryCompleteGameMarksCompleteIfTie()
        {
            var game = new Game();
            game.Moves.AddRange(new Move[]
            {
                new Move { PlayerIndex = PlayerIndex.X, Spot = 0},
                new Move { PlayerIndex = PlayerIndex.O, Spot = 6},
                new Move { PlayerIndex = PlayerIndex.X, Spot = 2},
                new Move { PlayerIndex = PlayerIndex.O, Spot = 8},
                new Move { PlayerIndex = PlayerIndex.X, Spot = 4},
                new Move { PlayerIndex = PlayerIndex.O, Spot = 5},
                new Move { PlayerIndex = PlayerIndex.X, Spot = 7},
                new Move { PlayerIndex = PlayerIndex.O, Spot = 1},
                new Move { PlayerIndex = PlayerIndex.X, Spot = 3}
            });

            Assert.False(game.Complete);
            Assert.Null(game.EndDate);
            Assert.Equal(GameResult.None, game.Result);

            var rc = game.TryCompleteGame();

            Assert.NotNull(rc);
            Assert.True(rc.Complete);
            Assert.NotNull(rc.EndDate);
            Assert.Equal(GameResult.Tie, rc.Result);
        }

        #endregion
    }
}