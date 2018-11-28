using System;
using System.Collections.Generic;
using System.Linq;

namespace tttlapi.Models
{
    /// <summary>
    /// The index of a particular player into the players array
    /// </summary>
    /// <remarks>
    /// This is primarily a informative construct.
    /// </remarks>
    public enum PlayerIndex
    {
        /// <summary>
        /// Player X is index 0 in the players array.
        /// </summary>
        X = 0,
        /// <summary>
        /// Player O is index 1 in the players array.
        /// </summary>
        O = 1
    }

    /// <summary>
    /// A move in the game is represented by a player and the coordinates on which they placed their piece.
    /// </summary>
    public class Move
    {
        /// <summary>
        /// The index in the players array identifying the player placing the piece
        /// </summary>
        /// <value>PlayerIndex</value>
        public PlayerIndex PlayerIndex { get; set; }

        /// <summary>
        /// The x coord on the board where the piece was placed
        /// </summary>
        /// <value>int</value>
        public int X { get; set; }

        /// <summary>
        /// The y coord on the board where the piece was placed
        /// </summary>
        /// <value>int</value>
        public int Y { get; set; }
    }

    /// <summary>
    /// The kind of player in the game
    /// </summary>
    public enum PlayerKind
    {
        /// <summary>
        /// A Human player
        /// </summary>
        Human = 0,

        /// <summary>
        /// A player using the hard coded rules
        /// </summary>
        Rules = 1,

        /// <summary>
        /// A player using a trained model to determine a piece placement
        /// </summary>
        Learning = 2,

        /// <summary>
        /// A player using a random number generator to determine a piece placement
        /// </summary>
        Random = 3
    }

    /// <summary>
    /// A player in the game
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Player number
        /// </summary>
        /// <value>int</value>
        public int Number { get; set; }

        /// <summary>
        /// The name of the player
        /// </summary>
        /// <value>string</value>
        public string Name { get; set; }

        /// <summary>
        /// The kind of player
        /// </summary>
        /// <value>PlayerKind</value>
        public PlayerKind Kind { get; set; }
    }

    /// <summary>
    /// The result of the game
    /// </summary>
    public enum GameResult
    {
        /// <summary>
        /// No result in the game
        /// </summary>
        None = 0,
        /// <summary>
        /// X won the game
        /// </summary>
        XWins = 1,
        /// <summary>
        /// O won the game
        /// </summary>
        OWins = 2,
        /// <summary>
        /// Game resulted in a tie
        /// </summary>
        Tie = 3
    }

    /// <summary>
    /// The game
    /// </summary>
    public class Game
    {
        /// <summary>
        /// Unique identifier for the game
        /// </summary>
        /// <value>int</value>
        public int Id { get; set; }
        /// <summary>
        /// The list of moves at this point in the game
        /// </summary>
        /// <value>List&lt;Move&gt;</value>
        public List<Move> Moves { get; set; } = new List<Move>();
        /// <summary>
        /// The game players array
        /// </summary>
        /// <value>Player[]</value>
        public Player[] Players { get; set; }
        /// <summary>
        /// Timestamp when the game was started
        /// </summary>
        /// <value>DateTime</value>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// Timestamp of when the game ended or null
        /// </summary>
        /// <value>DateTime</value>
        public DateTime? EndDate { get; set; } = null;
        /// <summary>
        /// Flag specifying whether the game is complete or not
        /// </summary>
        /// <value>bool</value>
        public bool Complete { get; set; }
        /// <summary>
        /// The game result
        /// </summary>
        /// <value>GameResult</value>
        public GameResult Result { get; set; }
    }

    /// <summary>
    /// Extension methods for the Game type
    /// </summary>
    public static class GameExtensions
    {
        /// <summary>
        /// The way to win in Tic Tac Toe
        /// </summary>
        /// <value>IEnumerable&lt;int[]&gt;</value>
        public static readonly IEnumerable<int[]> WaysToWin = new[] {
            // rows
            new[] { 0, 1, 2 },
            new[] { 3, 4, 5 },
            new[] { 6, 7, 8 },
            
            // cols
            new[] { 0, 3, 6 },
            new[] { 1, 4, 7 },
            new[] { 2, 5, 8 },
            
            // diagonals
            new[] { 0, 4, 8 },
            new[] { 2, 4, 6 }
        };

        /// <summary>
        /// Are all cells occupied?
        /// </summary>
        public static bool IsBoardFull(this Game game) => game.Moves.Count >= 9;

        /// <summary>
        /// Detects whether a cell is already occupied or not
        /// </summary>
        /// <param name="game">Game</param>
        /// <param name="move">Move to test</param>
        /// <returns>bool</returns>
        public static bool IsCellOccupied(this Game game, Move move) => game.Moves.Any(m => m.X == move.X && m.Y == move.Y);

        /// <summary>
        /// Determine the next player
        /// </summary>
        /// <param name="game">Game</param>
        /// <param name="curr">Current PlayerIndex</param>
        /// <returns>PlayerIndex</returns>
        public static PlayerIndex NextPlayer(this Game game, PlayerIndex curr)
        {
            var rc = curr == PlayerIndex.X ? PlayerIndex.O : PlayerIndex.X;
            return rc;
        }

        /// <summary>
        /// Translate Moves to a vector representing the board with the pieces placed
        /// </summary>
        /// <param name="game">Game</param>
        /// <returns>int?[]</returns>
        public static int?[] ToVector(this Game game)
        {
            var rc = new int?[9];
            game.Moves.ForEach(m => rc[m.Y * 3 + m.X] = (int)m.PlayerIndex);
            return rc;
        }

        /// <summary>
        /// Determine the result if the game were to end right now
        /// </summary>
        /// <param name="game">Game</param>
        /// <returns>GameResult</returns>
        public static GameResult TryGetCurrentGameResult(this Game game)
        {
            var rc = GameResult.None;

            var striped = new StripedVector<int?>(WaysToWin, game.ToVector());
            foreach (var stripe in striped)
            {
                if (stripe.All(s => s == (int)PlayerIndex.X))
                {
                    rc = GameResult.XWins;
                    break;
                }
                else if (stripe.All(s => s == (int)PlayerIndex.O))
                {
                    rc = GameResult.OWins;
                    break;
                }
            }

            // Board is full - tie
            if (rc == GameResult.None && game.IsBoardFull())
            {
                rc = GameResult.Tie;
            }

            return rc;
        }

        /// <summary>
        /// Finalize the game setting the appropriate state
        /// </summary>
        /// <returns>Game</returns>
        public static Game TryCompleteGame(this Game game)
        {
            if (!game.Complete)
            {
                var result = game.TryGetCurrentGameResult();
                if (result > GameResult.None)
                {
                    game.Complete = true;
                    game.EndDate = DateTime.Now;
                    game.Result = result;
                }
            }

            return game;
        }
    }
}