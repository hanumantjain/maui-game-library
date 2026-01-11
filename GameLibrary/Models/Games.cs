using System;

namespace GameLibrary.Models;

public class Games
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Genre? Genre { get; set; }

    public int GenreId { get; set; }

    public double Price { get; set; }

    public DateOnly ReleasedDate { get; set; }

    public string Image { get; set; }   
}
