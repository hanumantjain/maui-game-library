using System;
using GameLibrary.Models;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

namespace WebApplication1.Services;

public class GenreService : IGenreService
{
    private readonly AppDbContext appDbContext;

    public GenreService(AppDbContext appDbContext)
    {
        this.appDbContext = appDbContext;
    }

    public async Task<List<Genre>> GetGenresAsync() => await appDbContext.Genre.ToListAsync();
}
