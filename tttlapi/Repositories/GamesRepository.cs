using System;
using System.Collections.Generic;
using System.Linq;
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
        Game Get(long id);
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
        Game End(long id);
        /// <summary>
        /// Record a player's turn
        /// </summary>
        /// <param name="id">id of the game</param>
        /// <param name="move">Move</param>
        /// <returns>Game</returns>
        Game RecordMove(long id, Move move);
    }

    /// <summary>
    /// Default implementation of the GameRepository
    /// </summary>
    public class GamesRepository : IGamesRepository
    {
        /// <summary>
        /// Temp storage for the games
        /// </summary>
        protected List<Game> Games { get; set; } = new List<Game>();

        /// <summary>
        /// Get all the games
        /// </summary>
        /// <returns>Game[]</returns>
        public Game[] GetAll()
        {
            return Games.ToArray();
        }

        /// <summary>
        /// Get a single game
        /// </summary>
        /// <param name="id">id of the game</param>
        /// <returns>Game</returns>
        public Game Get(long id)
        {
            return Games.FirstOrDefault(g => g.Id == id);
        }

        /// <summary>
        /// Start a game and return it
        /// </summary>
        /// <param name="players">array of game players</param>
        /// <returns>Game</returns>
        public Game Start(Player[] players)
        {
            var game = new Game
            {
                Id = Games.Count + 1,
                Players = players,
                StartDate = DateTime.Now,
                Complete = false,
                Result = GameResult.None
            };

            Games.Add(game);

            return game;
        }

        /// <summary>
        /// End a game
        /// </summary>
        /// <param name="id">id of the game</param>
        /// <returns>Game</returns>
        public Game End(long id)
        {
            var game = Get(id);
            if (game != null
                // Games that are complete are imumtable
                && !game.Complete)
            {
                game.EndDate = DateTime.Now;
                game.Complete = true;
            }

            return game;
        }

        /// <summary>
        /// Record a player's move
        /// </summary>
        /// <param name="id">id of the game</param>
        /// <param name="move">Move</param>
        /// <returns>Game</returns>
        public Game RecordMove(long id, Move move)
        {
            var game = Get(id);
            if (game != null)
            {
                throw new Exception("Game not found.");
            }

            // Games that are complete are imumtable
            if (!game.Complete)
            {
                throw new Exception("Game is complete");
            }

            // Disallow placement of a piece in a cell that is already occupied
            if (!game.IsCellOccupied(move))
            {
                game.Moves.Add(move);
            }
            else
            {
                throw new Exception($"Cell is occupied: x={move.X}, y={move.Y}");
            }

            if (game.IsComplete())
            {
                game.CompleteGame();
            }

            return game;
        }
    }
}
