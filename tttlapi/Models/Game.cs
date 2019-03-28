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
    /// Desribes potential locations on a Tic Tac Toe board
    /// </summary>
    public enum BoardLocation
    {
        /// <summary>
        /// A corner of the board
        /// </summary>
        Corner,

        /// <summary>
        /// A side of the board
        /// </summary>
        Side,

        /// <summary>
        /// The center of the board
        /// </summary>
        Center,

        /// <summary>
        /// Any location on the board
        /// </summary>
        Any
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
        /// The spot on the board where the piece was placed
        /// </summary>
        /// <value>int</value>
        /// <remarks>
        /// x=> 0   1   2   y
        ///                 |
        ///   +---+---+---+ v
        ///   | 0 | 1 | 2 | 0
        ///   +---+---+---+
        ///   | 3 | 4 | 5 | 1
        ///   +---+---+---+
        ///   | 6 | 7 | 8 | 2
        ///   +---+---+---+
        /// </remarks>
        public int Spot { get; set; }
    }

    /// <summary>
    /// Extension methods for the Move type
    /// </summary>
    public static class MoveExtensions
    {
        /// <summary>
        /// Is the current spot in this Move instance at BoardLocation
        /// </summary>
        /// <param name="move"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public static bool Is(this Move move, BoardLocation location)
        {
            var rc = GameExtensions.BoardLocationMap[location].Contains(move.Spot);
            return rc;
        }
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
        static Random Random { get; } = new Random();

        /// <summary>
        /// Map of BoardLocation values to array of spots
        /// </summary>
        /// <value></value>
        public static IDictionary<BoardLocation, int[]> BoardLocationMap = new Dictionary<BoardLocation, int[]>
        {
            { BoardLocation.Center, new[] { 4 } },
            { BoardLocation.Corner, new[] { 0, 2, 6, 8}},
            { BoardLocation.Side, new[] { 1, 3, 5, 7}},
            { BoardLocation.Any, Enumerable.Range(0, 9).ToArray() },
        };

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
        /// Find an empty spot at a BoardLocation
        /// </summary>
        /// <param name="game">Game</param>
        /// <param name="playerIndex">PlayerIndex</param>
        /// <param name="location">BoardLocation</param>
        /// <returns>Move or null</returns>
        public static Move FindEmptySpot(this Game game, PlayerIndex playerIndex, BoardLocation location)
        {
            var spots = BoardLocationMap[location].Shuffle().Where(s => !game.IsSpotOccupied(s));

            var rc = spots.Count() > 0 ? new Move { PlayerIndex = playerIndex, Spot = spots.First() } : null;

            return rc;
        }

        /// <summary>
        /// Find a random empty spot
        /// </summary>
        /// <param name="game">Game</param>
        /// <param name="playerIndex">PlayerIndex</param>
        /// <returns>Move or null</returns>
        public static Move FindRandomEmptySpot(this Game game, PlayerIndex playerIndex)
        {
            Move rc = null;

            if (!game.IsBoardFull())
            {
                var move = game.FindEmptySpot(playerIndex, BoardLocation.Any);

                rc = move;
            }

            return rc;
        }

        /// <summary>
        /// See if player has a winning move
        /// </summary>
        /// <param name="game">Game</param>
        /// <param name="playerIndex">PlayerIndex</param>
        /// <returns>Move or null</returns>
        public static Move FindWinningMove(this Game game, PlayerIndex playerIndex)
        {
            Move rc = null;
            var striped = new StripedVector<int?>(WaysToWin, game.ToVector());
            foreach (var stripe in striped)
            {
                // If stripe contains only player or empty AND at least 2 pieces have been placed
                if (stripe.All(s => s.Value == (int)playerIndex || !s.Value.HasValue) && stripe.Count(s => s.Value == (int)playerIndex) == 2)
                {
                    // Get the index of the empty, winning spot
                    var spot = stripe.First(s => !s.Value.HasValue).Key;

                    rc = new Move { PlayerIndex = playerIndex, Spot = spot };
                    break;
                }
            }

            return rc;
        }

        /// <summary>
        /// Are all spots occupied?
        /// </summary>
        public static bool IsBoardFull(this Game game) => game.Moves.Count >= 9;

        /// <summary>
        /// Detects whether a spot is already occupied or not
        /// </summary>
        /// <param name="game">Game</param>
        /// <param name="spot">Spot to test</param>
        /// <returns>bool</returns>
        public static bool IsSpotOccupied(this Game game, int spot) => game.Moves.Any(m => m.Spot == spot);

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
            game.Moves.ForEach(m => rc[m.Spot] = (int)m.PlayerIndex);
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
                if (stripe.All(s => s.Value == (int)PlayerIndex.X))
                {
                    rc = GameResult.XWins;
                    break;
                }
                else if (stripe.All(s => s.Value == (int)PlayerIndex.O))
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