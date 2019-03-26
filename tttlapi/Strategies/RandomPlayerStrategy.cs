using System;
using tttlapi.Models;

namespace tttlapi.Strategies
{
    /// <summary>
    /// The strategy a random computer player employs
    /// </summary>
    public class RandomPlayerStrategy : IPlayerStrategy
    {
        /// <summary>
        /// Seeded instance of Random
        /// </summary>
        /// <value>Random</value>
        protected static Random Random { get; }

        static RandomPlayerStrategy()
        {
            Random = new Random((int)DateTime.Now.Ticks);
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
                do
                {
                    move = new Move
                    {
                        PlayerIndex = playerIndex,
                        Spot = Random.Next(9)
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
}