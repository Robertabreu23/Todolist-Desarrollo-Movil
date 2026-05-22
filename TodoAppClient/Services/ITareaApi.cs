using TodoAppClient.Models;

namespace TodoAppClient.Services;

public interface ITareaApi
{
    Task<List<Tarea>> GetAllAsync(CancellationToken ct = default);
    Task<Tarea?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Tarea> CreateAsync(TareaCreate dto, CancellationToken ct = default);
    Task UpdateAsync(int id, TareaUpdate dto, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
    Task SetCompletadaAsync(int id, bool completada, CancellationToken ct = default);
}
