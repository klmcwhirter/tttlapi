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
                    ShouldTryPlacePiece = (p,g) => !g.IsSpotOccupied(4),
                    TryPlacePiece = (p,g) => g.FindEmptySpot(p, BoardLocation.Center)
                },

                // O If X places first in the center - O places a corner; O blocks until tie
                new Rule {
                    Name = "O If X Places in Center O places in Corner",
                    ShouldTryPlacePiece = (p,g) => g.IsSpotOccupied(4),
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