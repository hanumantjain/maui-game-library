using System;

namespace Client.Models;

public class Products
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public int GenreId { get; set; }

    public double Price { get; set; }

    public DateOnly ReleasedDate { get; set; }

    public string Image { get; set; } = string.Empty;   

}
