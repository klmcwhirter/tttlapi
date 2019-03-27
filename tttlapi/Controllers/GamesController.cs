using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using tttlapi.Models;
using tttlapi.Repositories;
using tttlapi.Strategies;

namespace tttlapi.Controllers
{
    /// <summary>
    /// Controller for the games API
    /// </summary>
    [Route("api/v1/games")]
    public class GamesController : Controller
    {
        /// <summary>
        /// The repository to use for games storage interaction
        /// </summary>
        protected IGamesRepository GamesRepository { get; }

        /// <summary>
        /// IPlayerStrategy instances for each PlayerKind
        /// </summary>
        /// <value>IList&lt;IPlayerStrategy&gt;</value>
        protected IList<IPlayerStrategy> PlayerStrategies { get; }

        /// <summary>
        /// Construct the GamesController
        /// </summary>
        /// <param name="gamesRepository">injected repository</param>
        /// <param name="playerStrategies">instances of PlayerStrategy</param>
        public GamesController(IGamesRepository gamesRepository, IList<IPlayerStrategy> playerStrategies)
        {
            GamesRepository = gamesRepository;
            PlayerStrategies = playerStrategies;
        }

        /// <summary>
        /// Get all games
        /// </summary>
        /// <returns>IEnumerable&lt;Game&gt;</returns>
        [HttpGet]
        public IEnumerable<Game> GetAll()
        {
            return GamesRepository.GetAll();
        }

        /// <summary>
        /// Get a game
        /// </summary>
        /// <param name="id">id of the game</param>
        /// <returns>Game</returns>
        [HttpGet("{id}")]
        public Game Get(int id)
        {
            var game = GamesRepository.Get(id);
            return game;
        }

        /// <summary>
        /// Start a game
        /// </summary>
        /// <param name="players">Array of the game players</param>
        /// <returns>Game</returns>
        [HttpPost("start")]
        public Game Start([FromBody]Player[] players)
        {
            var game = GamesRepository.Start(players);

            // Player X goes first
            var playerIndex = PlayerIndex.X;

            if (GetStrategy(playerIndex, game).CanAutomateTurn())
            {
                game = TakeTurns(playerIndex, game);
            }

            return game;
        }

        /// <summary>
        /// End a game
        /// </summary>
        /// <param name="id">id of the game</param>
        /// <returns>Game</returns>
        [HttpPatch("{id}/end")]
        public Game End(int id)
        {
            var game = GamesRepository.End(id);
            return game;
        }

        /// <summary>
        /// A player took a turn; record the move
        /// </summary>
        /// <param name="id">Id of the value to patch</param>
        /// <param name="move">the move</param>
        [HttpPatch("{id}/move")]
        public Game Move(int id, [FromBody]Move move)
        {
            var game = GamesRepository.RecordMove(id, move);

            var playerIndex = game.NextPlayer(move.PlayerIndex);

            game = TakeTurns(playerIndex, game);

            return game;
        }

        /// <summary>
        /// Gets the strategy instance for the PlayerKind for playerIndex
        /// </summary>
        /// <param name="playerIndex">=player in the game</param>
        /// <param name="game">Game</param>
        /// <returns>IPlayerStrategy</returns>
        protected IPlayerStrategy GetStrategy(PlayerIndex playerIndex, Game game)
        {
            var player = game.Players[(int)playerIndex];
            var strategy = PlayerStrategies[(int)player.Kind];
            return strategy;
        }

        /// <summary>
        /// Take turns as long as the current player can automate their turn
        /// </summary>
        /// <param name="playerIndex">current game player</param>
        /// <param name="game">Game</param>
        /// <returns>Game</returns>
        protected Game TakeTurns(PlayerIndex playerIndex, Game game)
        {
            var strategy = GetStrategy(playerIndex, game);
            while (!game.Complete && strategy.CanAutomateTurn())
            {
                var move = strategy.AutomateTurn(playerIndex, game);
                game = GamesRepository.RecordMove(game.Id, move);

                playerIndex = game.NextPlayer(move.PlayerIndex);
                strategy = GetStrategy(playerIndex, game);
            }

            return game;
        }
    }
}
