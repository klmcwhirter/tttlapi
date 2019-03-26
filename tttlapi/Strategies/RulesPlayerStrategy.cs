using System;
using tttlapi.Models;

namespace tttlapi.Strategies
{
    /// <summary>
    /// The strategy a rules based computer player employs
    /// </summary>
    public class RulesPlayerStrategy : IPlayerStrategy
    {
        /// <summary>
        /// Automate turn
        /// </summary>
        /// <param name="playerIndex">index of the player in the players array</param>
        /// <param name="game">the Game in progress</param>
        /// <returns>Move</returns>
        public Move AutomateTurn(PlayerIndex playerIndex, Game game)
        {
            // TODO: implement actual rules
            Move move = null;

            // Don't attempt if game is complete
            if (!game.Complete)
            {
                do
                {
                    move = new Move
                    {
                        PlayerIndex = playerIndex,
                        Spot = 0 // Random.Next(9)
                    };
                } while (game.IsSpotOccupied(move));
            }

            return move;
        }

        /// <summary>
        /// Can the player automate its turn?
        /// </summary>
        /// <returns>bool</returns>
        public bool CanAutomateTurn() => true;
    }

    /// <summary>
    /// Desribes potential locations on a Tic Tac Toe board
    /// </summary>
    public enum Location
    {
        Corner,
        Side,
        Center
    }

    /// <summary>
    /// Describes a game offensive or defensive rule
    /// </summary>
    public class Rule {
        /// <summary>
        /// Predicate to detect opponent's position
        /// </summary>
        /// <value>location to be used to tune next move</value>
        public Func<Game, Location> OpponentPredicate { get; set; }
        public Func<Game, Move> PiecePlacement { get; set; }
    }
}