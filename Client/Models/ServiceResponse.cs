using System;

namespace Client.Models;

public class ServiceResponse
{
    public string? Message { get; set; }
    public bool Success { get; set; }
    public object? Data { get; set; }
}
