using System;
using GameLibrary.Models;

namespace WebApplication1.Services;

public interface IGenreService
{
    Task<List<Genre>> GetGenresAsync();
}
