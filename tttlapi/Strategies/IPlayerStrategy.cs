using tttlapi.Models;

namespace tttlapi.Strategies
{
    /// <summary>
    /// The strategy a player employs
    /// </summary>
    public interface IPlayerStrategy
    {
        /// <summary>
        /// User friendly name of the strategy
        /// </summary>
        /// <value>string</value>
        string Name { get; }

        /// <summary>
        /// Can the player automate its turn?
        /// </summary>
        /// <returns>bool</returns>
        bool CanAutomateTurn();

        /// <summary>
        /// Automate turn
        /// </summary>
        /// <param name="playerIndex">index of the player in the players array</param>
        /// <param name="game">the Game in progress</param>
        /// <returns>Move</returns>
        Move AutomateTurn(PlayerIndex playerIndex, Game game);
    }
}