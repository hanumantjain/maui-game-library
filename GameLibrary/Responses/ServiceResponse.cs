using System;

namespace GameLibrary.Responses;

public class ServiceResponse
{
    public string? Message { get; set; }
    public bool Success { get; set; }
    public object? Data { get; set; }
}
