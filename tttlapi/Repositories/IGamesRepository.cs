using tttlapi.Models;

namespace tttlapi.Repositories
{
    /// <summary>
    /// Contract for a GamesRepository
    /// </summary>
    public interface IGamesRepository
    {
        /// <summary>
        /// Get all the games
        /// </summary>
        /// <returns>Game[]</returns>
        Game[] GetAll();
        /// <summary>
        /// Get a single game
        /// </summary>
        /// <param name="id">id of the game</param>
        /// <returns>Game</returns>
        Game Get(int id);
        /// <summary>
        /// Start a game and return it
        /// </summary>
        /// <param name="players">array of game players</param>
        /// <returns>Game</returns>
        Game Start(Player[] players);
        /// <summary>
        /// End a game
        /// </summary>
        /// <param name="id">id of the game</param>
        /// <returns>Game</returns>
        Game End(int id);
        /// <summary>
        /// Record a player's turn
        /// </summary>
        /// <param name="id">id of the game</param>
        /// <param name="move">Move</param>
        /// <returns>Game</returns>
        Game RecordMove(int id, Move move);
    }
}
