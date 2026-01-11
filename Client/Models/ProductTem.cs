using System;

namespace Client.Models;

public class ProductTem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public int GenreId { get; set; }

    public double Price { get; set; }

    public DateOnly ReleasedDate { get; set; }

    public string? GenreName { get; set; }

    // Keep the original base64 payload so edits can preserve or re-send the image
    public string? ImageBase64 { get; set; }

    public ImageSource? Image { get; set; }  
}
