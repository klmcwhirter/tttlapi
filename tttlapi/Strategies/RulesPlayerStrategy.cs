using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using tttlapi.Models;

namespace tttlapi.Strategies
{
    /// <summary>
    /// The strategy a rules based computer player employs
    /// </summary>
    public class RulesPlayerStrategy : IPlayerStrategy
    {
        /// <summary>
        /// User friendly name of the strategy
        /// </summary>
        /// <value>string</value>
        public string Name { get; } = "Rules";

        /// <summary>
        /// Ctor for RulesPlayerStrategy
        /// </summary>
        /// <param name="logger"></param>
        public RulesPlayerStrategy(ILogger<RulesPlayerStrategy> logger)
        {
            Logger = logger;
        }

        /// <summary>
        /// Automate turn
        /// </summary>
        /// <param name="playerIndex">index of the player in the players array</param>
        /// <param name="game">the Game in progress</param>
        /// <returns>Move</returns>
        public Move AutomateTurn(PlayerIndex playerIndex, Game game)
        {
            Move move = null;

            // Don't attempt if game is complete
            if (!game.Complete)
            {
                foreach (var rule in Rules[(int)playerIndex])
                {
                    if (rule.ShouldTryPlacePiece(playerIndex, game))
                    {
                        Logger.LogInformation($"Trying to place piece: {rule.Name}");
                        var triedMove = rule.TryPlacePiece(playerIndex, game);
                        if (triedMove != null)
                        {
                            Logger.LogInformation($"Found move: {rule.Name}");
                            move = triedMove;
                            break;
                        }
                    }
                    else
                    {
                        Logger.LogInformation($"{rule.Name} could not handle move.");
                    }
                }
            }

            return move;
        }

        /// <summary>
        /// Can the player automate its turn?
        /// </summary>
        /// <returns>bool</returns>
        public bool CanAutomateTurn() => true;

        #region Rules

        /// <summary>
        /// The rules defining the strategy
        /// </summary>
        /// <value>List&lt;Rule[]&gt;</value>
        protected static List<Rule[]> Rules = new List<Rule[]>
        {
            // PlayerIndex.X - offensive rules
            new[]
            {
                // If no moves yet, place in a corner
                new Rule { Name = "X First move", ShouldTryPlacePiece = (p,g) => g.Moves.Count <= 0, TryPlacePiece = (p,g) => g.FindEmptySpot(p, BoardLocation.Corner)},

                // try to find winning move
                new Rule { Name = "X Try winning", ShouldTryPlacePiece = (p,g) => true, TryPlacePiece = (p,g) => g.FindWinningMove(p)},

                // try to find blocking move
                new Rule {
                    Name = "X Try blocking",
                    ShouldTryPlacePiece = (p,g) => true,
                    TryPlacePiece = (p,g) => g.FindWinningMove(PlayerIndex.O, PlayerIndex.X)
                },

// TODO - need a way to detect / select adjacent vs opposite corners / sides

                // If O places first on a side - X places in center; then places in adjacent corner after any O move
                new Rule {
                    Name = "X If O side, try center else corner",
                    ShouldTryPlacePiece = (p,g) => g.PlayerLocationPlacedLast(PlayerIndex.O, BoardLocation.Side),
                    TryPlacePiece = (p,g) => g.FindEmptySpot(p, BoardLocation.Center) ?? g.FindEmptySpot(p, BoardLocation.Corner)
                    },

                // if O places first in an adjacent corner - X places in the other adjacent corner; then other corner
                new Rule {
                    Name = "X If O places first in adjacent corner, place other else corner",
                    ShouldTryPlacePiece = (p,g) => g.PlayerLocationPlacedLast(PlayerIndex.O, BoardLocation.Corner),
                    TryPlacePiece = (p,g) => g.FindEmptySpot(p, BoardLocation.Corner)
                    },

                // If O places first in opposite corner - X places in adjacent corner; then remaining corner
                new Rule {
                    Name = "X If O places first in opposite corner, place adjacent else other corner",
                    ShouldTryPlacePiece = (p,g) => g.PlayerLocationPlacedLast(PlayerIndex.O, BoardLocation.Corner),
                    TryPlacePiece = (p,g) => g.FindEmptySpot(p, BoardLocation.Corner)
                    },

                // If O places first in center - X places in opposite corner 
                //   If O then places in a corner - X places in the remaining corner
                //   If O then places in a side - X blocks until tie
                new Rule {
                    Name = "X If O places first in center, then ...",
                    ShouldTryPlacePiece = (p,g) => g.IfOPlacedCenterThen(),
                    TryPlacePiece = (p,g) => g.AfterOPlacesCenter(p)
                    },

                // fallback - random spot
                new Rule { Name = "X Fallback random", ShouldTryPlacePiece = (p,g) => true, TryPlacePiece = (p,g) => g.FindRandomEmptySpot(p)},
            },

            // PlayerIndex.O - defensive rules
            new[]
            {
                // First try to find winning move
                new Rule { Name = "O Try winning", ShouldTryPlacePiece = (p,g) => true, TryPlacePiece = (p,g) => g.FindWinningMove(p)},

                // try to find blocking move
                new Rule {
                    Name = "O Try blocking",
                    ShouldTryPlacePiece = (p,g) => true,
                    TryPlacePiece = (p,g) => g.FindWinningMove(PlayerIndex.X, PlayerIndex.O)
                },

                // O If X places first anywhere but center - O places in the center
                new Rule {
                    Name = "O If X Places anywhere but Center O places in Center",
                    ShouldTryPlacePiece = (p,g) => !g.PlayerLocationPlacedLast(PlayerIndex.X, BoardLocation.Center) && !g.IsSpotOccupied(4),
                    TryPlacePiece = (p,g) => g.FindEmptySpot(p, BoardLocation.Center)
                },

                // O If X places first in the center - O places a corner; O blocks until tie
                new Rule {
                    Name = "O If X Places in Center O places in Corner",
                    ShouldTryPlacePiece = (p,g) => g.PlayerLocationPlacedLast(PlayerIndex.X, BoardLocation.Center) && !g.IsSpotOccupied(4),
                    TryPlacePiece = (p,g) => g.FindEmptySpot(p, BoardLocation.Corner)
                },

                // fallback - random spot
                new Rule { Name = "O Fallback random", ShouldTryPlacePiece = (p,g) => true, TryPlacePiece = (p,g) => g.FindRandomEmptySpot(p)},
            }
        };

        /// <summary>
        /// Logger used by this class
        /// </summary>
        /// <value></value>
        protected ILogger<RulesPlayerStrategy> Logger { get; }
        #endregion
    }

    /// <summary>
    /// Extension methods for the Move type
    /// </summary>
    public static class RulesMoveExtensions
    {
        /// <summary>
        /// Is the current spot in this Move instance at BoardLocation
        /// </summary>
        /// <param name="move"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public static bool Is(this Move move, BoardLocation location)
        {
            var rc = RulesGameExtensions.BoardLocationMap[location].Contains(move.Spot);
            return rc;
        }
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
    /// Rules strategy extensions for the Game class
    /// </summary>
    public static class RulesGameExtensions
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

        #region Game Find helper methods

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
            return game.FindWinningMove(playerIndex, playerIndex);
        }

        /// <summary>
        /// See if player has a winning move
        /// </summary>
        /// <param name="game">Game</param>
        /// <param name="playerIndex">PlayerIndex</param>
        /// <param name="returnPlayerIndex">PlayerIndex</param>
        /// <returns>Move or null</returns>
        public static Move FindWinningMove(this Game game, PlayerIndex playerIndex, PlayerIndex returnPlayerIndex)
        {
            Move rc = null;
            var striped = new StripedVector<int?>(GameExtensions.WaysToWin, game.ToVector());
            foreach (var stripe in striped)
            {
                // If stripe contains only player or empty AND at least 2 pieces have been placed
                if (stripe.All(s => s.Value == (int)playerIndex || !s.Value.HasValue) && stripe.Count(s => s.Value == (int)playerIndex) == 2)
                {
                    // Get the index of the empty, winning spot
                    var spot = stripe.First(s => !s.Value.HasValue).Key;

                    rc = new Move { PlayerIndex = returnPlayerIndex, Spot = spot };
                    break;
                }
            }

            return rc;
        }

        #endregion

        #region Game Rule helper methods

        /// <summary>
        /// After O places in the center, ... Move selection logic
        /// </summary>
        /// <param name="game">Game</param>
        /// <param name="playerIndex">PlayerIndex (always PlayerIndex.X)</param>
        /// <returns>Move</returns>
        public static Move AfterOPlacesCenter(this Game game, PlayerIndex playerIndex)
        {
            Move rc = null;

            if (game.OPlacedCenter())
            {
                // TODO - Should be opposite corner
                rc = game.FindEmptySpot(playerIndex, BoardLocation.Corner);
            }
            else if (game.OPlacedCenterPreviously())
            {
                if (game.PlayerLocationPlacedLast(PlayerIndex.O, BoardLocation.Corner))
                {
                    rc = game.FindEmptySpot(playerIndex, BoardLocation.Corner);
                }
                else if (game.PlayerLocationPlacedLast(PlayerIndex.O, BoardLocation.Side))
                {
                    rc = game.FindRandomEmptySpot(playerIndex);
                }
            }

            return rc;
        }

        /// <summary>
        /// Detect location PlayerIndex placed last was BoardLocation
        /// </summary>
        /// <param name="game">Game</param>
        /// <param name="playerIndex">PlayerIndex</param>
        /// <param name="location">BoardLocation</param>
        /// <returns>bool</returns>
        public static bool PlayerLocationPlacedLast(this Game game, PlayerIndex playerIndex, BoardLocation location)
        {
            var rc = game.Moves.LastOrDefault(m => m.PlayerIndex == playerIndex)?.Is(location) ?? false;
            return rc;
        }

        /// <summary>
        /// Detect location PlayerIndex placed previously was BoardLocation
        /// </summary>
        /// <param name="game">Game</param>
        /// <param name="playerIndex">PlayerIndex</param>
        /// <param name="location">BoardLocation</param>
        /// <returns>bool</returns>
        public static bool PlayerLocationPlacedPreviously(this Game game, PlayerIndex playerIndex, BoardLocation location)
        {
            var rc = game.Moves
                    .Where(m => m.PlayerIndex == playerIndex)
                    .Reverse()
                    .Take(2) // Last 2 moves
                    .Reverse()
                    .FirstOrDefault()? // Just the previous move
                    .Is(location) ?? false;
            return rc;
        }

        /// <summary>
        /// Detect if O placed Center
        /// </summary>
        /// <param name="game">Game</param>
        /// <returns>bool</returns>
        public static bool OPlacedCenter(this Game game) => game.PlayerLocationPlacedLast(PlayerIndex.O, BoardLocation.Center);

        /// <summary>
        /// Detect if O placed in Center previously
        /// </summary>
        /// <param name="game">Game</param>
        /// <returns>bool</returns>
        public static bool OPlacedCenterPreviously(this Game game) => game.PlayerLocationPlacedPreviously(PlayerIndex.O, BoardLocation.Center);

        /// <summary>
        /// Detect if should use the If O places center ... rule
        /// </summary>
        /// <param name="game">Game</param>
        /// <returns>bool</returns>
        public static bool IfOPlacedCenterThen(this Game game) => game.OPlacedCenter() || game.OPlacedCenterPreviously();

        #endregion

        /// <summary>
        /// Helper predicate to search for a spot in one of the BoardLocationMap arrays
        /// </summary>
        /// <param name="location"></param>
        /// <param name="spot"></param>
        /// <returns></returns>
        public static BoardLocation MapContainsSpot(this BoardLocation location, int spot)
        {
            if (BoardLocationMap[location].Any(i => i == spot))
            {
                return location;
            }

            return BoardLocation.Any;
        }

        /// <summary>
        /// Given a board spot, find its board location
        /// </summary>
        /// <param name="spot"></param>
        /// <returns></returns>
        public static BoardLocation ToBoardLocation(this int spot)
        {
            var rc = BoardLocation.Center.MapContainsSpot(spot);
            if (rc == BoardLocation.Any) rc = BoardLocation.Corner.MapContainsSpot(spot);
            if (rc == BoardLocation.Any) rc = BoardLocation.Side.MapContainsSpot(spot);
            return rc;
        }
    }

    /// <summary>
    /// Describes a game offensive or defensive rule
    /// </summary>
    public class Rule
    {
        /// <summary>
        /// Name of the rule
        /// </summary>
        /// <value>string</value>
        public string Name { get; set; }

        /// <summary>
        /// Predicate to detect opponent's position
        /// </summary>
        /// <value>location to be used to tune next move</value>
        public Func<PlayerIndex, Game, bool> ShouldTryPlacePiece { get; set; }
        /// <summary>
        /// Try to place a piece
        /// </summary>
        /// <value>Move or null</value>
        public Func<PlayerIndex, Game, Move> TryPlacePiece { get; set; }
    }
}