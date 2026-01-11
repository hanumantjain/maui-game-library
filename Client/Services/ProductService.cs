using System.Net.Http.Json;
using System.Text;
using Client.Models;
using GameLibrary.Models;

namespace Client.Services;

public class ProductService : IProductService
{
    private readonly HttpClient httpClient;

    public ProductService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<ServiceResponse> AddProductAsync(Products product)
    {
        try
        {
                // Quick client-side validation for required fields to surface clearer errors before sending
                var missingFields = new List<string>();
                if (product == null)
                    return new ServiceResponse { Success = false, Message = "Product is null" };

                if (string.IsNullOrWhiteSpace(product.Name))
                    missingFields.Add("Name");
                if (string.IsNullOrWhiteSpace(product.Description))
                    missingFields.Add("Description");
                if (product.GenreId <= 0)
                    missingFields.Add("GenreId");
                if (product.Price <= 0)
                    missingFields.Add("Price");
                if (product.ReleasedDate == default)
                    missingFields.Add("ReleasedDate");
                if (string.IsNullOrWhiteSpace(product.Image))
                    missingFields.Add("Image");

                if (missingFields.Any())
                {
                    var msg = $"Missing required fields: {string.Join(", ", missingFields)}";
                    System.Diagnostics.Debug.WriteLine($"AddProductAsync validation failed: {msg}");
                    return new ServiceResponse { Success = false, Message = msg };
                }

                // Serialize payload explicitly so we can log it
                var payloadJson = System.Text.Json.JsonSerializer.Serialize(product);
                System.Diagnostics.Debug.WriteLine($"AddProductAsync: POST payload: {payloadJson}");

                using var content = new StringContent(payloadJson, Encoding.UTF8);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                var response = await httpClient.PostAsync("http://localhost:5068/api/Games", content);

            var responseBody = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"AddProductAsync: Response {(int)response.StatusCode}: {responseBody}");

            if (!response.IsSuccessStatusCode)
            {
                return new ServiceResponse { Success = false, Message = $"Server returned {(int)response.StatusCode}: {responseBody}" };
            }

            // Try to read ServiceResponse first (preferred)
            var result = System.Text.Json.JsonSerializer.Deserialize<ServiceResponse>(responseBody,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (result != null)
                return result;

            // Fallback: server returned the created Games object; treat that as success
            var createdGame = System.Text.Json.JsonSerializer.Deserialize<Games?>(responseBody,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (createdGame != null)
            {
                return new ServiceResponse { Success = true, Message = "Product added", Data = createdGame };
            }

            // Generic success fallback
            return new ServiceResponse { Success = true, Message = "Product added", Data = responseBody };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"AddProductAsync exception: {ex}");
            return new ServiceResponse { Success = false, Message = ex.Message };
        }
    }

    public async Task<ServiceResponse> UpdateProductAsync(Products product)
    {
        try
        {
            if (product == null)
                return new ServiceResponse { Success = false, Message = "Product is null" };

            var payloadJson = System.Text.Json.JsonSerializer.Serialize(product);
            System.Diagnostics.Debug.WriteLine($"UpdateProductAsync: PUT payload: {payloadJson}");

            using var content = new StringContent(payloadJson, Encoding.UTF8);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var response = await httpClient.PutAsync($"http://localhost:5068/api/Games/{product.Id}", content);
            var responseBody = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"UpdateProductAsync: Response {(int)response.StatusCode}: {responseBody}");

            if (!response.IsSuccessStatusCode)
            {
                return new ServiceResponse { Success = false, Message = $"Server returned {(int)response.StatusCode}: {responseBody}" };
            }

            var result = System.Text.Json.JsonSerializer.Deserialize<ServiceResponse>(responseBody,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result != null)
                return result;

            return new ServiceResponse { Success = true, Message = "Product updated", Data = responseBody };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"UpdateProductAsync exception: {ex}");
            return new ServiceResponse { Success = false, Message = ex.Message };
        }
    }

    public async Task<ServiceResponse> DeleteProductAsync(int id)
    {
        try
        {
            var response = await httpClient.DeleteAsync($"http://localhost:5068/api/Games/{id}");
            var responseBody = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"DeleteProductAsync: Response {(int)response.StatusCode}: {responseBody}");

            if (!response.IsSuccessStatusCode)
            {
                return new ServiceResponse { Success = false, Message = $"Server returned {(int)response.StatusCode}: {responseBody}" };
            }

            var result = System.Text.Json.JsonSerializer.Deserialize<ServiceResponse>(responseBody,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result != null)
                return result;

            return new ServiceResponse { Success = true, Message = "Product deleted", Data = responseBody };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"DeleteProductAsync exception: {ex}");
            return new ServiceResponse { Success = false, Message = ex.Message };
        }
    }
    public async Task<List<Games>> GetProductAsync()
    {
        try
        {
            var responseMsg = await httpClient.GetAsync("http://localhost:5068/api/Games");
            var responseBody = await responseMsg.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"GetProductAsync: Response {(int)responseMsg.StatusCode}: {responseBody}");

            if (!responseMsg.IsSuccessStatusCode)
            {
                System.Diagnostics.Debug.WriteLine($"GetProductAsync failed with status {(int)responseMsg.StatusCode}");
                return new List<Games>();
            }

            var sr = System.Text.Json.JsonSerializer.Deserialize<GameLibrary.Responses.ServiceResponse>(responseBody,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (sr == null)
                return new List<Games>();

            if (!sr.Success)
            {
                System.Diagnostics.Debug.WriteLine($"GetProductAsync: service reported failure: {sr.Message}");
                return new List<Games>();
            }

            if (sr.Data == null)
                return new List<Games>();

            if (sr.Data is System.Text.Json.JsonElement je)
            {
                try
                {
                    var dataJson = je.GetRawText();
                    var list = System.Text.Json.JsonSerializer.Deserialize<List<Games>>(dataJson,
                        new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Games>();
                    return list;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"GetProductAsync: failed to parse Data to List<Games>: {ex}");
                    return new List<Games>();
                }
            }

            // Fallback: serialize Data and try to parse
            try
            {
                var dataJson = System.Text.Json.JsonSerializer.Serialize(sr.Data);
                var list = System.Text.Json.JsonSerializer.Deserialize<List<Games>>(dataJson,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Games>();
                return list;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetProductAsync: fallback parse failed: {ex}");
                return new List<Games>();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetProductAsync exception: {ex}");
            return new List<Games>();
        }
    }

    public async Task<List<Models.Genre>> GetGenresAsync()
    {
        try
        {
            var response = await httpClient.GetAsync("http://localhost:5068/api/Games/genre");
            var responseBody = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"GetGenresAsync: Response {(int)response.StatusCode}: {responseBody}");

            if (!response.IsSuccessStatusCode)
            {
                System.Diagnostics.Debug.WriteLine($"GetGenresAsync failed with status {(int)response.StatusCode}");
                return new List<Models.Genre>();
            }

            var list = System.Text.Json.JsonSerializer.Deserialize<List<GameLibrary.Models.Genre>>(responseBody,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<GameLibrary.Models.Genre>();

            return list.Select(g => new Models.Genre { Id = g.Id, Name = g.Name }).ToList();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetGenresAsync exception: {ex}");
            return new List<Models.Genre>();
        }
    }

}
