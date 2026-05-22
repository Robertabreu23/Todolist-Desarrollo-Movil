using System.ComponentModel.DataAnnotations;
using TodoApi.Models;

namespace TodoApi.Dtos;

public class TareaCreateDto
{
    [Required]
    [MaxLength(200)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Descripcion { get; set; }

    public Prioridad Prioridad { get; set; } = Prioridad.Normal;

    public DateTime? FechaVencimiento { get; set; }
}
