using Xunit;
using tttlapi.Models;
using System.Linq;

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

            Enumerable.Range(0, 8).ToList().ForEach(i => game.Moves.Add(new Move()));
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

        #region IsCellOccupied

        [Fact]
        public void IsCellOccupiedReturnsFalseWhenNoMoves()
        {
            var game = new Game();

            var rc = game.IsCellOccupied(new Move { X = 1, Y = 1 });

            Assert.False(rc);
        }

        [Fact]
        public void IsCellOccupiedReturnsTrueWhenItIs()
        {
            var game = new Game();
            game.Moves.Add(new Move { X = 1, Y = 1 });

            var rc = game.IsCellOccupied(new Move { X = 1, Y = 1 });

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
                            X = x,
                            Y = y
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
                new Move { PlayerIndex = PlayerIndex.X, X = 0, Y = 0},
                new Move { PlayerIndex = PlayerIndex.O, X = 0, Y = 2},
                new Move { PlayerIndex = PlayerIndex.X, X = 2, Y = 0},
                new Move { PlayerIndex = PlayerIndex.O, X = 2, Y = 2},
                new Move { PlayerIndex = PlayerIndex.X, X = 1, Y = 1},
                new Move { PlayerIndex = PlayerIndex.O, X = 2, Y = 1},
                new Move { PlayerIndex = PlayerIndex.X, X = 1, Y = 2},
                new Move { PlayerIndex = PlayerIndex.O, X = 1, Y = 0},
                new Move { PlayerIndex = PlayerIndex.X, X = 0, Y = 1}
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
                new Move { PlayerIndex = PlayerIndex.X, X = 0, Y = 0},
                new Move { PlayerIndex = PlayerIndex.X, X = 1, Y = 1},
                new Move { PlayerIndex = PlayerIndex.X, X = 2, Y = 2}
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
                new Move { PlayerIndex = PlayerIndex.X, X = 1, Y = 0},
                new Move { PlayerIndex = PlayerIndex.O, X = 0, Y = 0},
                new Move { PlayerIndex = PlayerIndex.X, X = 2, Y = 0},
                new Move { PlayerIndex = PlayerIndex.O, X = 1, Y = 1},
                new Move { PlayerIndex = PlayerIndex.X, X = 2, Y = 1},
                new Move { PlayerIndex = PlayerIndex.O, X = 2, Y = 2}
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
                new Move { PlayerIndex = PlayerIndex.X, X = 0, Y = 0},
                new Move { PlayerIndex = PlayerIndex.O, X = 2, Y = 0},
                new Move { PlayerIndex = PlayerIndex.X, X = 1, Y = 1},
                new Move { PlayerIndex = PlayerIndex.O, X = 2, Y = 1},
                new Move { PlayerIndex = PlayerIndex.X, X = 1, Y = 2},
                new Move { PlayerIndex = PlayerIndex.O, X = 1, Y = 0},
                new Move { PlayerIndex = PlayerIndex.X, X = 0, Y = 1},
                new Move { PlayerIndex = PlayerIndex.O, X = 0, Y = 2},
                new Move { PlayerIndex = PlayerIndex.X, X = 2, Y = 2}
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
                new Move { PlayerIndex = PlayerIndex.X, X = 0, Y = 0},
                new Move { PlayerIndex = PlayerIndex.X, X = 1, Y = 1},
                new Move { PlayerIndex = PlayerIndex.X, X = 2, Y = 2}
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
                new Move { PlayerIndex = PlayerIndex.X, X = 0, Y = 0},
                new Move { PlayerIndex = PlayerIndex.O, X = 0, Y = 2},
                new Move { PlayerIndex = PlayerIndex.X, X = 2, Y = 0},
                new Move { PlayerIndex = PlayerIndex.O, X = 2, Y = 2},
                new Move { PlayerIndex = PlayerIndex.X, X = 1, Y = 1},
                new Move { PlayerIndex = PlayerIndex.O, X = 2, Y = 1},
                new Move { PlayerIndex = PlayerIndex.X, X = 1, Y = 2},
                new Move { PlayerIndex = PlayerIndex.O, X = 1, Y = 0},
                new Move { PlayerIndex = PlayerIndex.X, X = 0, Y = 1}
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