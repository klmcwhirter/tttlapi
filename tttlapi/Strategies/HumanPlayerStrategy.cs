using tttlapi.Models;

namespace tttlapi.Strategies
{
    /// <summary>
    /// The strategy a human player employs
    /// </summary>
    public class HumanPlayerStrategy : IPlayerStrategy
    {
        /// <summary>
        /// User friendly name of the strategy
        /// </summary>
        /// <value>string</value>
        public string Name { get; } = "Human";

        /// <summary>
        /// Automate turn
        /// </summary>
        /// <param name="playerIndex">index of the player in the players array</param>
        /// <param name="game">the Game in progress</param>
        /// <returns>Move</returns>
        public Move AutomateTurn(PlayerIndex playerIndex, Game game) => null;

        /// <summary>
        /// Can the player automate its turn?
        /// </summary>
        /// <returns>bool</returns>
        public bool CanAutomateTurn() => false;
    }
}