using Microsoft.AspNetCore.Mvc;
using Models;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Controllers
{
    [ApiController, Route("api/v1/[controller]")]
    public class GameController : ControllerBase
    {
        #region Constractor
        public IGameService _gameService { get; set; }
        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }
        #endregion

        [HttpGet("[action]")]
        public IActionResult Index()
        {
            var data = _gameService.GeneratePalyer();

            return Ok(data);
        }

        [HttpPost("[action]/{input}")]
        public IActionResult EnemyShot([FromBody] List<Player> players, string input)
        {
            try
            {
                var data = _gameService.EnemyShot(players, input);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
