using System;
using tttlapi.Models;

namespace tttlapi.Strategies
{
    /// <summary>
    /// The strategy a learning based computer player employs
    /// </summary>
    public class LearningPlayerStrategy : IPlayerStrategy
    {
        /// <summary>
        /// Seeded instance of Random
        /// </summary>
        /// <value>Random</value>
        protected static Random Random { get; }

        static LearningPlayerStrategy()
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
            // TODO: implement usage of trained model
            Move move = null;

            // Don't attempt if game is complete
            if (!game.IsComplete())
            {
                do
                {
                    move = new Move
                    {
                        PlayerIndex = playerIndex,
                        X = Random.Next(3),
                        Y = Random.Next(3)
                    };
                } while (game.IsCellOccupied(move));
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