
using GameLibrary.Models;
using GameLibrary.Responses;

namespace WebApplication1.Services;

public interface IGameService
{
    Task<ServiceResponse> AddGameAsync(Games game);
    Task<ServiceResponse> UpdateGameAsync(int id, Games game);
    Task<ServiceResponse> DeleteGameAsync(int id);
    Task<ServiceResponse> GetGameByIdAsync(int id);
    Task<ServiceResponse> GetGamesAsync();

}
