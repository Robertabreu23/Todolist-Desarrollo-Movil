using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Services;

public class NotificacionUrgenteService : BackgroundService
{
    private static readonly TimeSpan AntelacionMinima = TimeSpan.FromHours(2);
    private static readonly TimeSpan IntervaloEscaneo = TimeSpan.FromMinutes(1);

    private readonly IServiceProvider _services;
    private readonly ILogger<NotificacionUrgenteService> _logger;

    public NotificacionUrgenteService(
        IServiceProvider services,
        ILogger<NotificacionUrgenteService> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "NotificacionUrgenteService iniciado. Escaneo cada {Intervalo}, antelación {Antelacion}.",
            IntervaloEscaneo, AntelacionMinima);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await EscanearAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fallo durante el escaneo de tareas urgentes.");
            }

            try
            {
                await Task.Delay(IntervaloEscaneo, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }

    private async Task EscanearAsync(CancellationToken ct)
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var ahora = DateTime.UtcNow;
        var umbral = ahora.Add(AntelacionMinima);

        var candidatas = await db.Tareas
            .Where(t =>
                t.Prioridad == Prioridad.Urgente &&
                !t.Completada &&
                !t.NotificacionEnviada &&
                t.FechaVencimiento != null &&
                t.FechaVencimiento <= umbral)
            .ToListAsync(ct);

        if (candidatas.Count == 0) return;

        foreach (var tarea in candidatas)
        {
            var faltante = tarea.FechaVencimiento!.Value - ahora;
            _logger.LogWarning(
                "[NOTIFICACIÓN] Tarea urgente próxima a vencer: Id={Id} Nombre=\"{Nombre}\" Vence={Vencimiento:o} Faltante={Faltante}",
                tarea.Id,
                tarea.Nombre,
                tarea.FechaVencimiento,
                faltante);

            tarea.NotificacionEnviada = true;
        }

        await db.SaveChangesAsync(ct);
    }
}
