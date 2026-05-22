using TodoApi.Models;

namespace TodoApi.Repositories;

public interface ITareaRepository
{
    Task<IReadOnlyList<TareaItem>> GetAllAsync(CancellationToken ct = default);
    Task<TareaItem?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<TareaItem> AddAsync(TareaItem tarea, CancellationToken ct = default);
    Task<bool> UpdateAsync(TareaItem tarea, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<bool> SetCompletadaAsync(int id, bool completada, CancellationToken ct = default);
}
