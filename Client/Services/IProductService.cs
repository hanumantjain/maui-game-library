using Client.Models;
using GameLibrary.Models;

namespace Client.Services;

public interface IProductService
{
    Task<List<Models.Genre>> GetGenresAsync();
    Task<ServiceResponse> AddProductAsync(Products product);

    Task<ServiceResponse> UpdateProductAsync(Products product);

    Task<ServiceResponse> DeleteProductAsync(int id);

    Task<List<Games>> GetProductAsync();
}
