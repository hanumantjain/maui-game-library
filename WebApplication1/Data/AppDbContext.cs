
using GameLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Data;

public class AppDbContext: DbContext
{  
    public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
    {
    }

    public DbSet<Games> Games { get; set; }
    public DbSet<Genre> Genre { get; set; }
}
