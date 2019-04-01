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
    /// Controller for the Capabilities API
    /// </summary>
    [Route("api/v1/capabilities")]
    [ApiController]
    public class CapabilitiesController : ControllerBase
    {
        /// <summary>
        /// IPlayerStrategy instances for each PlayerKind
        /// </summary>
        /// <value>IList&lt;IPlayerStrategy&gt;</value>
        protected IList<IPlayerStrategy> PlayerStrategies { get; }

        /// <summary>
        /// Construct the CapabilitiesController
        /// </summary>
        /// <param name="playerStrategies">instances of PlayerStrategy</param>
        public CapabilitiesController(IList<IPlayerStrategy> playerStrategies)
        {
            PlayerStrategies = playerStrategies;
        }

        /// <summary>
        /// Get all API capbilities
        /// </summary>
        /// <returns>Capabilities</returns>
        [HttpGet]
        public Capabilities GetAll()
        {
            var rc = new Capabilities
            {
                Strategies = PlayerStrategies.Select(s => s.Name).ToArray()
            };
            return rc;
        }
    }
}
