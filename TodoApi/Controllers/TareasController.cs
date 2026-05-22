using Microsoft.AspNetCore.Mvc;
using TodoApi.Dtos;
using TodoApi.Models;
using TodoApi.Repositories;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TareasController : ControllerBase
{
    private readonly ITareaRepository _repo;

    public TareasController(ITareaRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TareaResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TareaResponseDto>>> GetAll(CancellationToken ct)
    {
        var tareas = await _repo.GetAllAsync(ct);
        return Ok(tareas.Select(ToResponse));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TareaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TareaResponseDto>> GetById(int id, CancellationToken ct)
    {
        var tarea = await _repo.GetByIdAsync(id, ct);
        if (tarea is null) return NotFound();
        return Ok(ToResponse(tarea));
    }

    [HttpPost]
    [ProducesResponseType(typeof(TareaResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TareaResponseDto>> Create(TareaCreateDto dto, CancellationToken ct)
    {
        var tarea = new TareaItem
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            Prioridad = dto.Prioridad,
            FechaVencimiento = dto.FechaVencimiento is { } fv ? DateTime.SpecifyKind(fv, DateTimeKind.Utc) : null,
            FechaCreacion = DateTime.UtcNow,
            Completada = false,
            NotificacionEnviada = false
        };

        await _repo.AddAsync(tarea, ct);
        return CreatedAtAction(nameof(GetById), new { id = tarea.Id }, ToResponse(tarea));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, TareaUpdateDto dto, CancellationToken ct)
    {
        var tarea = await _repo.GetByIdAsync(id, ct);
        if (tarea is null) return NotFound();

        var fechaVencimientoNueva = dto.FechaVencimiento is { } fv
            ? DateTime.SpecifyKind(fv, DateTimeKind.Utc)
            : (DateTime?)null;

        var resetNotificacion =
            tarea.Prioridad != dto.Prioridad ||
            tarea.FechaVencimiento != fechaVencimientoNueva;

        tarea.Nombre = dto.Nombre;
        tarea.Descripcion = dto.Descripcion;
        tarea.Completada = dto.Completada;
        tarea.Prioridad = dto.Prioridad;
        tarea.FechaVencimiento = fechaVencimientoNueva;
        if (resetNotificacion) tarea.NotificacionEnviada = false;

        await _repo.UpdateAsync(tarea, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var ok = await _repo.DeleteAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }

    [HttpPatch("{id:int}/completada")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarcarCompletada(int id, TareaCompletadaDto dto, CancellationToken ct)
    {
        var ok = await _repo.SetCompletadaAsync(id, dto.Completada, ct);
        return ok ? NoContent() : NotFound();
    }

    private static TareaResponseDto ToResponse(TareaItem t) => new()
    {
        Id = t.Id,
        Nombre = t.Nombre,
        Descripcion = t.Descripcion,
        Completada = t.Completada,
        Prioridad = t.Prioridad,
        FechaCreacion = t.FechaCreacion,
        FechaVencimiento = t.FechaVencimiento
    };
}
