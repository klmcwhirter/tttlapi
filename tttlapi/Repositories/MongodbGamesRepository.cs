using System;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using tttlapi.Models;

namespace tttlapi.Repositories
{
    /// <summary>
    /// Default implementation of the GameRepository
    /// </summary>
    public class MongodbGamesRepository : IGamesRepository
    {
        /// <summary>
        /// Mongo collection for Games
        /// </summary>
        protected string GamesCollectionName = "games";


        /// <summary>
        /// The Mongo client needed to interact with the db
        /// </summary>
        /// <value>IMongoDatabase</value>
        public IMongoDatabase MongoDb { get; }

        /// <summary>
        /// The collection containing Games
        /// </summary>
        /// <returns>IMongoCollection&lt;Game&gt;</returns>
        public IMongoCollection<Game> GamesCollection => MongoDb.GetCollection<Game>(GamesCollectionName);

        /// <summary>
        /// Constuct RedisGamesRepository
        /// </summary>
        /// <param name="mongoDb">IMongoDatabase</param>
        public MongodbGamesRepository(IMongoDatabase mongoDb)
        {
            MongoDb = mongoDb;
        }

        /// <summary>
        /// Get all the games
        /// </summary>
        /// <returns>Game[]</returns>
        public Game[] GetAll()
        {
            var rc = GamesCollection.AsQueryable();
            return rc.ToArray();
        }

        /// <summary>
        /// Get a single game
        /// </summary>
        /// <param name="id">id of the game</param>
        /// <returns>Game</returns>
        public Game Get(int id)
        {
            var rc = GamesCollection.AsQueryable().FirstOrDefault(g => g.Id == id);
            return rc;
        }

        /// <summary>
        /// object with which a lock can be requested to serialize the id selection logic
        /// </summary>
        /// <returns></returns>
        protected static object GamesIdLock = new object();

        /// <summary>
        /// Start a game and return it
        /// </summary>
        /// <param name="players">array of game players</param>
        /// <returns>Game</returns>
        public Game Start(Player[] players)
        {
            lock (GamesIdLock)
            {
                var id = GamesCollection.CountDocuments(new BsonDocument());
                var game = new Game
                {
                    Id = (int)id,
                    Players = players,
                    StartDate = DateTime.Now,
                    Complete = false,
                    Result = GameResult.None
                };

                GamesCollection.InsertOne(game);

                return game;
            }
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

                GamesCollection.ReplaceOne(Builders<Game>.Filter.Eq("Id", id), game);
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

            GamesCollection.ReplaceOne(Builders<Game>.Filter.Eq("Id", id), game);

            return game;
        }
    }
}
