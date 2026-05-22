using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using TodoAppClient.Models;

namespace TodoAppClient.Services;

public class TareaApi : ITareaApi
{
    private readonly HttpClient _http;

    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public TareaApi(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<Tarea>> GetAllAsync(CancellationToken ct = default)
    {
        var result = await _http.GetFromJsonAsync<List<Tarea>>("/api/tareas", JsonOptions, ct);
        return result ?? new List<Tarea>();
    }

    public async Task<Tarea?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var response = await _http.GetAsync($"/api/tareas/{id}", ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Tarea>(JsonOptions, ct);
    }

    public async Task<Tarea> CreateAsync(TareaCreate dto, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("/api/tareas", dto, JsonOptions, ct);
        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<Tarea>(JsonOptions, ct);
        return created!;
    }

    public async Task UpdateAsync(int id, TareaUpdate dto, CancellationToken ct = default)
    {
        var response = await _http.PutAsJsonAsync($"/api/tareas/{id}", dto, JsonOptions, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var response = await _http.DeleteAsync($"/api/tareas/{id}", ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return;
        response.EnsureSuccessStatusCode();
    }

    public async Task SetCompletadaAsync(int id, bool completada, CancellationToken ct = default)
    {
        var payload = new CompletadaPatch { Completada = completada };
        var response = await _http.PatchAsJsonAsync($"/api/tareas/{id}/completada", payload, JsonOptions, ct);
        response.EnsureSuccessStatusCode();
    }
}
