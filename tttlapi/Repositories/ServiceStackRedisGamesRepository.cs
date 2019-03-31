#if SERVICESTACK
using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Redis;
using tttlapi.Models;
using tttlapi.Services;

namespace tttlapi.Repositories
{

    /// <summary>
    /// Default implementation of the GameRepository
    /// </summary>
    public class ServiceStackRedisGamesRepository : IGamesRepository
    {
        /// <summary>
        /// Redis prefix for Games
        /// </summary>
        protected string GamesPrefix = "urn:games";

        /// <summary>
        /// The RedisClient needed to interact with the db
        /// </summary>
        /// <value>IRedisClient</value>
        protected IRedisClient RedisClient { get; }

        /// <summary>
        /// Transform JSON to Game
        /// </summary>
        protected ITransformer<string, Game> JsonToGameTransformer { get; }

        /// <summary>
        /// Transform Game to JSON
        /// </summary>
        protected ITransformer<Game, string> JsonFromGameTransformer { get; }

        /// <summary>
        /// Constuct RedisGamesRepository
        /// </summary>
        /// <param name="redisClient">IRedisClient</param>
        /// <param name="jsonToGameTransformer"></param>
        /// <param name="jsonFromGameTransformer"></param>
        public ServiceStackRedisGamesRepository(
            IRedisClient redisClient,
            ITransformer<string, Game> jsonToGameTransformer,
            ITransformer<Game, string> jsonFromGameTransformer
            )
        {
            RedisClient = redisClient;
            JsonToGameTransformer = jsonToGameTransformer;
            JsonFromGameTransformer = jsonFromGameTransformer;
        }

        /// <summary>
        /// Get all the games
        /// </summary>
        /// <returns>Game[]</returns>
        public Game[] GetAll()
        {
            var json = RedisClient.GetAllItemsFromList(GamesPrefix);
            var rc = json.Select(j => JsonToGameTransformer.Transform(j));
            return rc.ToArray();
        }

        /// <summary>
        /// Get a single game
        /// </summary>
        /// <param name="id">id of the game</param>
        /// <returns>Game</returns>
        public Game Get(int id)
        {
            var json = RedisClient.GetItemFromList(GamesPrefix, id);
            var rc = JsonToGameTransformer.Transform(json);
            return rc;
        }

        /// <summary>
        /// Start a game and return it
        /// </summary>
        /// <param name="players">array of game players</param>
        /// <returns>Game</returns>
        public Game Start(Player[] players)
        {
            var id = (int)RedisClient.GetListCount(GamesPrefix);
            var game = new Game
            {
                Id = id,
                Players = players,
                StartDate = DateTime.Now,
                Complete = false,
                Result = GameResult.None
            };

            var json = JsonFromGameTransformer.Transform(game);
            RedisClient.AddItemToList(GamesPrefix, json);

            return game;
        }

        /// <summary>
        /// End a game
        /// </summary>
        /// <param name="id">id of the game</param>
        /// <returns>Game</returns>
        public Game End(int id)
        {
            var game = Get(id);
            if (game != null
                // Games that are complete are imumtable
                && !game.Complete)
            {
                game.EndDate = DateTime.Now;
                game.Complete = true;

                var json = JsonFromGameTransformer.Transform(game);
                RedisClient.SetItemInList(GamesPrefix, id, json);
            }

            return game;
        }

        /// <summary>
        /// Record a player's move
        /// </summary>
        /// <param name="id">id of the game</param>
        /// <param name="move">Move</param>
        /// <returns>Game</returns>
        public Game RecordMove(int id, Move move)
        {
            var game = Get(id);
            if (game == null)
            {
                throw new Exception("Game not found.");
            }

            // Games that are complete are imumtable
            if (game.Complete)
            {
                throw new Exception("Game is complete");
            }

            // Disallow placement of a piece in a cell that is already occupied
            if (!game.IsSpotOccupied(move.Spot))
            {
                game.Moves.Add(move);
            }
            else
            {
                throw new Exception($"Cell is occupied: spot={move.Spot}");
            }

            game = game.TryCompleteGame();

            var json = JsonFromGameTransformer.Transform(game);
            RedisClient.SetItemInList(GamesPrefix, id, json);

            return game;
        }
    }
}
#endif
