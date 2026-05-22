using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Repositories;

public class TareaRepository : ITareaRepository
{
    private readonly AppDbContext _db;

    public TareaRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<TareaItem>> GetAllAsync(CancellationToken ct = default)
    {
        return await _db.Tareas
            .AsNoTracking()
            .OrderByDescending(t => t.Prioridad)
            .ThenBy(t => t.FechaVencimiento ?? DateTime.MaxValue)
            .ThenByDescending(t => t.FechaCreacion)
            .ToListAsync(ct);
    }

    public Task<TareaItem?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return _db.Tareas.FirstOrDefaultAsync(t => t.Id == id, ct);
    }

    public async Task<TareaItem> AddAsync(TareaItem tarea, CancellationToken ct = default)
    {
        _db.Tareas.Add(tarea);
        await _db.SaveChangesAsync(ct);
        return tarea;
    }

    public async Task<bool> UpdateAsync(TareaItem tarea, CancellationToken ct = default)
    {
        _db.Tareas.Update(tarea);
        var rows = await _db.SaveChangesAsync(ct);
        return rows > 0;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var tarea = await _db.Tareas.FindAsync(new object[] { id }, ct);
        if (tarea is null) return false;
        _db.Tareas.Remove(tarea);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> SetCompletadaAsync(int id, bool completada, CancellationToken ct = default)
    {
        var tarea = await _db.Tareas.FindAsync(new object[] { id }, ct);
        if (tarea is null) return false;
        tarea.Completada = completada;
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
