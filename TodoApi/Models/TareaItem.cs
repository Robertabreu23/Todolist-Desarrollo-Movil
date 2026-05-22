using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models;

public class TareaItem
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Descripcion { get; set; }

    public bool Completada { get; set; }

    public Prioridad Prioridad { get; set; } = Prioridad.Normal;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public DateTime? FechaVencimiento { get; set; }

    public bool NotificacionEnviada { get; set; }
}
