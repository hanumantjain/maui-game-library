using GameLibrary.Models;
using GameLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

namespace WebApplication1.Services;

public class GameService : IGameService
{
    private readonly AppDbContext appDbContext;
    public GameService(AppDbContext appDbContext)
    {
        this.appDbContext = appDbContext;
    }

    public async Task<ServiceResponse> AddGameAsync(Games game)
    {
        if(game == null)
        {
            return new ServiceResponse()
            {
                Message = "Bad Request",
                Success = false
            };
        }
        var chk = await appDbContext.Games.Where(p => p.Name.ToLower().Equals(game.Name.ToLower())).FirstOrDefaultAsync();
        if(chk is null)
        {   
            appDbContext.Games.Add(game);
            await appDbContext.SaveChangesAsync();
            return new ServiceResponse()
            {
                Message = "Product added",
                Success = true
            };
        }
            
        return new ServiceResponse()
        {
            Message = "Product already added",
            Success = false
        };
        
    }
    public async Task<ServiceResponse> UpdateGameAsync(int id, Games game)
    {
        var result = await appDbContext.Games.FindAsync(id);
        if (result is null)
        {
            return new ServiceResponse() {Message = "Game not found", Success = false};
        };
        result.Name = game.Name;
        result.Description = game.Description;
        result.GenreId = game.GenreId;
        result.Price = game.Price;
        result.ReleasedDate = game.ReleasedDate;
        result.Image = game.Image;

        await appDbContext.SaveChangesAsync();
        return new ServiceResponse() {Message = "Game updated", Success = true};

    }
    public async Task<ServiceResponse> DeleteGameAsync(int id)
    {
        var game = await appDbContext.Games.FirstOrDefaultAsync(g => g.Id == id);
        if(game is null)
        {
            return new ServiceResponse() { Message = "Game is not found", Success = false };
        }

        appDbContext.Games.Remove(game);
        await appDbContext.SaveChangesAsync();
        return new ServiceResponse() { Message = "Game is removed", Success = true };

    }
    public async Task<ServiceResponse> GetGameByIdAsync(int id)
{
    // Eager-load Genre so clients receive Genre.Name
    var game = await appDbContext.Games
                .Include(g => g.Genre)
                .FirstOrDefaultAsync(g => g.Id == id);

    if (game == null)
    {
        return new ServiceResponse
        {
            Success = false,
            Message = "Game not found"
        };
    }

    return new ServiceResponse
    {
        Success = true,
        Data = game
    };
}

    public async Task<ServiceResponse> GetGamesAsync()
{
    // Eager-load Genre for every game so clients can display genre names
    var games = await appDbContext.Games
                .Include(g => g.Genre)
                .ToListAsync();

    return new ServiceResponse
    {
        Success = true,
        Data = games
    };
}

}
