using GameLibrary.Models;
using GameLibrary.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly IGameService gameService;
        private readonly IGenreService genreService;

        public GamesController(IGameService gameService, IGenreService genreService)
        {
            this.gameService = gameService;
            this.genreService = genreService;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse>> GetGamesAsync()
        {
            var response = await gameService.GetGamesAsync();
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ServiceResponse>> GetGameByIdAsync(int id)
        {
            var response = await gameService.GetGameByIdAsync(id);
            if(!response.Success)
            {
                return NotFound(response);
            }
            else
            {
                return Ok(response);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ServiceResponse>> DeleteGameAsync(int id)
        {
            var response = await gameService.DeleteGameAsync(id);
            if(!response.Success)
            {
                return NotFound(response);
            }
            else
            {
                return Ok(response);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ServiceResponse>> UpdateGameAsync(int id, Games game)
        {
            if(id != game.Id)
            {
                return BadRequest("ID not found");
            }
            var response = await gameService.UpdateGameAsync(id, game);
            if(!response.Success)
                return NotFound(response.Message);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse>> AddGameAsync(Games game)
        {
            if(game is null)
            {
                return BadRequest("Bad Request");
            }
            var result = await gameService.AddGameAsync(game);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpGet("genre")]
        public async Task<ActionResult<List<Genre>>> GetGenresAsync() => Ok(await genreService.GetGenresAsync());

    }
}
